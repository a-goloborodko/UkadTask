using System.Collections.Generic;
using UkadTask.Models.Response;

namespace UkadTask.Models.Response
{
    public class HistoryResponseModel
    {
        public string HostUrl { get; set; }
        public ICollection<PageModel> Pages { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }

        public static HistoryResponseModel CreateFailureHistoryModel(string hostUrl, string errorMessage)
        {
            return new HistoryResponseModel { Success = false, HostUrl = hostUrl, Error = errorMessage };
        }
    }
}
