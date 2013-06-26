using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using MagoraTest.Entity;

namespace MagoraTest.Interfaces
{      
    //Репозитарий
    public interface IMagoraRepository
    {
        IEnumerable<MagoraData> Records { get; }
        void Add(MagoraData i);
        void AddRange(IEnumerable<MagoraData> e);
        void Save();
    }
}
