using System;
using System.Runtime.Serialization;

namespace TestKSP1
{
    [Serializable]
    internal class UnableToOpenSocketException : Exception
    {
        public UnableToOpenSocketException()
        {
        }

        public UnableToOpenSocketException(string message) : base(message)
        {
        }

        public UnableToOpenSocketException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnableToOpenSocketException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}