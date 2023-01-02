using MediatR;

namespace GlobalAuthorizerExample.Features.GetCourseVideoDetails
{
    public partial class GetCourseVideoDetails
    {
        public class Request : IRequest<Response>
        {
            public Request(int videoId)
            {
                VideoId = videoId;
            }

            public int VideoId { get; set; }
        }
    }

}
