namespace GlobalAuthorizerExample.Features.GetCourseVideoDetails
{
    public partial class GetCourseVideoDetails
    {
        public class Response
        {
            public int VideoId { get; set; }
            public string Title { get; set; }
            public long Duration { get; set; }
            public string Thumbnail { get; set; }
        }

    }

}
