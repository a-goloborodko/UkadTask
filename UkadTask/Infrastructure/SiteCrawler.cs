using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UkadTask.Domain;

namespace UkadTask.Infrastructure
{
    public class SiteCrawler
    {
        public int MaxPageToCrawling { get; set; }

        public SiteCrawler()
        {
            MaxPageToCrawling = 100;
        }

        public SiteCrawler(int maxPageToCrawling)
        {
            if (maxPageToCrawling <= 0)
                throw new ArgumentException("Value must be more than 0");

            MaxPageToCrawling = maxPageToCrawling;
        }

        public virtual async Task<IEnumerable<CrawledPage>> CrawlAsync(string startUrl)
        {
            ConcurrentDictionary<string, CrawledPage> crawledPages = new ConcurrentDictionary<string, CrawledPage>();
            string host = new HelperService().GetHostFromUrl(startUrl);

            await CrawlPage(crawledPages, host, CorrectUrl(startUrl));
            return crawledPages.Select(x => x.Value).ToList();
        }

        public async virtual Task<IEnumerable<CrawledPage>> CrawlAsync(IList<string> urls)
        {
            if (urls == null || urls.Count() == 0)
                throw new ArgumentException("urls can't be null or does not contain elements");

            ConcurrentDictionary<string, CrawledPage> crawledPages = new ConcurrentDictionary<string, CrawledPage>();

            string[] urlsArray;

            //check MaxPageToCrowling setting
            if (urls.Count() > MaxPageToCrawling)
            {
                urlsArray = urls.Take(MaxPageToCrawling).ToArray();
            }
            else
            {
                urlsArray = urls.ToArray();
            }

            var tasks = urlsArray.Select(async url =>
            {
                await MeasurePageResponseTime(crawledPages, url);
            });

            await Task.WhenAll(tasks.ToArray());

            return crawledPages.Select(x => x.Value).ToList();
        }

        private async Task CrawlPage(ConcurrentDictionary<string, CrawledPage> crawledPages, string host, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("url can't be null");

            if (crawledPages.ContainsKey(url) || crawledPages.Count == MaxPageToCrawling)  //if it page has been already crawled
                return;

            List<string> pagesToCrawl = new List<string>(); //list of parsed links
            string htmlResult = ""; //result of HttpClient
            Regex regexLink = new Regex("(?<=<a\\s*?href=(?:'|\"))[^'\"]*?(?=(?:'|\"))");   //reg to find links on html page
            MatchCollection matches;
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    stopwatch.Stop();

                    //if http status not 200 - stop executing method
                    if (!response.IsSuccessStatusCode)
                        return;

                    htmlResult = await content.ReadAsStringAsync();
                    matches = regexLink.Matches(htmlResult);
                }
            }
            catch (HttpRequestException)  //raises when has been pased nonexistent url -> angular ui url or something else
            {
                return; //nothing to do when url not exist
            }

            bool isAdded = crawledPages.TryAdd(url, new CrawledPage { Url = url, ResponseTime = (int)stopwatch.ElapsedMilliseconds });

            if (!isAdded || matches.Count == 0) //if page already crawled or hasn't links
                return;

            //get parsed links
            string link = string.Empty;
            foreach (Match match in matches)
            {
                link = match.ToString();
                if (string.IsNullOrWhiteSpace(link))
                    continue;

                if (!link.StartsWith("http") && !link.StartsWith("www") && !link.StartsWith(host))   //transform absolute path to absolure url
                {
                    link = host + link;
                }
                if (link.StartsWith(host))  //get only internal links
                {
                    link = CorrectUrl(link);

                    if (!crawledPages.ContainsKey(link) && !pagesToCrawl.Contains(link))
                    {
                        pagesToCrawl.Add(link);
                    }
                }
            }

            if (pagesToCrawl.Count == 0)    //if html page hasn't internal links
                return;

            //check MaxPageToCrawling
            int length = pagesToCrawl.Count;
            if (pagesToCrawl.Count + crawledPages.Count > MaxPageToCrawling)
            {
                length = MaxPageToCrawling - crawledPages.Count;
            }

            for (int i = 0; i < length; i++)
            {
                await CrawlPage(crawledPages, host, pagesToCrawl[i]);
            }
            return;
        }

        private async Task MeasurePageResponseTime(ConcurrentDictionary<string, CrawledPage> crawledPages, string url)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    stopwatch.Stop();
                    if (response.IsSuccessStatusCode)
                        crawledPages.TryAdd(url, new CrawledPage { Url = url, ResponseTime = (int)stopwatch.ElapsedMilliseconds });
                }
            }
            catch (HttpRequestException)
            {
                throw; //nothing to do when url not exist
            }
        }

        private string CorrectUrl(string url)
        {
            string lastSymbol = url.Substring(url.Length - 1, 1);
            if (lastSymbol == "/" || lastSymbol == "#") //delete '/' or '#' from the end of url if it exist
                url = url.Remove(url.Length - 1);

            return url;
        }
    }
}