using System;

namespace UkadTask.Domain
{
    public class CrawledPage
    {
        public string Url { get; set; }

        //milliseconds
        public int ResponseTime { get; set; }
    }
}