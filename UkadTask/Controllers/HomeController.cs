using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UkadTask.Domain;
using UkadTask.Infrastructure;
using UkadTask.Models.Response;

namespace UkadTask.Controllers
{
    public class HomeController : Controller
    {
        private MainService _service;
        private HelperService _helper;

        public HomeController()
        {
            _service = new MainService();
            _helper = new HelperService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> MeasureSite(string url)
        {
            MeasurementResult result = await _service.MeasureSiteAsync(url);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult History(string url)
        {
            string hostUrl = _helper.GetHostFromUrl(url);
            HistoryResponseModel result;
            if (hostUrl != null)
            {
                result = _service.GetHistory(hostUrl);
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(HistoryResponseModel.CreateFailureHistoryModel(url, "History is empty"), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (_service != null)
                _service.Dispose();

            base.Dispose(disposing);
        }
    }
}