using System;
using System.Collections.Generic;
using System.IO;
using Fanex.BetList.Accounting.Criteria;
using Fanex.BetList.Accounting.Models;

namespace Fanex.BetList.Accounting.Services
{
    public interface IBetListService
    {
        IList<TicketDetail> GetBetListByCustInfo_Refno(string transactionId, int customerId);

        IList<TicketDetail> GetBetListByCustInfo_DateRange(
            int custId,
            DateTime fromDate,
            DateTime toDate,
            int betType,
            int pageSize,
            int pageIndex,
            ref int quantity,
            bool isCache,
            bool isLimitForU3 = false);

        IList<BetType> GetBetTypeByDate(BetTypeByDateCriteria criteria);

        int GetCustIdByUserName(string userName);

        IList<TicketDetail> GetBetListStatement(
            int custId,
            int transType,
            DateTime fromDate,
            ref int quality,
            ref decimal totalComm,
            ref decimal totalWinLoss);

        MemoryStream ExportExcelBetListStatement(
            int custId,
            int transType,
            DateTime fromDate,
            ref string fileName);
    }
}
