using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NCassette.Exceptions
{
    [Serializable]
    public class NCassetteConfigureException : Exception
    {
        public NCassetteConfigureException()
        {
        }

        public NCassetteConfigureException(string message) : base(message)
        {
        }

        public NCassetteConfigureException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NCassetteConfigureException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
