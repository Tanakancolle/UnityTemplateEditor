using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace TemplateEditor
{
    public class TemplatePrefabCreator
    {
        public static readonly string TempCreatePrefabSettingIdsKey = "TemplateEditor.Setting.PrefabCreates";
        public static readonly string TempCreateScriptPathKeyFormat = "TemplateEditor.Setting.CreatePath.{0}";

        public static void AddTempCreatePrefabSetting(TemplateSetting setting, string scriptPath)
        {
            // Add Setting Id
            var prefabSettings = SessionState.GetIntArray(TempCreatePrefabSettingIdsKey, null);
            if (prefabSettings == null)
            {
                prefabSettings = new int[] {setting.GetInstanceID()};
            }
            else
            {
                var length = prefabSettings.Length;
                var newSettings = new int[length + 1];
                prefabSettings.CopyTo(newSettings, 0);
                newSettings[length] = setting.GetInstanceID();

                prefabSettings = newSettings;
            }

            SessionState.SetIntArray(TempCreatePrefabSettingIdsKey, prefabSettings);

            // Set Script Path
            SessionState.SetString(string.Format(TempCreateScriptPathKeyFormat, setting.GetInstanceID()), scriptPath);
        }

        public static void CreateTempSettingPrefab()
        {
            var ids = GetOutTempSettings();
            if (ids == null)
            {
                return;
            }

            var settingDic = new Dictionary<string, List<TemplateSetting>>(ids.Length);
            foreach (var id in ids)
            {
                var setting = EditorUtility.InstanceIDToObject(id) as TemplateSetting;
                if (setting == null)
                {
                    continue;
                }

                // 同じプレハブ生成パス & 同じプレハブなら一緒にする
                var key = setting.PrefabPath + "/" + setting.PrefabName + setting.DuplicatePrefab.GetInstanceID();
                if (settingDic.ContainsKey(key))
                {
                    settingDic[key].Add(setting);
                }
                else
                {
                    settingDic.Add(key, new List<TemplateSetting>() {setting});
                }
            }

            foreach (var settings in settingDic.Values)
            {
                DuplicatePrefab(settings.ToArray());
            }
        }

        private static int[] GetOutTempSettings()
        {
            var ids = SessionState.GetIntArray(TempCreatePrefabSettingIdsKey, null);
            SessionState.EraseIntArray(TempCreatePrefabSettingIdsKey);
            return ids;
        }

        private static void DuplicatePrefab(TemplateSetting[] settings)
        {
            if (settings == null || settings.Length == 0)
            {
                Debug.LogWarning("設定ファイルがありません");
                return;
            }

            var components = new List<Component>(settings.Length);

            foreach (var setting in settings)
            {
                // Load Attach Script
                var scriptKye = string.Format(TempCreateScriptPathKeyFormat, setting.GetInstanceID());
                var scriptPath = SessionState.GetString(scriptKye, null);
                var mono = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
                SessionState.EraseString(scriptKye);

                if (mono == null)
                {
                    Debug.LogErrorFormat("{0} : スクリプトファイルがありませんでした", scriptPath);
                    return;
                }

                var scriptType = mono.GetClass();

                if (scriptType == null)
                {
                    Debug.LogErrorFormat("{0} : クラスを取得できませんでした。ファイル名とクラス名が違う可能性があります", mono.name);
                    return;
                }

                components.Add(setting.AttachTarget.AddComponent(scriptType));
            }

            // コピーパスは同じなはずのため、最初のを使用する
            var prefabPath = AssetDatabase.GetAssetPath(settings[0].DuplicatePrefab);
            var createPath = settings[0].PrefabPath;
            var prefabName = settings[0].PrefabName;
            if (string.IsNullOrEmpty(createPath))
            {
                // 空白の場合はアクティブなパスへ生成
                createPath = TemplateUtility.GetActiveFolder();
            }

            if (string.IsNullOrEmpty(prefabName))
            {
                // 空白の場合はコピー元のプレハブ名
                prefabName = Path.GetFileName(prefabPath);
            }

            prefabName += Path.GetExtension(prefabName) == string.Empty ? ".prefab" : string.Empty;
            var createFullPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(createPath, prefabName));
            AssetDatabase.CopyAsset(prefabPath, createFullPath);

            foreach (var component in components)
            {
                UnityEngine.Object.DestroyImmediate(component, true);
            }
        }
    }
}
