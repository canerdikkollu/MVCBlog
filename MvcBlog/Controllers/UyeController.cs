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
    public class UyeController : Controller
    {
        mvcblogDB db = new mvcblogDB();

        // GET: Uye
        public ActionResult Index(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.UyeId)
            {/*farklı üye bilgilerini görüntülemeyi engellemek için*/
                return HttpNotFound();
            }
            return View(uye);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Uye uye /*,string Sifre*/)
        {
            //var md5pass = Crypto.Hash(Sifre, "MD5");
            try
            {
                var login = db.Uyes.Where(u => u.KullaniciAdi == uye.KullaniciAdi).SingleOrDefault();
                if (login.KullaniciAdi == uye.KullaniciAdi && login.Email == uye.Email && login.Sifre == uye.Sifre)
                {
                    Session["uyeid"] = login.UyeId;
                    Session["kullaniciadi"] = login.KullaniciAdi;
                    Session["yetkiid"] = login.YetkiId;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Uyari = "Giriş İşlemi Başarısız Bilgilerinizi Kontrol Ederek Tekrar Deneyin!";
                    return View();
                }

            }
            catch (Exception e)
            {
                ViewBag.Uyari = "Giriş İşlemi Başarısız Bilgilerinizi Kontrol Ederek Tekrar Deneyin!";
                return View();
            }


        }

        public ActionResult LogOut()
        {
            Session["uyeid"] = null;
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Uye uye, HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoInfo = new FileInfo(Foto.FileName);

                    string newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFoto/" + newFoto);
                    uye.Foto = "/Uploads/UyeFoto/" + newFoto;
                    uye.YetkiId = 2;
                    db.Uyes.Add(uye);
                    db.SaveChanges();
                    Session["uyeid"] = uye.UyeId;
                    Session["Kullaniciadi"] = uye.KullaniciAdi;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Fotoğraf", "Fotoğraf Seçiniz!");
                }
            }
            return View(uye);
        }

        public ActionResult Edit(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.UyeId)
            {
                return HttpNotFound();
            }
            return View(uye);
        }

        [HttpPost]
        public ActionResult Edit(Uye uye, int id, HttpPostedFileBase Foto)
        {
            try
            {
                var guncelUye = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
                string oldfilePath = guncelUye.Foto;

                if (Foto != null && Foto.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(Foto.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("/Uploads/UyeFoto/"), fileName);
                    Foto.SaveAs(path);

                    guncelUye.Foto = "/Uploads/UyeFoto/" + Foto.FileName;
                    string fullPath = Request.MapPath("~" + oldfilePath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                guncelUye.AdSoyad = uye.AdSoyad;
                guncelUye.KullaniciAdi = uye.KullaniciAdi;
                guncelUye.Sifre = uye.Sifre;
                guncelUye.Email = uye.Email;
                db.SaveChanges();
                Session["kullaniciadi"] = uye.KullaniciAdi;
                return RedirectToAction("Index", "Home", new { id = guncelUye.UyeId });

            }
            catch
            {
                return View();
            }
        }
    }
}



/*
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
                guncellenecekMakale.İcerik = makale.İcerik;
                guncellenecekMakale.Tarih = makale.Tarih;
             
                guncellenecekMakale.KategoriId = makale.KategoriId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
     */
