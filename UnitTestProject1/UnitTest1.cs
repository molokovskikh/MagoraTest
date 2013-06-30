using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Collections.Generic;
using MagoraTest.Interfaces;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AddGetDelete_Repository_Magora()
        {
            MagoraTest.Interfaces.IMagoraRepository mr = MagoraTest.Entity.MagoraRepository.Instance;
            MagoraTest.Interfaces.IMagoraData d = new MagoraTest.Entity.MagoraData() { Data = "Test" };
            mr.Add(d);
            mr.Save();
            Assert.AreEqual(d.Data,mr.Records.LastOrDefault().Data);           
        }


        [TestMethod]
        public void Index_MagoraController0_0_WebMagora()
        {
            var controller = new WebMagora.Controllers.MagoraDataController();
            int? g=null,c=null;            
            var result = controller.Index(g, c);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(((ViewResult)result).Model, typeof(IEnumerable<IMagoraData>));
        }

        [TestMethod]
        public void Index_MagoraController10_0_WebMagora()
        {
            var controller = new WebMagora.Controllers.MagoraDataController();
            int g = 10, c = 0;
            var result = controller.Index(g,c);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.IsInstanceOfType(((PartialViewResult)result).Model,typeof(IEnumerable<IMagoraData>));
        }
        
        
        [TestMethod]
        public void Index_MagoraController10_10_WebMagora()
        {
            var controller = new WebMagora.Controllers.MagoraDataController();
            int g = 10, c = 10;
            var result = controller.Index(g, c);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.IsInstanceOfType(((PartialViewResult)result).Model, typeof(IEnumerable<IMagoraData>));
        }
    }
}
