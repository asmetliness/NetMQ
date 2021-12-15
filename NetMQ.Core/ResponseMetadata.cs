using System;
using MessagePack;

namespace NetMQ.Core
{
    [MessagePackObject()]
    public class ResponseMetadata
    {
        [Key(0)]
        public string RequestId { get; set; }
        [Key(1)]
        public bool IsSuccess { get; set; }
        [Key(2)]
        public ErrorData ErrorData { get; set; }

        public static ResponseMetadata CreateSuccess(RequestMetadata request)
        {
            return new ResponseMetadata()
            {
                RequestId = request.RequestId,
                IsSuccess = true,
                ErrorData = null
            };
        }

        public static ResponseMetadata CreateError(RequestMetadata request, Exception ex)
        {
            return new ResponseMetadata()
            {
                RequestId = request.RequestId,
                IsSuccess = false,
                ErrorData = new ErrorData()
                {
                    Message = ex.ToString()
                }
            };
        }

    }


    [MessagePackObject()]
    public class ErrorData
    {
        [Key(0)]
        public string Message { get; set; }
    }
}