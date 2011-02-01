using System;

namespace NServiceBus.Utils
{
    ///<summary>
    /// Runs an action when disposed to facilitate using {} blocks
    ///</summary>
    public class RunOnDispose : IDisposable
    {
        private Action _action;

        ///<summary>
        /// Creates a new RunOnDispose that runs the supplied action when disposed
        ///</summary>
        ///<param name="action"></param>
        public RunOnDispose(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            var action = _action;
            _action = null;
            if (action != null)
                action();
        }
    }
}