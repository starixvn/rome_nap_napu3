using System;
using Fanex.NAP.Framework.Data;

namespace Fanex.BetList.Accounting.Criteria
{
    public class BetListStatement : CriteriaBase
    {
        public BetListStatement()
        {
            Language = "en";
            TransId = 0;
        }

        [SpParam(SqlParamName = "CustID")]
        public int CustId { get; set; }

        public int TransType { get; set; }

        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "language")]
        public string Language { get; set; }

        [SpParam(SqlParamName = "transid")]
        public int TransId { get; set; }

        public DateTime MoveBetsDay { get; set; }

        public override string GetSettingKey()
        {
            return "BetListByDayAll";
        }
    }
}
