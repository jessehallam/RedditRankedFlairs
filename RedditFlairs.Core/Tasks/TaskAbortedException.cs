using System;
using System.Runtime.Serialization;

namespace RedditFlairs.Core.Tasks
{
    [Serializable]
    public class TaskAbortedException : Exception
    {
        public TaskAbortedException()
        {
        }

        public TaskAbortedException(string message) : base(message)
        {
        }

        public TaskAbortedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TaskAbortedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}