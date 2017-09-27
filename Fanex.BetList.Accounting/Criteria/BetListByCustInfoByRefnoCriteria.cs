using System;
using Fanex.NAP.Framework.Data;

namespace Fanex.BetList.Accounting.Criteria
{
    public class BetListByCustInfoByRefnoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustID")]
        public int CustId { get; set; }

        [SpParam(SqlParamName = "TransID")]
        public string TransId { get; set; }

        [SpParam(SqlParamName = "MoveBetsDay")]
        public DateTime MoveBetsDay { get; set; }

        [SpParam(SqlParamName = "BalanceUpDay")]
        public DateTime BalanceUpDay { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_BetsSelBetList_ByTransID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
