using MessagePack;

namespace Core
{
    [MessagePackObject(keyAsPropertyName:false)]
    public class ResponseDto
    {
        [Key(0)]
        public string ConcatMessage { get; set; }
        
        public static ResponseDto FromRequest(RequestDto request)
        {
            return new ResponseDto()
            {
                ConcatMessage = $"{request.Message}_{request.Message2}_{request.Number}",
            };
        }
    }
}