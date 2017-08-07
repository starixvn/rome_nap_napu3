namespace Fanex.Tarus.BetList.Business.Results
{
    using Criteria.Results;
    using DataAccess.Results;
    using Result.Core;
    using Result.Core.Base;

    public interface ILucky3PopupResultService
    {
        string GetResult(int matchId);
    }

    public class Lucky3PopupResultService : ILucky3PopupResultService
    {
        private readonly ILucky3PopupResultDataAccess _lucky3PopupResultDataAccess;

        public Lucky3PopupResultService(ILucky3PopupResultDataAccess lucky3PopupResultDataAccess = null)
        {
            _lucky3PopupResultDataAccess = lucky3PopupResultDataAccess ?? new Lucky3PopupResultDataAccess();
        }

        public string GetResult(int matchId)
        {
            var criteria = new GetNumberGamePopupResultCriteria()
            {
                MatchID = matchId
            };

            return ResultEngine.HtmlParser(_lucky3PopupResultDataAccess.GetLucky3PopupResult(criteria), GetFanexResultSetting());
        }

        private ResultSetting GetFanexResultSetting()
        {
            return new ResultSetting
            {
                SportType = SportType.Lucky3,
                IsOutright = false,
                IsParlayPopUp = false,
                IsPopUp = true,
                VisiblePopUpTitle = false,
                LanguageCode = "en-US",
                VisibleMatchCodeIDColumn = false,
                VisibleFreeHeader = true,
                VisibleDetailedLink = false,
                HighlightWinnerTeam = true
            };
        }
    }
}