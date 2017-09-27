using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Fanex.BetList.Core.Entities;

namespace Fanex.BetList.Accounting.Repositories
{
    public static class TicketDataTransformer
    {
        public static List<ITicketData> Transform(DataTable dataTable)
        {
            List<ITicketData> ticketData = new List<ITicketData>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                DataColumnCollection dataColumns = dataTable.Columns;
                IDictionary<string, PropertyInfo> mapper = DataColumnTicketDataPropertyMapper;

                foreach (DataRow drow in dataTable.Rows)
                {
                    ticketData.Add(GetITicketData(dataColumns, mapper, drow));
                }
            }

            return ticketData;
        }

        private static ITicketData GetITicketData(
            DataColumnCollection dataColumns,
            IDictionary<string, PropertyInfo> mapper,
            DataRow drow)
        {
            ITicketData ticket = new TicketData();

            string lowerColumnName;
            PropertyInfo propertyInfo;
            object dbValue;
            object propertyValue;

            foreach (DataColumn column in dataColumns)
            {
                lowerColumnName = column.ColumnName.ToLowerInvariant();

                if (mapper.TryGetValue(lowerColumnName, out propertyInfo))
                {
                    dbValue = drow[lowerColumnName];
                    propertyValue = dbValue == DBNull.Value ?
                        null :
                        Convert.ChangeType(dbValue, propertyInfo.PropertyType);

                    propertyInfo.SetValue(ticket, propertyValue, null);
                }
            }

            return ticket;
        }

        private static IDictionary<string, PropertyInfo> DataColumnTicketDataPropertyMapper
        {
            get
            {
                Type ticketType = typeof(ITicketData);
                BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;

                return new Dictionary<string, PropertyInfo>
                {
                    { "refno", ticketType.GetProperty("RefNo", flag) },
                    { "bettype", ticketType.GetProperty("BetTypeId", flag) },
                    { "status", ticketType.GetProperty("Status", flag) },
                    { "odds", ticketType.GetProperty("Odds", flag) },
                    { "stake", ticketType.GetProperty("Stake", flag) },
                    { "betteam", ticketType.GetProperty("BetTeam", flag) },
                    { "spositiontaking", ticketType.GetProperty("SuperPT", flag) },
                    { "mpositiontaking", ticketType.GetProperty("MasterPT", flag) },
                    { "apositiontaking", ticketType.GetProperty("AgentPT", flag) },
                    { "transdesc", ticketType.GetProperty("TransDesc", flag) }
                };
            }
        }
    }
}
