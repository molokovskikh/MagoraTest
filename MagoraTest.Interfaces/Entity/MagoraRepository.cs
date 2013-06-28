using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Threading;
using System.Data.Linq.Mapping;
using MagoraTest.Interfaces;
using System.ComponentModel;

namespace MagoraTest.Entity
{   
    
    public partial class MagoraData : INotifyPropertyChanging, INotifyPropertyChanged,IMagoraData
    {
        string IMagoraData.Title
        {
            get
            {
                return (Data ?? string.Empty).Split(new char[] { '\n' }).FirstOrDefault();
            }
        }
        string IMagoraData.Data
        {
            get
            {
                return this.Data;
            }
            set
            {
                this.Data = value;
            }
        }

    }
    
    public class MagoraRepository: IMagoraRepository
    {
        private static IMagoraRepository _instance = null;
        private static readonly object syncRoot = new object();
        public static IMagoraRepository Instance
        {
            get
            {
                if (_instance != null) return _instance;
                Monitor.Enter(syncRoot);
                Interlocked.Exchange<IMagoraRepository>(ref _instance, new MagoraRepository());
                Monitor.Exit(syncRoot);
                return _instance;
            }
        }


        private MagoraDataContext _db;
        public MagoraRepository()       
        {
             _db = new MagoraDataContext();
             Init();
        }

        void Init()
        {
            if (_db == null) return;

            if (!_db.DatabaseExists())
            {
                _db.CreateDatabase();
                _db.ExecuteCommand(string.Format("ALTER DATABASE {0} COLLATE SQL_Latin1_General_CP1251_CI_AS",_db.Connection.Database.ToString()));
            }
         
        }
        
        public IEnumerable<IMagoraData> Records
        {            
            get 
            {                
                return _db.GetTable<MagoraData>();                
            }
        }

      

        public void Add(IMagoraData i)
        {                                   
            lock (i)
            {                
                Monitor.Enter(_db.MagoraDatas);
                _db.MagoraDatas.InsertOnSubmit(i as MagoraData);
                Monitor.Exit(_db.MagoraDatas);
            }
        }
        public void AddRange(IEnumerable<IMagoraData> l)
        {
            lock (l)
            {
                Monitor.Enter(_db.MagoraDatas);
                _db.MagoraDatas.InsertAllOnSubmit(l as IEnumerable<MagoraData>);
                Monitor.Exit(_db.MagoraDatas);
            }                        
        }
        public void Save()
        {            
            _db.SubmitChanges();
        }
    }

    
}
