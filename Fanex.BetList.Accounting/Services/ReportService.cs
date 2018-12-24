using System;
using Fanex.BetList.Accounting.Constants;
using Fanex.BetList.Accounting.Core;
using Fanex.BetList.Accounting.Models;
using Fanex.Data;
using Fanex.Tarus.Infrastructure.Caching;

namespace Fanex.BetList.Accounting.Services
{
    public static class ReportService
    {
        private const string BalanceUpCacheKey = "Fanex.BetList.Accounting.Services.BalanceUp";

        public static BalanceUp BalanceUp
        {
            get
            {
                BalanceUp balanceUp = CacheHelper.Get<BalanceUp>(BalanceUpCacheKey);

                if (balanceUp == null)
                {
                    try
                    {
                        using (ObjectDb db = new ObjectDb("Core_ReportService_GetBalanceUpDay"))
                        {
                            balanceUp = db.QueryEntity<BalanceUp>();
                        }

                        if (balanceUp != null)
                        {
                            CacheHelper.Set(BalanceUpCacheKey, balanceUp, DateTime.Now.AddMinutes(5));
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggingHandler.Log(ex);
                    }
                }

                return balanceUp;
            }
        }

        public static bool IsVirtualSportBetType(int betTypeId)
        {
            return BetTypeContants.MinVirtualSportBetType <= betTypeId &&
                betTypeId <= BetTypeContants.MaxVirtualSportBetType;
        }

        public static bool IsGoldDeluxe(int betTypeId)
        {
            return betTypeId == BetTypeContants.GoldDeluxeBaccarat ||
                betTypeId == BetTypeContants.GoldDeluxeParlayBaccarat ||
                betTypeId == BetTypeContants.GoldDeluxeRoulette;
        }

        public static bool IsMicroGaming(int betTypeId)
        {
            return BetTypeContants.MicroGamingBaccarat <= betTypeId &&
                betTypeId <= BetTypeContants.MicroGamingEndBetType;
        }

        public static bool IsAGCasino(int betTypeId)
        {
            return 1801 <= betTypeId && betTypeId <= 1804;
        }

        public static bool IsRNGGame_RNGCasino(int betTypeId)
        {
            return 2101 <= betTypeId && betTypeId <= 2199;
        }

        public static bool IsMiniGame_RNGCasino(int betTypeId)
        {
            return 2201 <= betTypeId && betTypeId <= 2299;
        }

        public static bool IsMobile_RNGCasino(int betTypeId)
        {
            return 2301 <= betTypeId && betTypeId <= 2399;
        }

        public static bool IsAllBet(int betTypeId)
        {
            return 2401 <= betTypeId && betTypeId <= 2499;
        }

        public static bool IsVoidbridge(int betTypeId)
        {
            return 4001 <= betTypeId && betTypeId <= 7999;
        }

        public static bool IsNG2NeedSuffix(int betTypeId)
        {
            return 2501 <= betTypeId && betTypeId <= 2699;
        }

        public static int GetNG2SportType(int betTypeId)
        {
            if (2501 <= betTypeId && betTypeId <= 2539)
            {
                return 213;
            }

            if (2541 <= betTypeId && betTypeId <= 2579)
            {
                return 214;
            }

            if (2581 <= betTypeId && betTypeId <= 2619)
            {
                return 215;
            }

            if (2621 <= betTypeId && betTypeId <= 2659)
            {
                return 216;
            }

            if (2661 <= betTypeId && betTypeId <= 2679)
            {
                return 217;
            }

            if (2681 <= betTypeId && betTypeId <= 2699)
            {
                return 218;
            }

            return 0;
        }
        public static bool IsHappy5Bettype(int bettypeId)
            => bettypeId >= 8101 && bettypeId <= 8105;
    }
}