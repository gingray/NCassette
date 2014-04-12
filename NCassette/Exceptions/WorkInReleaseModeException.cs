using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NCassette.Exceptions
{
    [Serializable]
    public class WorkInReleaseModeException : Exception
    {
        public WorkInReleaseModeException()
        {
        }

        public WorkInReleaseModeException(string message) : base(message)
        {
        }

        public WorkInReleaseModeException(string message, Exception inner) : base(message, inner)
        {
        }

        protected WorkInReleaseModeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
