using MessagePack;

namespace NetMQ.Core
{
    [MessagePackObject()]
    public class RequestMetadata
    {
        [Key(0)]
        public string RequestId { get; set; }
        [Key(1)]
        public string RequestDtoKey { get; set; }
    }
}