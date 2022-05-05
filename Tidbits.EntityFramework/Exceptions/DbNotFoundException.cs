using System.Runtime.Serialization;

namespace Tidbits.EntityFramework.Exceptions
{
    [Serializable]
    public class DbNotFoundException : Exception
    {
#nullable enable

        public DbNotFoundException(string? message, Exception? exception) : base(message, exception)
        {
        }
#nullable disable
        protected DbNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
