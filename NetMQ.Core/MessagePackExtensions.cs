using MessagePack;

namespace NetMQ.Core
{
    public static class MessagePackExtensions
    {

        public static byte[] ToMessagePack(this object obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }
    }
}