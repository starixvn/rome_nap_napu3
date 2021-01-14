namespace Sunplus.Accounting.Core.Cache
{
    using DNAide.Core;
    using Fanex.RemotingRef;

    /// <summary>
    /// Summary description for CacheRef
    /// </summary>
    public static class CacheRef
    {
        private static readonly IRemotingRefService remotingRefService = new RemotingRefService();

        public static string GetProductName(int productId)
            => remotingRefService.GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), "en-US", productId.ToString());

        public static string GetBettype(object bettypeID)
        {
            return remotingRefService.GetBetTypeName(bettypeID.ToString(), "en-US");
        }

        public static string GetSportName(object sportID)
        {
            return remotingRefService.GetSportName(sportID.ToString(), "en-US", string.Empty);
        }

        public static string GetTeamName(object teamID)
        {
            return remotingRefService.GetTeamName(teamID.ToString(), "en-US", string.Empty);
        }

        public static string GetLeagueName(object leagueID)
        {
            return remotingRefService.GetLeagueName(leagueID.ToString(), "en-US", string.Empty);
        }

        public static string GetHorseTeamName(object CodeID)
        {
            return remotingRefService.GetHorseTeamName(CastAide.AsString(CodeID), "en-US");
        }

        public static string GetOddsTypeName(object OddsType)
        {
            return remotingRefService.GetOddsTypeName(CastAide.AsString(OddsType), "en-US");
        }

        public static string GetCombinationName(string betteam)
        {
            string name = string.Empty;
            switch (betteam)
            {
                case "110":
                    name = "Trixie";
                    break;

                case "1110":
                    name = "Yankee";
                    break;

                case "11110":
                    name = "Canadian";
                    break;

                case "111110":
                    name = "Heinz";
                    break;

                case "1111110":
                    name = "Super Heinz";
                    break;

                case "11111110":
                    name = "Goliath";
                    break;
            }
            return name;
        }

        public static string GetLiveCasinoResult(int resultId)
        {
            string result = string.Empty;
            /*switch(resultId)
            {
                case 1:
                    result = "Banker";
                    break;

                case 2:
                    result = "Player";
                    break;

                case 3:
                    result = "Tie";
                    break;

                case 4:
                    result = "Big";
                    break;

                case 5:
                    result = "Small";
                    break;

                case 6:
                    result = "B.Pair";
                    break;

                case 7:
                    result = "P.Pair";
                    break;
            }*/
            return remotingRefService.GetCasinoBetItem(resultId.ToString(), "en-US");
        }

        public static string GetRouletteBetItem(int itemId)
        {
            return remotingRefService.GetRouletteBetItem(itemId.ToString(), "en-US");
        }

        public static string GetBettypeNameSportingSolution(object betypeId, string resourceId)
        {
            string bettypeName = "";
            bettypeName = remotingRefService.GetBetTypeName(betypeId.ToString(), "en-US", string.Empty, resourceId);
            return bettypeName;
        }
    }
}