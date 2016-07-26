using System;
using System.Collections.Generic;

namespace UkadTask.Domain
{
    public partial class Page
    {
        public Page()
        {
            this.History = new List<History>();
        }

        public int Id { get; set; }
        public int HostId { get; set; }
        public string Url { get; set; }

        //Min page response time in miliseconds
        public int MinResponseTime { get; set; }

        //Max page response time in miliseconds
        public int MaxResponseTime { get; set; }
        public virtual ICollection<History> History { get; set; }
        public virtual Host Host { get; set; }
    }
}
