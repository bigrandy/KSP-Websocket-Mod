using System;
using System.Runtime.Serialization;

namespace TestKSP1
{
    [Serializable]
    internal class UnableToWriteException : Exception
    {
        public UnableToWriteException()
        {
        }

        public UnableToWriteException(string message) : base(message)
        {
        }

        public UnableToWriteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnableToWriteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}