using System.Xml.Serialization;

namespace UkadTask.Domain.Sitemap
{
    [XmlType("url")]
    public class Url
    {
        [XmlElement("loc")]
        public string loc;
    }
}
