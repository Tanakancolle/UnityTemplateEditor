# テンプレートエディタ
Unityのエディタ拡張で作られた、テンプレートからファイルを生成できるツールです。

## チュートリアル
### ・基本
#### Step.1
テンプレート設定ファイルを作成します

![1-1](https://user-images.githubusercontent.com/18282136/44031010-8f4d93be-9f3d-11e8-843c-f04b7d733136.png)

#### Step.2
テンプレート設定ファイルに内容を記述します

![1-2](https://user-images.githubusercontent.com/18282136/44031201-31b518a2-9f3e-11e8-893e-b027d526fb45.gif)

#### Step.3
下部にある **Create** をクリックします

![1-3](https://user-images.githubusercontent.com/18282136/44031283-6e4cfa14-9f3e-11e8-858b-4e716896f210.png)

#### Step.4
ファイルが生成されます

![1-4](https://user-images.githubusercontent.com/18282136/44031369-be457b90-9f3e-11e8-879e-35d07e5a2333.png)

## ・右クリックメニューに追加
#### Step.1
先ほどのテンプレート設定ファイルの **Add Asset Menu** にチェックを入れます

![2-1](https://user-images.githubusercontent.com/18282136/44031619-7fefaca2-9f3f-11e8-950b-558f7b4aa5a0.png)

#### Step.2
右クリックメニューにテンプレート設定ファイルと同じ名前の項目が追加されているので、クリックします

![2-2](https://user-images.githubusercontent.com/18282136/44031738-e921109e-9f3f-11e8-99b0-9907d3bc2005.png)

#### Step.3
ウィンドウが表示されるので、 **Create** をクリックするとファイルが生成されます

![2-3](https://user-images.githubusercontent.com/18282136/44031814-2a8ad06a-9f40-11e8-97be-dcd9c1b32863.png)

### ・「Pre Process」の活用
#### Step.1
先ほどのテンプレート設定ファイルの **Pre Process** へ **UnityCSharpTemplatePathProcessor** を追加します

![3-1](https://user-images.githubusercontent.com/18282136/44032040-ed823536-9f40-11e8-9765-c55683ef197c.png)

#### Step.2
**Create Path** と **Script Name** へ **UnityTemplatePath** と **UnityTemplateName** を設定します

![3-2](https://user-images.githubusercontent.com/18282136/44032731-f740d3d2-9f42-11e8-9540-8206bb66f500.gif)

#### Step.3
**Create** をクリックすると、右クリックメニューにある「Create/C# Script」で生成されるスクリプトが変更され *ません！*

**Overwrite Type** を **Replace** にして実行すると変更されます。
**※変更すると元に戻せないので、注意してください**

![3-3](https://user-images.githubusercontent.com/18282136/44034487-2891dd78-9f48-11e8-9fb4-43cb34387b1d.png)

### ・特殊な置き換え
#### Step.1
新しいテンプレート設定ファイルを作成し、ファイル名を **UsingTemplateSetting** に変更します

#### Step.2
**Code** へ適当なusingを宣言します

![4-2](https://user-images.githubusercontent.com/18282136/44035836-6139ddb2-9f4b-11e8-850a-79310165f6bd.png)

#### Step.3
もう一つテンプレート設定ファイルを作成し **Pre Process** に先ほど作成した **UsingTemplateSetting** を設定します

![4-3](https://user-images.githubusercontent.com/18282136/44035435-786d4704-9f4a-11e8-86bc-bdbd5c89a829.png)

#### Step.4
**Pre Process** に **TemplateSettingCodeArrayProcessor** を追加します

![4-4](https://user-images.githubusercontent.com/18282136/44035613-d9881cd0-9f4a-11e8-8cd3-25cb90220033.png)

#### Step.5
**Code** へ **{<Repeat:{0}:TemplateSettingCodeArray>}** を入力します

![4-5](https://user-images.githubusercontent.com/18282136/44035972-a96560e8-9f4b-11e8-8e3b-07ef8358957e.png)

#### Step.6
適当に他の設定内容を記述します

![4-6](https://user-images.githubusercontent.com/18282136/44036102-fea0e7bc-9f4b-11e8-99ec-064d21b570fd.png)

#### Step.7
**Create** をクリックすると、usingが宣言されたファイルが作成されます

![4-7](https://user-images.githubusercontent.com/18282136/44036259-508be158-9f4c-11e8-80ff-fcd9e9b4238d.png)

**ここの詳細な説明はReplace.mdで行います**