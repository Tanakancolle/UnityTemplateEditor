using System;

namespace ReleaseExecutor
{
    public interface IReleaseExecutor
    {
        void Execute(ReleaseExecutorWindow.ReleaseParameter parameter, Action onComplete);
    }
}
