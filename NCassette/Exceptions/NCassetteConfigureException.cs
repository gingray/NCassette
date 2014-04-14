using System;
using System.Runtime.Serialization;

namespace NCassetteLib.Exceptions
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
