using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Threading;
using System.Data.Linq.Mapping;
using MagoraTest.Interfaces;

namespace MagoraTest.Entity
{   

    partial class MagoraData
    {
        public string Title
        {
            get
            {
                return (Data ?? string.Empty).Split(new char[] { '\n' }).FirstOrDefault();
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
        }
        
        public IEnumerable<MagoraData> Records
        {            
            get 
            {                
                return _db.GetTable<MagoraData>();                
            }
        }

      

        public void Add(MagoraData i)
        {
            _db.MagoraDatas.Attach(i,true);
        }
        public void AddRange(IEnumerable<MagoraData> l)
        {
            _db.MagoraDatas.AttachAll(l,true);
        }
        public void Save()
        {
            _db.SubmitChanges();
        }
    }

    
}
