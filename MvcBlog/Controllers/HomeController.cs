using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;


namespace MvcBlog.Controllers
{
    public class HomeController : Controller
    {
        mvcblogDB db = new mvcblogDB();

        // GET: Home
        public ActionResult Index()
        {
            var makale = db.Makales.OrderByDescending(m => m.MakaleId).ToList();
            return View(makale);
        }

        public ActionResult MakaleDetay(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if(makale==null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        public ActionResult YorumYap(string yorum,int MakaleId)
        {
            var uyeid = Session["uyeid"];
            if (yorum ==null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            db.Yorums.Add(new Yorum { UyeId = Convert.ToInt32(uyeid), MakaleId = MakaleId, Icerik = yorum, Tarih = DateTime.Now });
            db.SaveChanges();
           
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Hakkimizda()
        {
            return View();
        }

        public ActionResult Iletisim()
        {
            return View();
        }

        public ActionResult KategoriPartial()
        {
            return View(db.Kategoris.ToList());
        }
      
    }
}