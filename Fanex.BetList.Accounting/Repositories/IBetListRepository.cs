using System.Collections.Generic;
using System.Data;
using Fanex.BetList.Accounting.Models;
using Fanex.BetList.Core.Entities;
using Fanex.NAP.Framework.Data;

namespace Fanex.BetList.Accounting.Repositories
{
    public interface IBetListRepository
    {
        void GetBetListData(
            ICriteria criteria,
            int custId,
            ref IList<TicketDetail> tickets,
            ref List<ITicketData> ticketData);

        DataTable GetBetTypeByDate(ICriteria criteria);

        int GetCustIdByUserName(ICriteria criteria);
    }
}
