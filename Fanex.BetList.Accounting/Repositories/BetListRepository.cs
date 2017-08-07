using System;
using System.Collections.Generic;
using System.Data;
using Fanex.BetList.Accounting.Models;
using Fanex.BetList.Core.Entities;
using Fanex.Data;
using Fanex.NAP.Framework.Data;

namespace Fanex.BetList.Accounting.Repositories
{
    public class BetListRepository : IBetListRepository
    {
        public void GetBetListData(
            ICriteria criteria,
            int custId,
            ref IList<TicketDetail> tickets,
            ref List<ITicketData> ticketData)
        {
            DataSet dataset = GetDataSet(criteria);

            if (dataset != null && dataset.Tables != null)
            {
                int tableCount = dataset.Tables.Count;

                if (tableCount > 0)
                {
                    tickets = TicketDetailTransformer.Transform(dataset.Tables[0], custId);
                }

                if (tableCount > 1)
                {
                    ticketData = TicketDataTransformer.Transform(dataset.Tables[1]);
                }
            }
        }

        public DataTable GetBetTypeByDate(ICriteria criteria)
        {
            DataSet dataset = GetDataSet(criteria);

            if (dataset != null && dataset.Tables != null && dataset.Tables.Count > 0)
            {
                return dataset.Tables[0];
            }

            return new DataTable();
        }

        public int GetCustIdByUserName(ICriteria criteria)
        {
            DataSet dataset = GetDataSet(criteria);

            if (dataset != null &&
                dataset.Tables != null &&
                dataset.Tables.Count > 0 &&
                dataset.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(dataset.Tables[0].Rows[0]["custid"]);
            }

            return 0;
        }

        private DataSet GetDataSet(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteDataSet(criteria.GetSpParams());
            }
        }
    }
}
