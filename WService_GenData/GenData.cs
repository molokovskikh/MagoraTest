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
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;

namespace WService_GenData
{
    public partial class GenData : ServiceBase
    {
        private static BlockingCollection<IFeedItem> _instance = null;
        private static readonly object syncRoot = new object();
        public static  BlockingCollection<IFeedItem> Instance
        {
            get
            {                
                if (_instance != null) return _instance;
                Monitor.Enter(syncRoot);
                Interlocked.Exchange<BlockingCollection<IFeedItem>>(ref _instance, new BlockingCollection<IFeedItem>());
                Monitor.Exit(syncRoot);
                return _instance;
            }
        }


        void EventLog_WriteEntry(string source, string message)
        {
           
                try
                {
                    EventLog.WriteEntry(source, message);
                }
                catch (Exception exc)
                {
                    try { EventLog.Clear(); }
                    catch { }

                    try
                    {
                        EventLog.WriteEntry(source, exc.Message);
                    }
                    catch
                    {
                    }

                }
            }


        /// <summary>
        /// Новая консоль для процесса
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        ManualResetEvent _console_signal_exit=null;        
        public GenData(ManualResetEvent signal_exit)
            : this()
        {
            _console_signal_exit = signal_exit;            
            AllocConsole();
        }
        

        public GenData()
        {
            InitializeComponent();
        }

        private CancellationTokenSource ts;

        //Для консольного запуска
        public void Start(string[] args)
        {
            OnStart(args);
        }

        //Запуск сервиса
        protected override void OnStart(string[] args)
        {
            const int PortionData=150;
            Task.Factory.StartNew(()=>            
            {
                   
            Func<string, string> f_replace = (input) => input.Replace("</div><div>", "\n")
                                                     .Replace("<div>", "")
                                                     .Replace("<br>","\n")
                                                     .Replace("</br>","\n")
                                                     .Replace("<br/>", "\n")
                                                     .Replace("&gt;", ">")
                                                     .Replace("&lt;", "<")
                                                     .Replace("&quot;","\"")
                                                     .Replace("&amp;","'");

            //Список RSS лент
            string[] rss_url = {
                               "http://news.yandex.ru/politics.rss",
                               "http://news.yandex.ru/hardware.rss",
                               "http://city-n.ru/rss",
                               "http://hh.ru/rss/searchvacancy.xml?itemsOnPage=1000&areaId=4&searchPeriod=60"
                               };           

          
           ts = ts ?? new CancellationTokenSource();
          
           
           //Задача получения данных из RSS-лент
           Action  fGen = ()=> 
           {                   
               HttpFeedFactory hff = new HttpFeedFactory();
               LinqFeedXmlParser parser = new LinqFeedXmlParser();
               Rss20Feed feed = new Rss20Feed();               
               while (true)
               {
                   //Ограничим заполнение коллекции (если этого не сделать память может кончится, к тому же столько данных нам не к чему)
                   if (GenData.Instance.Count > PortionData) continue;                   
                   if(ts.IsCancellationRequested) break;
                                      
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
                                        .Aggregate(0,(a,b)=>{ GenData.Instance.Add(b);return a;});
               }
           };

           
           //Запустим две задачи для заполнения из лент
           for (int i = 1; i < 2; i++)
               Task.Factory.StartNew(fGen);

           //Задача заполения текстом
           Task.Factory.StartNew(() =>
           {
               while(true)
               {
                   //Ограничим заполнение коллекции (если этого не сделать память может кончится, к тому же столько данных нам не к чему)
                   if (GenData.Instance.Count > PortionData) continue;
                   
                   GenData.Instance.Add(
                   new Rss20FeedItem
                   {
                       Content = "Заголовок случайных данных\nСлучайные данные\nКонцепция MVC позволяет разделить данные, представление и обработку действий пользователя на три отдельных компонента\nМодель (англ. Model). Модель предоставляет знания: данные и методы работы с этими данными, реагирует на запросы, изменяя своё состояние. Не содержит информации, как эти знания можно визуализировать.\nПредставление, вид (англ. View). Отвечает за отображение информации (визуализацию). Часто в качестве представления выступает форма (окно) с графическими элементами.\nКонтроллер (англ. Controller). Обеспечивает связь между пользователем и системой: контролирует ввод данных пользователем и использует модель и представление для реализации необходимой реакции."
                   }
                                    );
               }
           });

           //Задача добавления данных в базу
           Task tAddDb= Task.Factory.StartNew(() => 
           {
               int cnt = MagoraTest.Entity.MagoraRepository.Instance.Records.Count();
               while (true)
               {
                   if (ts.IsCancellationRequested) break;
                  
                   try
                   {
                       MagoraRepository.Instance.AddRange(
                       GenData.Instance.Skip(cnt).Take(PortionData).Select(s => new MagoraData() { Data = f_replace(s.Content) })
                       );
                   }
                   catch (SqlException)
                   {
                   }
                   catch(Exception exc)
                   {
                   }
                   if(GenData.Instance.Count>PortionData)
                   {
                       IFeedItem item=new Rss20FeedItem();
                       for(int i=0;i<PortionData;)
                       {
                           if(GenData.Instance.TryTake(out item))
                               i++;
                       }
                   }
                   
                   MagoraRepository.Instance.Save();
                   if (_console_signal_exit != null)
                   {                      
                       Console.Write("{0:dd.MM.yyyy hh:mm:ss}  -  Write portion data to DataBase\n",DateTime.Now);
                   }
                   Thread.Sleep(1000);
               }
           });

          
           //Ожидаем завершение задачи
           if(tAddDb.Status != TaskStatus.Faulted)   tAddDb.Wait();
           //Если консоль, тогда просигналим что пора основной поток
           if (_console_signal_exit != null) 
               _console_signal_exit.Set();
            });
           
        }
        //Запуск сервиса

        protected override void OnStop()
        {
            if (ts != null && !ts.IsCancellationRequested)
                ts.Cancel();
            base.OnStop();
        }
    }
}
