using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using QDFeedParser;
using QDFeedParser.Xml;

namespace WService_GenData
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            string[] rss_url = {
                               "http://news.yandex.ru/politics.rss",
                               "http://news.yandex.ru/hardware.rss",
                               "http://city-n.ru/rss",
                               "http://hh.ru/rss/searchvacancy.xml?itemsOnPage=1000&areaId=4&searchPeriod=60"
                               };
            HttpFeedFactory hff = new HttpFeedFactory();
            hff.BeginDownloadXml(
            string t =  hff.DownloadXml(new Uri(rss_url[1]));
            
            Rss20Feed feed = new Rss20Feed();
            LinqFeedXmlParser parser = new LinqFeedXmlParser();
            parser.ParseFeed(feed, t);
            return;
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    new GenData() 
            //};
            //ServiceBase.Run(ServicesToRun);
        }
    }
}
