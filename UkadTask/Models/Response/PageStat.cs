
namespace UkadTask.Models.Response
{
    public class PageStat
    {
        public string PageUrl { get; set; }
        public int MaxResponseTime { get; set; }
        public int MinResponseTime { get; set; }
        public int CurrentResponseTime { get; set; }
    }
}