﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMagora.Filters;
using WebMagora.Models;

namespace WebMagora.Controllers
{
    [InitDatabase]
    public class MagoraDataController : Controller
    {
        //
        // GET: /MagoraData/
       
        public ActionResult Index(int ? give,int ? count)
        {
            //Если give>0 то это Ajax запрос (то частичный рендеринг)
            string viewName=count.HasValue ? "PartialIndex" : "Index";
            var data = MagoraTest.Entity.MagoraRepository.Instance.Records
                    .Skip(count.HasValue ? count.Value : 0)
                        .Take(give.HasValue ? give.Value : 150)
                        .Select(s => new MagoraData { Data = s.Data });
            if (count.HasValue)
            {
                ViewData["count"] = count.Value;
                return PartialView(viewName, data);
            }
            return View(viewName,data);
        }

        /// <summary>
        /// Обработчик картинок
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult Images(int id)
        {
            return File(string.Format(@"..\..\images\orderedList{0}.png", id), "image/png");
        }
    }
}
