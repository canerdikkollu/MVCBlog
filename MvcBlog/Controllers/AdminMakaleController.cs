using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;
using System.Web.Helpers;
using System.IO;

namespace MvcBlog.Controllers
{
    public class AdminMakaleController : Controller
    {
        mvcblogDB db = new mvcblogDB();
       
        // GET: AdminMakale
        public ActionResult Index()
        {
            var makales = db.Makales.ToList();
            return View(makales);
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi");
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale makale, string etiketler, HttpPostedFileBase Foto)
        {
            //kaydete tıklandıktan sonra yapılacak işlemler
            if (ModelState.IsValid)
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoInfo = new FileInfo(Foto.FileName);

                    string newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newFoto);
                    makale.Foto = "/Uploads/MakaleFoto/" + newFoto;

                }
                if (etiketler != null)
                {
                    string[] etiketDizi = etiketler.Split(',');
                    foreach (var i in etiketDizi)
                    {
                        var yeniEtiket = new Etiket { EtiketAdi = i };
                        db.Etikets.Add(yeniEtiket);
                        makale.Etikets.Add(yeniEtiket);
                    }
                }
                makale.UyeId = Convert.ToInt32(Session["uyeid"]);
                db.Makales.Add(makale);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(makale);

        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale == null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi", makale.KategoriId);

            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, HttpPostedFileBase Foto, Makale makale)
        {
            try
            {
                var guncellenecekMakale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                string oldfilePath = guncellenecekMakale.Foto;

                if (Foto != null && Foto.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(Foto.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("/Uploads/MakaleFoto/"), fileName);
                    Foto.SaveAs(path);

                    guncellenecekMakale.Foto = "/Uploads/MakaleFoto/" + Foto.FileName;
                    string fullPath = Request.MapPath("~" + oldfilePath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                guncellenecekMakale.Baslik = makale.Baslik;
                guncellenecekMakale.Icerik = makale.Icerik;
                guncellenecekMakale.Tarih = makale.Tarih;
             
                guncellenecekMakale.KategoriId = makale.KategoriId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale == null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var silinecekMakale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                if (silinecekMakale == null)
                {
                    return HttpNotFound();
                }

                if (System.IO.File.Exists(Server.MapPath(silinecekMakale.Foto)))
                {
                    System.IO.File.Delete(Server.MapPath(silinecekMakale.Foto));
                }
                foreach (var i in silinecekMakale.Yorums.ToList())
                {
                    db.Yorums.Remove(i);
                }
                foreach (var i in silinecekMakale.Etikets.ToList())
                {
                    db.Etikets.Remove(i);
                }
                db.Makales.Remove(silinecekMakale);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
