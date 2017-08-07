using Fanex.NAP.Framework.Data;

namespace Fanex.BetList.Accounting.Criteria
{
    public class BetListByCustInfoByRefnoU3Criteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustID")]
        public int CustId { get; set; }

        [SpParam(SqlParamName = "TransID")]
        public string TransId { get; set; }

        public override string GetSettingKey()
        {
            return "U3_BetsSelBetList_ByTransID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
