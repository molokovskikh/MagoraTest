using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using QDFeedParser;
using QDFeedParser.Xml;
using System.ServiceProcess;
using System.Collections;
using System.Diagnostics;
using System.Configuration.Install;
using System.Threading;

namespace WService_GenData
{
    static class Program
    {
        
    private static bool InstallService()
        {
            bool res = true;
            // установить
            using (ProjectInstaller pi = new ProjectInstaller())
            {
                IDictionary  savedState = new Hashtable();
                
                try
                {                   
                    pi.Context = new InstallContext();                            
                    pi.Context.Parameters.Add("assemblypath", Process.GetCurrentProcess().MainModule.FileName);
                    foreach (Installer i in pi.Installers)                        
                        i.Context = pi.Context;                  
                    pi.Install(savedState);
                    pi.Commit(savedState);                    
                }
                catch (Exception ex)
                {
                    try
                    {
                        System.Diagnostics.EventLog.WriteEntry(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ": InstallService()", ex.ToString());
                    }
                    catch
                    { }
                    pi.Rollback(savedState);
                    res = false;
                }
            }
            return res;
        }

    

        private static void StartService(string ServiceName)
        {
            ServiceController ctrl_service;
            try
            {
                ctrl_service = new ServiceController(ServiceName);                
                ctrl_service.Start();
                ctrl_service.WaitForStatus(ServiceControllerStatus.Running);
                ctrl_service.Close();
            }
            catch (Exception ex)
            {
                try
                {
                System.Diagnostics.EventLog.WriteEntry(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ": StartService()", ex.ToString());
                }
                catch
                { }
            }
        }

        private static void StopService(string ServiceName)
        {
            ServiceController ctrl_service;
            try
            {
                ctrl_service = new ServiceController(ServiceName);
                ctrl_service.Stop();
                ctrl_service.WaitForStatus(ServiceControllerStatus.Stopped);
                ctrl_service.Close();
            }
            catch (Exception ex)
            {
                try
                {
                System.Diagnostics.EventLog.WriteEntry(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ": StopService()", ex.ToString());
                }
                catch
                { }
            }
        }

        private static void RemoveService()
        {
            // удалить
            using (ProjectInstaller pi = new ProjectInstaller())
            {
                
                try
                {
                    pi.Context = new InstallContext();
                    pi.Context.Parameters.Add("assemblypath", Process.GetCurrentProcess().MainModule.FileName);
                    foreach (Installer i in pi.Installers)
                        i.Context = pi.Context;
                    pi.Uninstall(null);
                }
                catch (Exception ex)
                {
                    try
                    {
                    System.Diagnostics.EventLog.WriteEntry(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ": RemoveService()", ex.ToString());
                    }
                    catch
                    { }
                   // Console.WriteLine(ex.Message);
                }
            }
        }
           
        static void Main(string[] args)
        {
            
            try
            {
                System.Diagnostics.EventLog.WriteEntry(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ": " , "Перед запуском");
            }
            catch
            { }
           
            if (args != null)
            {
                if (args.Length > 0)
                {
                    if (args[0].ToUpper().Equals("INSTALL"))
                    {
                        try
                        {
                            if (InstallService())
                                StartService(new GenData().ServiceName);
                        }
                        catch (Exception exc)
                        {
                            try
                            {
                                System.Diagnostics.EventLog.WriteEntry(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ": " + args[0].ToUpper(), exc.ToString());
                            }
                            catch
                            { }
                        }
                        return;
                    }

                    if (args[0].ToUpper().Equals("START"))
                    {
                        try
                        {                            
                           StartService(new GenData().ServiceName);
                        }
                        catch (Exception exc)
                        {
                            try
                            {
                            System.Diagnostics.EventLog.WriteEntry(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ": " + args[0].ToUpper(), exc.ToString());
                            }
                            catch
                            { }
                        }
                        return;
                    }

                    if (args[0].ToUpper().Equals("STOP"))
                    {
                        try
                        {
                            StopService(new GenData().ServiceName);
                        }
                        catch (Exception exc)
                        {
                            try
                            {
                                System.Diagnostics.EventLog.WriteEntry(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ": " + args[0].ToUpper(), exc.ToString());
                            }
                            catch
                            { }
                        }
                        return;
                    }

                    if (args[0].ToUpper().Equals("UNINSTALL"))
                    {
                        try
                        {
                            RemoveService();
                        }
                        catch(Exception exc)
                        {
                            try
                            {
                            System.Diagnostics.EventLog.WriteEntry(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ": " + args[0].ToUpper(), exc.ToString());
                            }
                            catch
                            { }
                        }
                        return;
                    }

                    if (args[0].ToUpper().Equals("CONSOLE"))
                    {
                        ManualResetEvent exit_signal = new ManualResetEvent(false);                          
                        new GenData(exit_signal).Start(null);
                        exit_signal.WaitOne();
                        return;
                    }
                }
            }


            try
            {
                System.Diagnostics.EventLog.WriteEntry(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ": " ,"fsdfds");
            }
            catch
            { }
            ServiceBase[] ServicesToRun = new ServiceBase[] { new GenData() };
            ServiceBase.Run(ServicesToRun);           
        }
    }
}
