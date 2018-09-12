using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ReleaseExecutor
{
    public static class AsyncOperationExtension
    {
        public static TaskAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation ao)
        {
            var tcs = new TaskCompletionSource<AsyncOperation>();
            Wait(ao, tcs);
            return tcs.Task.GetAwaiter();
        }

        private static void Wait(AsyncOperation ao, TaskCompletionSource<AsyncOperation> tcs)
        {
            EditorApplication.delayCall += () =>
            {
                if (ao.isDone == false)
                {
                    Wait(ao, tcs);
                    return;
                }

                tcs.SetResult(ao);
            };
        }
    }
}
