using System;
using System.Collections.Generic;

namespace UkadTask.Domain
{
    public partial class History
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        //Page response time in miliseconds
        public int ResponseTime { get; set; }
        public DateTime Date { get; set; }
        public virtual Page Page { get; set; }
    }
}
