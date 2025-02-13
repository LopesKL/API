using System.Net;

namespace API.Application.Dto.Response
{
    public class OkDto<T>
    {
        public OkDto(T response)
        {
            Response = response;
        }

        public T Response { get; private set; }
        public HttpStatusCode StatusCode => HttpStatusCode.OK;
    }


    public class OkDto2<T>
    {
        public OkDto2(T response)
        {
            Response = response;
        }

        public T Response { get; private set; }
        public HttpStatusCode StatusCode => HttpStatusCode.OK;
    }
}
