using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using MagoraTest.Entity;

namespace MagoraTest.Interfaces
{
    //Модель данных
    public interface IMagoraData
    {
        string Title { get; }
        string Data { get; set; }
    }
    //Репозитарий
    public interface IMagoraRepository
    {
        IEnumerable<IMagoraData> Records { get; }
        void Add(IMagoraData i);
        void AddRange(IEnumerable<IMagoraData> e);
        void Save();
    }
}
