using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using Fanex.BetList.Accounting.Constants;
using Fanex.BetList.Accounting.Core;
using Fanex.BetList.Accounting.Criteria;
using Fanex.BetList.Accounting.Extensions;
using Fanex.BetList.Accounting.Models;
using Fanex.BetList.Accounting.Repositories;
using Fanex.BetList.Accounting.Resources;
using Fanex.BetList.Age.BetListExcelBuilderExt;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.Factories;
using Fanex.BetList.Core.TicketHelper;
using NPOI.SS.UserModel;
using Sunplus.Accounting.Core.Common;
using Sunplus.Accounting.Core.Cache;

namespace Fanex.BetList.Accounting.Services
{
    public class BetListService : IBetListService
    {
        private readonly IBetListRepository _betListRepository;

        public BetListService(IBetListRepository betListRepository = null)
        {
            _betListRepository = betListRepository ?? new BetListRepository();
        }

        public IList<TicketDetail> GetBetListByCustInfo_Refno(string transactionId, int customerId)
        {
            var balanceUp = ReportService.BalanceUp;

            var criteria = new BetListByCustInfoByRefnoCriteria()
            {
                TransId = transactionId,
                CustId = customerId,
                MoveBetsDay = balanceUp.MoveBetsDay,
                BalanceUpDay = balanceUp.BalanceUpDay
            };

            var criteriaForU3 = new BetListByCustInfoByRefnoU3Criteria()
            {
                TransId = transactionId,
                CustId = customerId
            };

            try
            {
                IList<TicketDetail> betDetails = new List<TicketDetail>();
                List<ITicketData> ticketData = new List<ITicketData>();

                bool isLimitForU3 = (AppSession.IsU3Account && !AppSession.IsU3SpecialAccount)
                    || (AppSession.IsU3SpecialAccount && !IsU3SpecialCustomers(customerId));

                if (isLimitForU3)
                {
                    _betListRepository.GetBetListData(criteriaForU3, customerId, ref betDetails, ref ticketData);
                }
                else
                {
                    _betListRepository.GetBetListData(criteria, customerId, ref betDetails, ref ticketData);
                }

                if (betDetails != null)
                {
                    ITicketHelper ticketHelper = new TicketHelper();
                    BetListHTMLFactory factory = new BetListHTMLFactory();
                    BetListHTMLBuilder builder = factory.CreateBuilder();
                    GetCachePropertyById funcGetOddsTypeName = CacheRef.GetOddsTypeName;

                    foreach (TicketDetail betDetail in betDetails)
                    {
                        ticketHelper.IsShowOddsWithChoice = IsShowOddsWithChoice(betDetail.Ticket.BetTypeId);

                        betDetail.TransDateHtml = builder.BuildTrans(betDetail.Ticket);
                        betDetail.ChoiceHtml = builder.BuildChoice(betDetail.Ticket, ticketHelper, ticketData, false);
                        betDetail.StakeHtml = betDetail.BuildStake(ref builder);
                        betDetail.OddHtml = builder.BuildOdds(betDetail.Ticket, ticketData, funcGetOddsTypeName);
                        betDetail.StatusHtml = BetListHelper.BuildStatus(betDetail.Ticket, ticketHelper, ticketData, factory, builder);
                    }

                    return betDetails;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<TicketDetail>();
        }

        public IList<TicketDetail> GetBetListByCustInfo_DateRange(
            int custId,
            DateTime fromDate,
            DateTime toDate,
            int betType,
            int pageSize,
            int pageIndex,
            ref int quantity,
            bool isCache,
            bool isLimitForU3 = false)
        {
            try
            {
                List<ITicketData> outputLiveCasino = new List<ITicketData>();
                List<ITicketData> outputLiveCasino_Old = new List<ITicketData>();
                List<ITicketData> outputLiveCasino_Merge;

                IList<TicketDetail> betDetails = new List<TicketDetail>();
                IList<TicketDetail> betDetailsOld = new List<TicketDetail>();
                IList<TicketDetail> betDetailsMerge = SessionManager.Get<IList<TicketDetail>>(BetListConstants.BetListByCustomerInfoSessionKey);

                if (!isCache || betDetailsMerge == null || betDetailsMerge.Count == 0)
                {
                    var balanceUp = ReportService.BalanceUp;

                    var betListByCustInfoCriteria = new BetListByCustInfoCriteria
                    {
                        CustId = custId,
                        FromDate = fromDate,
                        ToDate = toDate,
                        BetType = betType,
                        MoveBetsDay = balanceUp.MoveBetsDay,
                    };

                    var betListByCustInfoU3Criteria = new BetListByCustInfoU3Criteria
                    {
                        CustId = custId,
                        FromDate = fromDate,
                        ToDate = toDate,
                        BetType = betType,
                        MoveBetsDay = balanceUp.MoveBetsDay,
                    };

                    var betListByCustInfoCriteria_Old = new BetListByCustInfoCriteria_Old
                    {
                        CustId = custId,
                        FromDate = fromDate,
                        ToDate = toDate,
                        BetType = betType,
                    };

                    var betListByCustInfoU3Criteria_Old = new BetListByCustInfoU3Criteria_Old
                    {
                        CustId = custId,
                        FromDate = fromDate,
                        ToDate = toDate,
                        BetType = betType,
                    };

                    if (toDate >= balanceUp.BalanceUpDay)
                    {
                        if (isLimitForU3)
                        {
                            _betListRepository.GetBetListData(betListByCustInfoU3Criteria, custId, ref betDetails, ref outputLiveCasino);
                        }
                        else
                        {
                            _betListRepository.GetBetListData(betListByCustInfoCriteria, custId, ref betDetails, ref outputLiveCasino);
                        }
                    }

                    if (fromDate < balanceUp.BalanceUpDay)
                    {
                        if (isLimitForU3)
                        {
                            _betListRepository.GetBetListData(betListByCustInfoU3Criteria_Old, custId, ref betDetailsOld, ref outputLiveCasino_Old);
                        }
                        else
                        {
                            _betListRepository.GetBetListData(betListByCustInfoCriteria_Old, custId, ref betDetailsOld, ref outputLiveCasino_Old);
                        }
                    }

                    betDetailsMerge = betDetails
                        .Concat(betDetailsOld)
                        .OrderBy(x => x.Ticket.TransDate).ToList();
                    outputLiveCasino_Merge = outputLiveCasino.Concat(outputLiveCasino_Old).ToList();

                    SessionManager.Set(BetListConstants.BetListByCustomerInfoSessionKey, betDetailsMerge);
                    SessionManager.Set(BetListConstants.BetListOutputLiveCasinoSessionKey, outputLiveCasino_Merge);
                }
                else
                {
                    outputLiveCasino_Merge = SessionManager.Get<List<ITicketData>>(BetListConstants.BetListOutputLiveCasinoSessionKey);
                }

                quantity = betDetailsMerge.Count;

                if (pageIndex > 0 && betDetailsMerge != null)
                {
                    IList<TicketDetail> betDetailsMergeModel = new List<TicketDetail>();

                    ITicketHelper ticketHelper = new TicketHelper();
                    BetListHTMLFactory factory = new BetListHTMLFactory();
                    BetListHTMLBuilder builder = factory.CreateBuilder();
                    GetCachePropertyById funcGetOddsTypeName = CacheRef.GetOddsTypeName;

                    int from = pageSize * (pageIndex - 1);
                    int to = pageSize * pageIndex;

                    for (int i = from; i < betDetailsMerge.Count && i < to; i++)
                    {
                        betDetailsMergeModel.Add(betDetailsMerge[i]);
                    }

                    foreach (TicketDetail betDetail in betDetailsMergeModel)
                    {
                        ticketHelper.IsShowOddsWithChoice = IsShowOddsWithChoice(betDetail.Ticket.BetTypeId);

                        betDetail.TransDateHtml = builder.BuildTrans(betDetail.Ticket);
                        betDetail.ChoiceHtml = builder.BuildChoice(betDetail.Ticket, ticketHelper, outputLiveCasino_Merge, false);
                        betDetail.StakeHtml = betDetail.BuildStake(ref builder);
                        betDetail.OddHtml = builder.BuildOdds(betDetail.Ticket, outputLiveCasino, funcGetOddsTypeName);
                        betDetail.StatusHtml = BetListHelper.BuildStatus(betDetail.Ticket, ticketHelper, outputLiveCasino_Merge, factory, builder);
                    }

                    return betDetailsMergeModel;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<TicketDetail>();
        }

        public IList<BetType> GetBetTypeByDate(BetTypeByDateCriteria criteria)
        {
            bool isLimitForU3 = (AppSession.IsU3Account && !AppSession.IsU3SpecialAccount)
                || (AppSession.IsU3SpecialAccount && !this.IsU3SpecialCustomers(criteria.CustId));

            if (isLimitForU3)
            {
                criteria.SetSettingKeyForU3();
            }

            IList<BetType> bettypes = new List<BetType>
            {
                new BetType(0, Labels.All)
            };

            var dataTable = _betListRepository.GetBetTypeByDate(criteria);

            int betTypeIdTmp;
            string betTypeNameTmp;

            foreach (DataRow row in dataTable.Rows)
            {
                betTypeIdTmp = Convert.ToInt32(row["typeid"]);
                betTypeNameTmp = row["typenamee"].ToString();
                betTypeNameTmp += GetSportNameSuffixByBetType(betTypeIdTmp);

                bettypes.Add(new BetType
                {
                    BetTypeID = betTypeIdTmp,
                    BetTypeName = betTypeNameTmp
                });
            }

            return bettypes;
        }

        public int GetCustIdByUserName(string userName)
        {
            try
            {
                return _betListRepository.GetCustIdByUserName(
                    new CustIDByUsernameCriteria
                    {
                        CustomerName = userName
                    });
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return 0;
        }

        public IList<TicketDetail> GetBetListStatement(
            int custId,
            int transType,
            DateTime fromDate,
            ref int quality,
            ref decimal totalComm,
            ref decimal totalWinLoss)
        {
            try
            {
                List<ITicketData> outputLiveCasino = new List<ITicketData>();
                IList<TicketDetail> betDetails = new List<TicketDetail>();

                var criteria = new BetListStatement
                {
                    CustId = custId,
                    TransType = transType,
                    FromDate = fromDate,
                    MoveBetsDay = ReportService.BalanceUp.MoveBetsDay
                };

                _betListRepository.GetBetListData(criteria, custId, ref betDetails, ref outputLiveCasino);

                SessionManager.Set(BetListConstants.BetListByStatementSessionKey, betDetails);
                SessionManager.Set(BetListConstants.BetListByStatementOutputLiveCasinoSessionKey, outputLiveCasino);

                ITicketHelper ticketHelper = new TicketHelper();
                BetListHTMLFactory factory = new BetListHTMLFactory();
                BetListHTMLBuilder builder = factory.CreateBuilder();
                GetCachePropertyById funcGetOddsTypeName = CacheRef.GetOddsTypeName;
                string status;
                ITicketData ticketDataTmp;

                foreach (TicketDetail betDetail in betDetails)
                {
                    ticketHelper.IsShowOddsWithChoice = IsShowOddsWithChoice(betDetail.Ticket.BetTypeId);

                    betDetail.TransDateHtml = builder.BuildTrans(betDetail.Ticket);
                    betDetail.ChoiceHtml = builder.BuildChoice(betDetail.Ticket, ticketHelper, outputLiveCasino, false);
                    betDetail.StakeHtml = betDetail.BuildStake(ref builder);
                    betDetail.OddHtml = builder.BuildOdds(betDetail.Ticket, outputLiveCasino, funcGetOddsTypeName);
                    betDetail.StatusHtml = BetListHelper.BuildStatus(betDetail.Ticket, ticketHelper, outputLiveCasino, factory, builder);

                    status = betDetail.Ticket.Status.ToLowerInvariant();

                    if (status != "running" && status != "waiting" && status != "void" && status != "reject")
                    {
                        totalComm += betDetail.Ticket.PlayerComm;
                        totalWinLoss += betDetail.Ticket.Winlost;
                    }

                    ticketDataTmp = outputLiveCasino.FirstOrDefault(data => data.RefNo == betDetail.Ticket.RefNo);

                    if (ticketDataTmp != null)
                    {
                        betDetail.Ticket.AgentPositionTaking = ticketDataTmp.AgentPT;
                        betDetail.Ticket.MasterPositionTaking = ticketDataTmp.MasterPT;
                        betDetail.Ticket.SuperPositionTaking = ticketDataTmp.SuperPT;
                    }
                }

                return betDetails;
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<TicketDetail>();
        }

        public MemoryStream ExportExcelBetListStatement(
            int custId,
            int transType,
            DateTime fromDate,
            ref string fileName)
        {
            try
            {
                IList<TicketDetail> betDetails = SessionManager.Get<IList<TicketDetail>>(BetListConstants.BetListByStatementSessionKey);
                List<ITicketData>  outputLiveCasino = SessionManager.Get<List<ITicketData>>(BetListConstants.BetListByStatementOutputLiveCasinoSessionKey);

                if (betDetails == null || betDetails.Count == 0)
                {
                    int quantity = 0;
                    decimal totalComm = 0;
                    decimal totalWinLost = 0;

                    betDetails = GetBetListStatement(custId, transType, fromDate, ref quantity, ref totalComm, ref totalWinLost);
                    outputLiveCasino = SessionManager.Get<List<ITicketData>>(BetListConstants.BetListByStatementOutputLiveCasinoSessionKey);
                }

                if (betDetails != null && betDetails.Count > 0)
                {
                    ITicketHelper ticketHelper = new TicketHelper();
                    GetCachePropertyById funcGetOddsTypeName = CacheRef.GetOddsTypeName;
                    BetListExcelFactory factory = new BetListExcelFactory();
                    BetListExcelBuilder builder = factory.CreateBuilder();

                    string reportTitle = "Daily Statement - " + fromDate.ToShortDateString() + " - " + betDetails[0].Ticket.UserName;
                    fileName = "DailyStatement-" + fromDate.ToShortDateString() + "-" + betDetails[0].Ticket.UserName + ".xls";
                    int index = 0;

                    builder.BuildWorksheet("BetList", 8);
                    builder.AddCaption(reportTitle, 8);
                    builder.AddHeaderStatement(true, false, false, false, true);

                    IList<ITicket> tickets = new List<ITicket>();

                    foreach (var betdetail in betDetails)
                    {
                        index++;
                        ticketHelper.Index = index;
                        builder.AddNo(index);

                        builder.AddTrans(betdetail.Ticket, ticketHelper);

                        builder.AddChoice(betdetail.Ticket, ticketHelper, outputLiveCasino)
                               .AddOdds(betdetail.Ticket, outputLiveCasino, funcGetOddsTypeName)
                               .AddStake(betdetail.Ticket);

                        builder.AddWinLoss(betdetail.Ticket);

                        builder.AddStatus(betdetail, ticketHelper);

                        builder.AddCreator(betdetail, ticketHelper);
                        tickets.Add(betdetail.Ticket);
                    }

                    if (tickets.Count > 0)
                    {
                        builder.AddFooter(tickets, false, 8);
                    }

                    MemoryStream ms = new MemoryStream();
                    IWorkbook resultWorkbook = builder.GetBetList();
                    resultWorkbook.Write(ms);

                    return ms;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return null;
        }

        private bool IsU3SpecialCustomers(int custId)
        {
            string[] specialCustomersU3 = ConfigurationManager.AppSettings["U3SpecialCustomers"].Split(',');
            string customerId = Convert.ToString(custId);

            for (int i = 0, max = specialCustomersU3.Length; i < max; i++)
            {
                if (customerId == specialCustomersU3[i].Trim())
                {
                    return true;
                }
            }

            return false;
        }

        private string GetSportNameSuffixByBetType(int betTypeId)
        {
            var sportName = string.Empty;

            if (ReportService.IsVirtualSportBetType(betTypeId))
            {
                sportName = Labels.Virtual_Sports;
            }
            else if (ReportService.IsGoldDeluxe(betTypeId))
            {
                sportName = Labels.Gold_Deluxe;
            }
            else if (ReportService.IsMicroGaming(betTypeId))
            {
                sportName = Labels.Microgaming;
            }
            else if (ReportService.IsAGCasino(betTypeId))
            {
                sportName = Labels.AGCasino;
            }
            else if (betTypeId == BetTypeContants.OverUnder ||
                betTypeId == BetTypeContants.OddEven)
            {
                sportName += Labels.Number_Game;
            }
            else if (ReportService.IsRNGGame_RNGCasino(betTypeId))
            {
                sportName += CacheRef.GetSportName(SportContants.RNG_RNGGame);
            }
            else if (ReportService.IsMiniGame_RNGCasino(betTypeId))
            {
                sportName += CacheRef.GetSportName(SportContants.RNG_MiniGame);
            }
            else if (ReportService.IsMobile_RNGCasino(betTypeId))
            {
                sportName += CacheRef.GetSportName(SportContants.RNG_Mobile);
            }
            else if (ReportService.IsAllBet(betTypeId))
            {
                sportName += CacheRef.GetSportName(SportContants.AllBet);
            }
            else if (ReportService.IsVoidbridge(betTypeId))
            {
                sportName += CacheRef.GetSportName(SportContants.Voidbridge);
            }
            else if (ReportService.IsNG2NeedSuffix(betTypeId))
            {
                sportName += CacheRef.GetSportName(ReportService.GetNG2SportType(betTypeId));
            }

            else if (ReportService.IsHappy5Bettype(betTypeId))
            {
                sportName = CacheRef.GetSportName(SportContants.Happy5);
            }

            return string.IsNullOrEmpty(sportName) ? string.Empty : " (" + sportName + ")";
        }

        private bool IsShowOddsWithChoice(int betTypeId)
        {
            return betTypeId == BetTypeContants.MixParlay || betTypeId == BetTypeContants.SystemParlay;
        }
    }
}
