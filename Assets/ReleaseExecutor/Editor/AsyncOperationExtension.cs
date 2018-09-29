using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ReleaseExecutor
{
    public static class AsyncOperationExtension
    {
        public static TaskAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation asyncOperation)
        {
            var tcs = new TaskCompletionSource<AsyncOperation>();
            asyncOperation.completed += (ao) => { tcs.SetResult(ao); };
            return tcs.Task.GetAwaiter();
        }
    }
}
