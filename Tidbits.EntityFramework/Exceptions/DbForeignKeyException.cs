using System.Runtime.Serialization;

namespace Tidbits.EntityFramework.Exceptions
{
    [Serializable]
    public class DbForeignKeyException : Exception
    {
#nullable enable

        public DbForeignKeyException(string? message, Exception? exception) : base(message, exception)
        {
        }
#nullable disable
        protected DbForeignKeyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
