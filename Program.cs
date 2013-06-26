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
            /*
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new GenData() 
            };
            ServiceBase.Run(ServicesToRun);
          */
            new GenData().Start(null);
        }
    }
}
