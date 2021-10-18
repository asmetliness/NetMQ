using MessagePack;

namespace NetMQ.Core
{
    public static class NetMqExtensions
    {

        public static T FromMessagePack<T>(this NetMQFrame frame)
        {
            return MessagePackSerializer.Deserialize<T>(frame.Buffer);
        }
    }
}