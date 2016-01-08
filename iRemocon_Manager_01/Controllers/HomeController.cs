using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Text.RegularExpressions;

using iRemocon_Manager_01.Models;

namespace iRemocon_Manager_01.Controllers {
    public class HomeController : Controller {

        TestDB_01Entities db = new TestDB_01Entities();

        public ActionResult Index() {

            string test = db.iRemocons.FirstOrDefault().Detail;

            ViewBag.Message = "";

            return View(db);
        }

        public ActionResult About() {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ShowCodes(int id) {

            ViewBag.Message = id.ToString();

            return View(db);
        }

        public ActionResult ChangeRemoconInfo(int id) {

            var target = db.iRemocons.Where(p => p.ID == id).Single();

            ChangeRemoconInfoModel CRmodel = new ChangeRemoconInfoModel();
            CRmodel.id = target.ID;
            CRmodel.IPAddress = target.IPAddress;
            CRmodel.Detail = target.Detail;

            return View(CRmodel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeRemoconInfo(ChangeRemoconInfoModel model) {

            var target = db.iRemocons.Where(p => p.ID == model.id).Single();

            if (ModelState.IsValid) {

                System.Net.IPAddress address;
                if (!System.Net.IPAddress.TryParse(model.IPAddress, out address)) {
                    ModelState.AddModelError("", "IPアドレスの書式が正しくありません。");
                    return View(model);
                }
                if (!Regex.IsMatch(model.IPAddress, @"[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}")) {
                    ModelState.AddModelError("", "IPアドレスの書式が正しくありません。");
                    return View(model);
                }

                foreach (var x in db.iRemocons) {
                    if (x.ID != model.id && x.IPAddress.Equals(model.IPAddress)) {
                        ModelState.AddModelError("", "既に登録済みのIPアドレスです。");
                        return View(model);
                    }
                }

                target.IPAddress = model.IPAddress;
                target.Detail = model.Detail;
                db.SaveChanges();

                return RedirectToAction("Index", "Home");

            }

            // ここで問題が発生した場合はフォームを再表示します
            return View(model);
        }

        public ActionResult RemoconCreate() {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RemoconCreate(RemoconCreateModel model) {

            if (ModelState.IsValid) {

                System.Net.IPAddress address;
                if (!System.Net.IPAddress.TryParse(model.IPAddress, out address)) {
                    ModelState.AddModelError("", "IPアドレスの書式が正しくありません。");
                    return View(model);
                }
                if (!Regex.IsMatch(model.IPAddress, @"[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}")) {
                    ModelState.AddModelError("", "IPアドレスの書式が正しくありません。");
                    return View(model);
                }

                foreach (var x in db.iRemocons) {
                    if (x.IPAddress.Equals(model.IPAddress)) {
                        ModelState.AddModelError("", "既に登録済みのIPアドレスです。");
                        return View(model);
                    }
                }

                iRemocon insert = new iRemocon();
                insert.IPAddress = model.IPAddress;
                insert.Detail = model.Detail;

                db.iRemocons.Add(insert);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");

            }

            // ここで問題が発生した場合はフォームを再表示します
            return View(model);
        }

        public ActionResult RegisterCode(string id) {

            RegisterCodeModel RCModel = new RegisterCodeModel();
            RCModel.IPAddress = id.Replace('_', '.');
            RCModel.Code = 1;
            RCModel.Detail = "必須項目です。";

            return View(RCModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCode(RegisterCodeModel model) {

            var target = db.iRemocons.Where(p => p.IPAddress.Equals(model.IPAddress)).Single();

            if (ModelState.IsValid) {

                if (model.IPAddress.Equals("192.168.10.80")) {
                    if (model.Code < 100 || 1500 < model.Code) {
                        ModelState.AddModelError("", "登録可能なコード番号は100～1500です。");
                        return View(model);
                    }
                } else {
                    if (model.Code < 1 || 1500 < model.Code) {
                        ModelState.AddModelError("", "登録可能なコード番号は1～1500です。");
                        return View(model);
                    }
                }
                
                foreach (var x in target.RegistrationCodes) {
                    if (model.Code == x.RegistrationCode1) {
                        ModelState.AddModelError("", "既に登録済みのコード番号です。");
                        return View(model);
                    }
                }

                Common common = new Common();
                string res;
                try {
                    res = common.ConnectRemocon(model.IPAddress, "*au\r\n");
                } catch {
                    ModelState.AddModelError("", "iRemocon("+model.IPAddress+") に接続できませんでした");
                    return View(model);
                }
                if (!res.Equals("ok")) {
                    ModelState.AddModelError("", "iRemocon(" + model.IPAddress + ") に接続できませんでした");
                    return View(model);
                }

                try {
                    res = common.ConnectRemocon(model.IPAddress, "*ic;"+ model.Code +"\r\n");
                } catch {
                    ModelState.AddModelError("", "iRemocon(" + model.IPAddress + ") に接続できませんでした");
                    return View(model);
                }
                if (!res.Equals("ic;ok")) {
                    ModelState.AddModelError("", "iRemocon(" + model.IPAddress + ") からのエラー： " + res);
                    return View(model);
                }

                RegistrationCode insert = new RegistrationCode();
                insert.RegistrationCode1 = model.Code;
                insert.Detail = model.Detail;
                target.RegistrationCodes.Add(insert);
                db.SaveChanges();

                return RedirectToAction("ShowCodes", "Home", new { id = target.ID });

            }

            // ここで問題が発生した場合はフォームを再表示します
            return View(model);
        }

        public ActionResult ChangeCodeInfo(string id, int code, string detail) {

            ChangeCodeModel CCModel = new ChangeCodeModel();
            CCModel.IPAddress = id.Replace('_', '.');
            CCModel.Code = code;
            CCModel.Detail = detail;
            CCModel.Check = true; 

            return View(CCModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeCodeInfo(ChangeCodeModel model) {

            var target = db.iRemocons.Where(p => p.IPAddress.Equals(model.IPAddress)).Single();
            var targetcode = target.RegistrationCodes.Where(p=>p.RegistrationCode1 == model.Code).Single();

            if (ModelState.IsValid) {

                if (!model.Check) {
                    Common common = new Common();
                    string res;
                    try {
                        res = common.ConnectRemocon(model.IPAddress, "*au\r\n");
                    } catch {
                        ModelState.AddModelError("", "iRemocon(" + model.IPAddress + ") に接続できませんでした");
                        return View(model);
                    }
                    if (!res.Equals("ok")) {
                        ModelState.AddModelError("", "iRemocon(" + model.IPAddress + ") に接続できませんでした");
                        return View(model);
                    }

                    try {
                        res = common.ConnectRemocon(model.IPAddress, "*ic;" + model.Code + "\r\n");
                    } catch {
                        ModelState.AddModelError("", "iRemocon(" + model.IPAddress + ") に接続できませんでした");
                        return View(model);
                    }
                    if (!res.Equals("ic;ok")) {
                        ModelState.AddModelError("", "iRemocon(" + model.IPAddress + ") からのエラー： " + res);
                        return View(model);
                    }
                }


                targetcode.Detail = model.Detail;
                db.SaveChanges();

                return RedirectToAction("ShowCodes", "Home", new { id = target.ID });

            }

            // ここで問題が発生した場合はフォームを再表示します
            return View(model);
        }

    }
}
