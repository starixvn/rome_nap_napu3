using System.Web.Mvc;
using Fanex.BetList.Accounting.Models;
using Fanex.Result.Core.LinkedGames;
using Fanex.Result.Core.ThirdParty;
using Fanex.Result.Core.ThirdParty.Models;
using Sunplus.Accounting.Core.Cache;

namespace Fanex.BetList.Accounting.Controllers
{
    public class ThirdPartyController : Controller
    {

        public ThirdPartyController()
        {
        }

        public ActionResult LinkedGamesResult(int productId, int custId, string transId3rd, string gameId3rd, int currencyId, string siteId)
        {
            var productName = CacheRef.GetProductName(productId);
            var service = new ThirdPartyResult("LinkedGamesResultConfigs");
            var resultRequest = new ResultRequest(custId, transId3rd, gameId3rd, currencyId, siteId);
            var result = service.GetResultGET(resultRequest);

            return View("~/Views/BetList/ThirdParty/Result.cshtml", new ThirdPartyResultViewModel(productName, result));
        }
    }
}
