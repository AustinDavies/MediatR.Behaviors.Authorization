using GlobalAuthorizerExample.Common.Authorization;
using MediatR;
using MediatR.Behaviors.Authorization;

namespace GlobalAuthorizerExample.Features.GetCourseVideoDetails
{
    public partial class GetCourseVideoDetails
    {
        public class Handler : IRequestHandler<Request, Response>
        {
            public Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var random = new Random(request.VideoId);

                return Task.FromResult(new Response
                {
                    VideoId = request.VideoId,
                    Title = $"Video {request.VideoId}",
                    Duration = random.Next(10, 60*60*10),
                    Thumbnail = $"https://localhost:3000/cdn/video-thumbnails/{request.VideoId}"
                });
            }
        }
    }
}
