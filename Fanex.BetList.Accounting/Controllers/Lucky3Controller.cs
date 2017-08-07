using System.Web.Mvc;
using Fanex.BetList.Accounting.Repositories;
using Fanex.BetListDetail.Lucky3.Criteria;
using Fanex.BetListDetail.Lucky3.Services;

namespace Fanex.BetList.Accounting.Controllers
{
    public class Lucky3Controller : Controller
    {
        private readonly ILucky3Service _lucky3Service;

        public Lucky3Controller()
            : this(null)
        {
        }

        public Lucky3Controller(ILucky3Service lucky3Service)
        {
            _lucky3Service = lucky3Service ?? new Lucky3Service(new Lucky3DataAccess());
        }

        public ActionResult ViewDetail(GetLucky3DetailParameter criteria)
        {
            criteria.LevelId = 4;
            var result = _lucky3Service.GetBetListDetail(criteria);

            return View(result);
        }
    }
}
