using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Fanex.BetList.Accounting.Models;
using Fanex.BetList.Core.Entities;

namespace Fanex.BetList.Accounting.Repositories
{
    public static class TicketDetailTransformer
    {
        public static IList<TicketDetail> Transform(DataTable dataTable, int customerId)
        {
            IList<TicketDetail> tickets = new List<TicketDetail>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                DataColumnCollection dataColumns = dataTable.Columns;
                IDictionary<string, PropertyInfo> ticketMapper = DataColumnTicketPropertyMapper;
                IDictionary<string, PropertyInfo> ticketDetailMapper = DataColumnTicketDetailPropertyMapper;

                foreach (DataRow drow in dataTable.Rows)
                {
                    tickets.Add(GetTicketDetail(dataColumns, ticketMapper, ticketDetailMapper, drow, customerId));
                }
            }

            return tickets;
        }

        private static TicketDetail GetTicketDetail(
            DataColumnCollection dataColumns,
            IDictionary<string, PropertyInfo> ticketMapper,
            IDictionary<string, PropertyInfo> ticketDetailMapper,
            DataRow drow,
            int customerId)
        {
            ITicket ticket = new Ticket { CustId = customerId };
            TicketDetail ticketDetail = new TicketDetail { Ticket = ticket };

            string lowerColumnName;
            PropertyInfo propertyInfo;
            object dbValue;
            object propertyValue;
            Type underlyingType;

            foreach (DataColumn column in dataColumns)
            {
                lowerColumnName = column.ColumnName.ToLowerInvariant();
                dbValue = drow[lowerColumnName];

                if (ticketMapper.TryGetValue(lowerColumnName, out propertyInfo))
                {
                    if (dbValue == DBNull.Value)
                    {
                        propertyValue = null;
                    }
                    else
                    {
                        underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);

                        if (underlyingType == null)
                        {
                            underlyingType = propertyInfo.PropertyType;
                        }

                        propertyValue = Convert.ChangeType(dbValue, underlyingType);
                    }

                    propertyInfo.SetValue(ticket, propertyValue, null);

                }
                else if (ticketDetailMapper.TryGetValue(lowerColumnName, out propertyInfo))
                {
                    propertyValue = dbValue == DBNull.Value ?
                        null :
                        Convert.ChangeType(dbValue, propertyInfo.PropertyType);

                    propertyInfo.SetValue(ticketDetail, propertyValue, null);
                }
            }

            return ticketDetail;
        }

        private static IDictionary<string, PropertyInfo> DataColumnTicketPropertyMapper
        {
            get
            {
                Type ticketType = typeof(ITicket);
                BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;

                return new Dictionary<string, PropertyInfo>
                {
                    { "actual_stake", ticketType.GetProperty("ActualStake", flag) },
                    { "awinlost", ticketType.GetProperty("AgentWinlost", flag) },
                    { "apositiontaking", ticketType.GetProperty("AgentPositionTaking", flag) },
                    { "comm", ticketType.GetProperty("Comm", flag) },
                    { "discount", ticketType.GetProperty("AgentDiscount", flag) },

                    { "awayid", ticketType.GetProperty("AwayId", flag) },
                    { "betid", ticketType.GetProperty("BetId", flag) },
                    { "betteam", ticketType.GetProperty("BetTeam", flag) },
                    { "bettype", ticketType.GetProperty("BetTypeId", flag) },
                    { "comstatus", ticketType.GetProperty("CommStatus", flag) },
                    { "custid", ticketType.GetProperty("CustId", flag) },
                    { "username", ticketType.GetProperty("UserName", flag) },
                    { "hdp1", ticketType.GetProperty("Handicap1", flag) },
                    { "hdp2", ticketType.GetProperty("Handicap2", flag) },
                    { "homeid", ticketType.GetProperty("HomeId", flag) },
                    { "eventdate", ticketType.GetProperty("EventDate", flag) },
                    { "ip", ticketType.GetProperty("IP", flag) },
                    { "liveindicator", ticketType.GetProperty("IsLive", flag) },
                    { "isneutral", ticketType.GetProperty("IsNeutral", flag) },
                    { "leagueid", ticketType.GetProperty("LeagueId", flag) },
                    { "livehomescore", ticketType.GetProperty("LiveHomeScore", flag) },
                    { "liveawayscore", ticketType.GetProperty("LiveAwayScore", flag) },

                    { "mwinlost", ticketType.GetProperty("MasterWinlost", flag) },
                    { "mpositiontaking", ticketType.GetProperty("MasterPositionTaking", flag) },
                    { "acomm", ticketType.GetProperty("AgentComm", flag) },
                    { "adiscount", ticketType.GetProperty("MasterDiscount", flag) },

                    { "modds", ticketType.GetProperty("MasterOdds", flag) },
                    { "matchcode", ticketType.GetProperty("MatchCode", flag) },
                    { "matchid", ticketType.GetProperty("MatchId", flag) },
                    { "odds", ticketType.GetProperty("Odds", flag) },
                    { "oddstype", ticketType.GetProperty("OddsType", flag) },
                    { "commission", ticketType.GetProperty("PlayerComm", flag) },
                    { "race", ticketType.GetProperty("Race", flag) },
                    { "refno", ticketType.GetProperty("RefNo", flag) },
                    { "showtime", ticketType.GetProperty("ShowTime", flag) },
                    { "sporttype", ticketType.GetProperty("SportTypeId", flag) },
                    { "stake", ticketType.GetProperty("Stake", flag) },
                    { "status", ticketType.GetProperty("Status", flag) },

                    { "swinlost", ticketType.GetProperty("SuperWinlost", flag) },
                    { "spositiontaking", ticketType.GetProperty("SuperPositionTaking", flag) },
                    { "scomm", ticketType.GetProperty("SuperComm", flag) },
                    { "sdiscount", ticketType.GetProperty("SuperDiscount", flag) },

                    { "transdate", ticketType.GetProperty("TransDate", flag) },
                    { "transid", ticketType.GetProperty("TransId", flag) },
                    { "winlost", ticketType.GetProperty("Winlost", flag) },
                    { "winlostdate", ticketType.GetProperty("WinlostDate", flag) },
                    { "transdesc", ticketType.GetProperty("TransDesc", flag) },

                    { "betcheck", ticketType.GetProperty("BetCheck", flag) },
                    { "oddsspread", ticketType.GetProperty("OddsSpread", flag) },

                    { "originalbettype", ticketType.GetProperty("OriginalBetType", flag) },
                    { "betdaqid", ticketType.GetProperty("BetdaqId", flag) },
                    { "extrainfo", ticketType.GetProperty("ExtraInfo", flag) },

                    { "currency", ticketType.GetProperty("Currency", flag) }
                };
            }
        }

        private static IDictionary<string, PropertyInfo> DataColumnTicketDetailPropertyMapper
        {
            get
            {
                Type ticketType = typeof(TicketDetail);
                BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;

                return new Dictionary<string, PropertyInfo>
                {
                    { "isshowotexrate", ticketType.GetProperty("IsShowOtExRate", flag) },
                    { "virtualrate", ticketType.GetProperty("VirtualRate", flag) },
                    { "apositiontaking2", ticketType.GetProperty("AgentPositionTaking2", flag) },
                    { "mpositiontaking2", ticketType.GetProperty("MasterPositionTaking2", flag) },
                    { "spositiontaking2", ticketType.GetProperty("SuperPositionTaking2", flag) },
                    { "betfrom", ticketType.GetProperty("BetFrom", flag) },
                    { "allowdelete", ticketType.GetProperty("AllowDelete", flag) },

                    { "creator", ticketType.GetProperty("Creator", flag) },
                    { "finalhomescore", ticketType.GetProperty("FinalHomeScore", flag) },
                    { "finalawayscore", ticketType.GetProperty("FinalAwayScore", flag) },
                    { "hthomescore", ticketType.GetProperty("HTHomeScore", flag) },
                    { "htawayscore", ticketType.GetProperty("HTAwayScore", flag) }
                };
            }
        }
    }
}
