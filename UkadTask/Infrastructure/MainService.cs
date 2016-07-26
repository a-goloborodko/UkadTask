using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UkadTask.Domain;
using UkadTask.Infrastructure;
using UkadTask.Repository;
using UkadTask.Models.Response;

namespace UkadTask.Infrastructure
{
    public class MainService : IDisposable
    {
        private SiteCrawler _crawler;
        private IRepository _repository;
        private HelperService _helper;
        private bool _disposed;

        public MainService()
        {
            _crawler = new SiteCrawler(100);
            _repository = new EFRepository();
            _helper = new HelperService();
        }

        public async Task<MeasurementResult> MeasureSiteAsync(string url)
        {
            bool isExist = await _helper.IsWebsiteExixtAsync(url);
            if (isExist)
            {
                IEnumerable<CrawledPage> mesuaredPages;
                IEnumerable<string> urls = await _helper.GetSitemapAsync(url);
                if (urls != null)
                    mesuaredPages = await _crawler.CrawlAsync((IList<string>)urls);
                else
                    mesuaredPages = await _crawler.CrawlAsync(url);

                IEnumerable<PageStat> pages = await SaveResultsAsync(mesuaredPages);

                return PrepareResult(pages);
            }

            return MeasurementResult.CreateResultWithError("Unable to find a website.Make sure that url is correct", url);
        }

        public HistoryResponseModel GetHistory(string hostUrl)
        {
            if (string.IsNullOrWhiteSpace(hostUrl))
                return HistoryResponseModel.CreateFailureHistoryModel(hostUrl, "Please enter url");

            var grouped = _repository.GetHistoryForHost(hostUrl)
                .GroupBy(x => x.Page.Url).ToList();

            if (grouped.Count == 0)
                return HistoryResponseModel.CreateFailureHistoryModel(hostUrl, "History is empty");

            HistoryResponseModel result = new HistoryResponseModel
            {
                HostUrl = hostUrl,
                Success = true,
                Pages = new List<PageModel>()
            };

            foreach (var page in grouped)
            {
                PageModel newPage = new PageModel();
                newPage.Url = page.Key;
                newPage.History = page.OrderByDescending(x => x.Date).Select(x => new HistoryModel
                {
                    Date = x.Date.ToString("MM/dd/yyyy HH:mm:ss"),
                    TimeResponse = x.ResponseTime
                }).ToList();

                result.Pages.Add(newPage);
            }

            return result;
        }

        public bool IsHostExistInDB(string url)
        {
            string hostUrl = _helper.GetHostFromUrl(url);
            Host findedHost = _repository.GetHostByUrl(hostUrl);
            return findedHost != null;
        }

        #region private methods
        private async Task<IEnumerable<PageStat>> SaveResultsAsync(IEnumerable<CrawledPage> mesuaredPages)
        {
            if (mesuaredPages.Count() == 0)
                return null;

            bool isHostExist = IsHostExistInDB(mesuaredPages.First().Url);
            if (!isHostExist)
                return await AddNewHostAndHistoryAsync(mesuaredPages);
            else
                return await AddHistoryAsync(mesuaredPages);
        }

        private async Task<IEnumerable<PageStat>> AddNewHostAndHistoryAsync(IEnumerable<CrawledPage> mesuaredPages)
        {
            //result of pages' measurements
            List<PageStat> resultStat = new List<PageStat>(mesuaredPages.Count());

            Host host = new Host
            {
                HostName = _helper.GetHostFromUrl(mesuaredPages.First().Url)
            };

            foreach (CrawledPage page in mesuaredPages)
            {
                Page newPage = new Page
                {
                    Url = page.Url,
                    MaxResponseTime = page.ResponseTime,
                    MinResponseTime = page.ResponseTime
                };

                resultStat.Add(new PageStat
                {
                    PageUrl = page.Url,
                    MaxResponseTime = page.ResponseTime,
                    MinResponseTime = page.ResponseTime,
                    CurrentResponseTime = page.ResponseTime
                });

                newPage.History.Add(new History
                {
                    Date = DateTime.Now,
                    ResponseTime = page.ResponseTime
                });

                host.Pages.Add(newPage);
            }

            _repository.AddHost(host);

            await _repository.SaveChangesAsync();

            return resultStat;
        }

        private async Task<IEnumerable<PageStat>> AddHistoryAsync(IEnumerable<CrawledPage> mesuaredPages)
        {
            //result of pages' measurements
            List<PageStat> resultStat = new List<PageStat>(mesuaredPages.Count());

            string hostUrl = _helper.GetHostFromUrl(mesuaredPages.First().Url);
            Host host = _repository.GetHostByUrlIncludPages(hostUrl);

            foreach (CrawledPage page in mesuaredPages)
            {
                Page pageToUpdate = host.Pages.FirstOrDefault(x => x.Url == page.Url);
                //update page and insert history in to database
                if (pageToUpdate != null)
                {
                    //update max and min response time
                    if (pageToUpdate.MaxResponseTime < page.ResponseTime)
                        pageToUpdate.MaxResponseTime = page.ResponseTime;
                    if (pageToUpdate.MinResponseTime > page.ResponseTime)
                        pageToUpdate.MinResponseTime = page.ResponseTime;

                    pageToUpdate.History.Add(new History
                    {
                        Date = DateTime.Now,
                        ResponseTime = page.ResponseTime
                    });

                    resultStat.Add(new PageStat
                        {
                            PageUrl = pageToUpdate.Url,
                            MaxResponseTime = pageToUpdate.MaxResponseTime,
                            MinResponseTime = pageToUpdate.MinResponseTime,
                            CurrentResponseTime = page.ResponseTime
                        });

                    _repository.UpdatePage(pageToUpdate);
                }
                //insert new page and history in to database
                else
                {
                    Page newPage = new Page
                    {
                        MaxResponseTime = page.ResponseTime,
                        MinResponseTime = page.ResponseTime,
                        Url = page.Url
                    };

                    newPage.History.Add(new History
                    {
                        Date = DateTime.Now,
                        ResponseTime = page.ResponseTime
                    });

                    resultStat.Add(new PageStat
                    {
                        PageUrl = newPage.Url,
                        MaxResponseTime = page.ResponseTime,
                        MinResponseTime = page.ResponseTime,
                        CurrentResponseTime = page.ResponseTime
                    });

                    host.Pages.Add(newPage);

                    _repository.UpdateHost(host);
                }
            }

            await _repository.SaveChangesAsync();
            return resultStat;
        }

        private MeasurementResult PrepareResult(IEnumerable<PageStat> pageMeasurement)
        {
            MeasurementResult result = new MeasurementResult();

            if (pageMeasurement != null)
            {
                result.Pages = pageMeasurement.OrderByDescending(x => x.CurrentResponseTime).ToList();
                result.AvgResponseTime = result.Pages.Average(x => x.CurrentResponseTime);
            }

            result.Success = result.Pages != null;
            result.HostUrl = result.Success ?
                _helper.GetHostFromUrl(result.Pages.First().PageUrl) : "";

            return result;
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _repository.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}