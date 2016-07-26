using System;
using System.Collections.Generic;

namespace UkadTask.Models.Response
{
    public class PageModel
    {
        public string Url { get; set; }
        public IEnumerable<HistoryModel> History { get; set; }
    }
}