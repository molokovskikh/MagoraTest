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
using System.Data.SqlClient;


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

          
           ts = ts ?? new CancellationTokenSource();
          
           
           
           Action  fGen = ()=> 
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
                           try
                           {
                               f.Add(hff.DownloadXml(new Uri(x)));
                           }
                           catch(QDFeedParser.MissingFeedException)
                           {
                           }
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
           };

           
           for(int i=1;i<20;i++)           
               Task.Factory.StartNew(fGen);

           Task.Factory.StartNew(() =>
           {
               GenData.Instance.Add(
                   new Rss20FeedItem
                   {
                       Content = "Заголовок случайных данных\nСлучайные данные\nКонцепция MVC позволяет разделить данные, представление и обработку действий пользователя на три отдельных компонента\nМодель (англ. Model). Модель предоставляет знания: данные и методы работы с этими данными, реагирует на запросы, изменяя своё состояние. Не содержит информации, как эти знания можно визуализировать.\nПредставление, вид (англ. View). Отвечает за отображение информации (визуализацию). Часто в качестве представления выступает форма (окно) с графическими элементами.\nКонтроллер (англ. Controller). Обеспечивает связь между пользователем и системой: контролирует ввод данных пользователем и использует модель и представление для реализации необходимой реакции."
                   }
                                    );
           });

           Task tAddDb= Task.Factory.StartNew(() => 
           {
               int cnt = MagoraTest.Entity.MagoraRepository.Instance.Records.Count();
               while (true)
               {
                   if (ts.IsCancellationRequested) break;
                   try
                   {
                       MagoraRepository.Instance.AddRange(
                       GenData.Instance.Skip(cnt).Take(150).Select(s => new MagoraData() { Data = s.Content })
                       );
                   }
                   catch (SqlException)
                   {
                   }
                   if(GenData.Instance.Count>150)
                    GenData.Instance.RemoveRange(0, 150);
                   
                   MagoraRepository.Instance.Save();
                   Thread.Sleep(1000);
               }
           });

          
           //if(tGen.Status != TaskStatus.Faulted)   tGen.Wait();
           if(tAddDb.Status != TaskStatus.Faulted)   tAddDb.Wait();
        }

        protected override void OnStop()
        {
            if (ts != null && !ts.IsCancellationRequested)
                ts.Cancel();
        }
    }
}
