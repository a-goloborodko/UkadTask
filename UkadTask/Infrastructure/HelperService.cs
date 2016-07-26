using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using UkadTask.Domain.Sitemap;

namespace UkadTask.Infrastructure
{
    public class HelperService
    {
        public string GetHostFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("Url can't be null");

            try
            {
                Uri uri = new Uri(url);
                return uri.Scheme + "://" + uri.Host;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<string>> GetSitemapAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("Url can't be null");

            string host = GetHostFromUrl(url);
            string sitemapUrl = string.Format("{0}/sitemap.xml", host);

            Urlset reply = new Urlset();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(sitemapUrl))
            using (HttpContent content = response.Content)
            {
                if (!response.IsSuccessStatusCode)  //if sitemap not exist
                    return null;

                using (Stream stream = await content.ReadAsStreamAsync())
                {
                    try
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(Urlset));
                        reply = (Urlset)ser.Deserialize(stream);
                    }
                    catch (InvalidOperationException)   //can't desirialize sitemap
                    {
                        return null;
                    }
                }
            }
            return reply.urlset.Select(x => x.loc).ToList();
        }

        public async Task<bool> IsWebsiteExixtAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            bool isWell = Uri.IsWellFormedUriString(url, UriKind.Absolute);
            if (!isWell)
                return false;

            bool isExist = false;
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(url), Method = HttpMethod.Head })
                {
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        isExist = response.IsSuccessStatusCode;
                    }
                }

                return isExist;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}