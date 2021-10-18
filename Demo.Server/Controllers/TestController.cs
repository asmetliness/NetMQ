using System.Threading.Tasks;
using Core;
using NetMQ.Server.Controllers;

namespace Server.Controllers
{
    public class TestController: BaseController<RequestDto, ResponseDto>
    {
        public override string RequestDtoKey => "Test";
        
        public override Task<ResponseDto> Handle(RequestDto request)
        {
            return Task.FromResult(ResponseDto.FromRequest(request));
        }
    }
}