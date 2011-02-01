using System;

namespace NServiceBus.Unicast
{
    /// <summary>
    /// Data containing whether processing succeeded or not for raising in events.
    /// </summary>
    public class MessageCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Whether processing succeeded or not.
        /// </summary>
        public bool Succeeded { get; set; }
    }
}