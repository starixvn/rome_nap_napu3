using System;
using Fanex.BetList.Accounting.Services;
using Fanex.NAP.Framework.Data;

namespace Fanex.BetList.Accounting.Criteria
{
    public class BetTypeByDateCriteria : CriteriaBase
    {
        private string _settingKey = "Acc_BetList_SelBettype";

        public BetTypeByDateCriteria()
        {
            var balanceUp = ReportService.BalanceUp;

            MoveBetsDay = balanceUp.MoveBetsDay;
            BalanceUpDay = balanceUp.BalanceUpDay;
        }

        [SpParam(SqlParamName = "CustId")]
        public int CustId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime MoveBetsDay { get; set; }

        public DateTime BalanceUpDay { get; set; }

        public override string GetSettingKey()
        {
            return _settingKey;
        }

        public override bool IsValid()
        {
            return true;
        }

        public void SetSettingKeyForU3()
        {
            _settingKey = "U3_BetList_SelBettype";
        }
    }
}
