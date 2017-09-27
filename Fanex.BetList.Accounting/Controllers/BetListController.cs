using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Fanex.BetList.Accounting.Core;
using Fanex.BetList.Accounting.Criteria;
using Fanex.BetList.Accounting.Models;
using Fanex.BetList.Accounting.Services;
using Fanex.BetDetails.Lucky3.Criteria;
using Fanex.BetDetails.Lucky3.Services;
using SmartPunterFilter = Fanex.AuthenticationFilter.AuthenticateSmartPunterAction;

namespace Fanex.BetList.Accounting.Controllers
{
    public class BetListController : Controller
    {
        private readonly IBetListService _betListService;
        private readonly ILucky3Service _lucky3Service;

        public BetListController()
        {
            _betListService = new BetListService();
            _lucky3Service = new Lucky3Service(null);
        }

        [SmartPunterFilter(SmartPunterFilter.AuthenticationType.Read)]
        public ActionResult GetBetListByCustInfo_Refno(string transactionId, int customerId)
        {
            IList<TicketDetail> result = new List<TicketDetail>();
            if (BetListHelper.IsNumeric(transactionId.Trim()))
            {
                result = _betListService.GetBetListByCustInfo_Refno(transactionId, customerId);
            }

            Hashtable paramHastable = new Hashtable();
            paramHastable.Add("custId", customerId);

            TempData["paramHastable"] = paramHastable;

            ViewBag.Quantity = 1;
            ViewBag.NumberBegining = 1;
            return PartialView("~/Views/BetList/BetlistByCustInfo/Betlist/_BetList.cshtml", result);
        }

        [SmartPunterFilter(SmartPunterFilter.AuthenticationType.Read/*, SmartPunterActionId.ViewBetListFromStatement*/)]
        public ActionResult GetBetListStatement(string username, int transType, DateTime fromDate)
        {
            Hashtable paramHastable = new Hashtable();
            paramHastable.Add("username", username);
            TempData["paramHastable"] = paramHastable;

            decimal totalComm = 0;
            decimal totalWinLost = 0;
            int quantity = 0;
            int customerId = _betListService.GetCustIdByUserName(username);
            var result = _betListService.GetBetListStatement(customerId, transType, fromDate, ref quantity, ref totalComm, ref totalWinLost);
            ViewBag.Quantity = quantity;
            ViewBag.TotalComm = totalComm;
            ViewBag.NumberBegining = 1;
            ViewBag.TotalWinLos = totalWinLost;

            ViewBag.CustID = customerId;
            ViewBag.TransType = transType;
            ViewBag.FromDate = fromDate.ToShortDateString();
            ViewBag.CustName = username;

            return PartialView("~/Views/BetList/BetListStatement/_BetListStatement.cshtml", result);
        }

        [HttpGet]
        public ActionResult ExportBetlistStatement(string username, int transType, DateTime fromDate)
        {
            string filename = string.Empty;
            int customerId = _betListService.GetCustIdByUserName(username);
            var ms = _betListService.ExportExcelBetListStatement(customerId, transType, fromDate, ref filename);

            if (ms != null)
            {
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return File(ms, @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", filename);
            }

            return new EmptyResult();
        }

        [HttpPost]
        public JsonResult GetBetTypes(BetTypeByDateCriteria betTypeByDateCriteria)
        {
            var result = _betListService.GetBetTypeByDate(betTypeByDateCriteria);

            return Json(result);
        }

        public ActionResult GetLucky3Detail(int custId, long transId)
        {
            var criteria = new GetLucky3DetailParameter
            {
                CustomerId = custId,
                TransactionId = transId
            };

            var viewModel = _lucky3Service.GetBetListDetail(criteria);

            return View("~/Views/Lucky3/ViewDetail.cshtml", viewModel);
        }
    }
}