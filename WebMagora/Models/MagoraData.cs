using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MagoraTest.Interfaces;

namespace WebMagora.Models
{
    public class MagoraData:IMagoraData
    {
        const string splitter_str ="\n"; //Разделитель строк
        const string splitter_html_string = "<br>"; //Разделитель строк в Display
        
        /// <summary>
        /// Данные
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Заголовок строки элемента списка
        /// </summary>
        public string Title
        {
            get
            {
                return (Data ?? string.Empty)
                        .Split(new string[] { splitter_str }, StringSplitOptions.RemoveEmptyEntries)
                            .FirstOrDefault();
            }
        }

        /// <summary>
        /// Проверка элемента списка на порядковый номер
        /// </summary>
        /// <param name="number">Порядковый номер элемента в результатах</param>
        /// <param name="o">порядковй номер для сверки</param>
        /// <returns></returns>
        private int calc(int number,int o)
        {
            int ost = -1;
            Math.DivRem(number, o, out ost);
            if(ost==0)//&&number==o)
                return o;
            return 0;
        }

        /// <summary>
        /// Возвращает номер картинки в зависимости от порядкового номера
        /// </summary>
        /// <param name="number">Порядковый номер в результатах</param>
        /// <returns></returns>
        public int ImageId(int number)
        {            
            int res = calc(number, 17);
            if (res > 0) 
                return 3;
            if ((res = calc(number, 11)) > 0) 
                return 2;
            if ((res = calc(number, 7)) > 0) 
                return 1;
            
            res = 4+new Random().Next(2);
            return res;
        }

        /// <summary>
        /// Высота (в строках) элемента списка
        /// </summary>
        /// <param name="number">Порядковый номер элемента в результатах</param>
        /// <returns></returns>
        public int RowSize(int number)
        {
            int res = calc(number,9);
            if (res > 0)
                return res;
            if ((res = calc(number, 7)) > 0)
                return res;
            return res;
        }

        /// <summary>
        /// Отображение строки с 2 по 5, в зависимости от позиции
        /// </summary>
        /// <param name="number">Порядковый номер элемента в результатах</param>
        /// <returns></returns>
        public string Display(int number)
        {
            int cs = 3;            
            switch(RowSize(number))
            {
                case 9:
                    cs=5;
                    break;
                case 7:
                    cs=4;
                    break;
            }
            
            string ffd= string.Format(
            Data.Split(new string[] { splitter_str }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1).Take(cs)
                .Aggregate(string.Empty, (f, e) =>
                    {                        
                        f+=e+(!string.IsNullOrEmpty(f)?"{0}":"");
                        return f;
                    }),
                   splitter_html_string );
            return ffd;
        }
    }
}