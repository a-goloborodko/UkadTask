using System.Collections.Generic;

namespace UkadTask.Models.Response
{
    public class MeasurementResult
    {
        public IEnumerable<PageStat> Pages { get; set; }
        public bool Success { get; set; }
        public string HostUrl { get; set; }
        public string Error { get; set; }
        public double AvgResponseTime { get; set; }

        public static MeasurementResult CreateResultWithError(string messageError, string hostUrl)
        {
            return new MeasurementResult { Success = false, Error = messageError, HostUrl = hostUrl };
        }
    }
}