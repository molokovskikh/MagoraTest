using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QDFeedParser;
using QDFeedParser.Xml;
using MagoraTest.Entity;


namespace WService_GenData
{
    public partial class GenData : ServiceBase
    {
        private static List<IFeedItem> _instance = null;
        private static readonly object syncRoot = new object();
        public static List<IFeedItem> Instance
        {
            get
            {
                if (_instance != null) return _instance;
                Monitor.Enter(syncRoot);
                Interlocked.Exchange<List<IFeedItem>>(ref _instance, new List<IFeedItem>());
                Monitor.Exit(syncRoot);
                return _instance;
            }
        }
        

        public GenData()
        {
            InitializeComponent();
        }

        private CancellationTokenSource ts;

        public void Start(string[] args)
        {
            OnStart(args);
        }


        protected override void OnStart(string[] args)
        {
            string[] rss_url = {
                               "http://news.yandex.ru/politics.rss",
                               "http://news.yandex.ru/hardware.rss",
                               "http://city-n.ru/rss",
                               "http://hh.ru/rss/searchvacancy.xml?itemsOnPage=1000&areaId=4&searchPeriod=60"
                               };           

           TaskFactory factory = new TaskFactory();
           ts = ts ?? new CancellationTokenSource();
          
           
           Task tGen=factory.StartNew(() => 
           {                   
               HttpFeedFactory hff = new HttpFeedFactory();
               LinqFeedXmlParser parser = new LinqFeedXmlParser();
               Rss20Feed feed = new Rss20Feed();               
               while (true)
               {
                   if(ts.IsCancellationRequested) break;
                   GenData.Instance.AddRange(
                   rss_url.Aggregate(new List<string>(),
                       (f, x) =>
                       {
                           f.Add(hff.DownloadXml(new Uri(x)));
                           return f;
                       }
                                    ).Aggregate(new List<IFeedItem>(),
                                        (f, x) =>
                                        {
                                            parser.ParseFeed(feed, x);
                                            f.AddRange(feed.Items.ToArray<IFeedItem>());                                            
                                            return f;
                                        })
                                           );
               }
           });

           Task tAddDb= factory.StartNew(() => 
           {
               int cnt = MagoraTest.Entity.MagoraRepository.Instance.Records.Count();
               while (true)
               {
                   if (ts.IsCancellationRequested) break;
                   MagoraRepository.Instance.AddRange(
                   GenData.Instance.Skip(cnt).Take(150).Select(s => new MagoraData() { Data = s.Content })
                   );
                   GenData.Instance.RemoveRange(0, 150);
                   MagoraRepository.Instance.Save();

               }
           });


           //if (tGen.Status != TaskStatus.Running)  tGen.Start();
           //if (tAddDb.Status != TaskStatus.Running)  tAddDb.Start();
           tGen.Wait();
           tAddDb.Wait();
        }

        protected override void OnStop()
        {
            if (ts != null && !ts.IsCancellationRequested)
                ts.Cancel();
        }
    }
}
