using System.Web.Mvc;
using Fanex.BetList.Accounting.Models;
using Fanex.Result.Core.LinkedGames;
using Sunplus.Accounting.Core.Cache;

namespace Fanex.BetList.Accounting.Controllers
{
    public class ThirdPartyController : Controller
    {

        public ThirdPartyController()
        {
        }

        public ActionResult LinkedGamesResult(int productId, string custId, string transId3rd)
        {
            var productName = CacheRef.GetProductName(productId);
            var service = new LinkedGamesResult();
            var result = service.GetResult(custId, transId3rd, isDesktop: true);

            return View("~/Views/BetList/ThirdParty/Result.cshtml", new ThirdPartyResultViewModel(productName, result));
        }
    }
}
