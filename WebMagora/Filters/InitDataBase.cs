using MagoraTest.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;


namespace WebMagora.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitDatabaseAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        private static  DatabaseInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }
      

        private class DatabaseInitializer
        {
            public DatabaseInitializer()
            {                
                    using (var context = new DbContext("DefaultConnection"))
                    {
                        
                        if(context.Database.CreateIfNotExists())
                        {
                            context.Database.ExecuteSqlCommand(
string.Format(
@"
ALTER DATABASE {0} COLLATE SQL_Latin1_General_CP1251_CI_AS
GO
CREATE TABLE [dbo].[MagoraData] 
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Data] varchar(max) COLLATE SQL_Latin1_General_CP1251_CI_AS NOT NULL 
)",context.Database.Connection.Database)
                            );
                        }
                        context.SaveChanges();
                    }                    
                
            }
        }
    }
}