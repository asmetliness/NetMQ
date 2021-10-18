using System;
using MessagePack;

namespace Core
{
    [MessagePackObject(keyAsPropertyName:false)]
    public class RequestDto
    {
        [Key(0)]
        public string Message { get; set; }
        [Key(1)]
        public string Message2 { get; set; }
        [Key(2)]
        public int Number { get; set; }

        public static RequestDto Create()
        {
            return new RequestDto()
            {
                Message = "Message",
                Message2 = Guid.NewGuid().ToString(),
                Number = 10000
            };
        }
    }
}