using System.Xml.Serialization;

namespace UkadTask.Domain.Sitemap
{
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Urlset
    {
        [XmlElement("url")]
        public Url[] urlset;
    }
}
