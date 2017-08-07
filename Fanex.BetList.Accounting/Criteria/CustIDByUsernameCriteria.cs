using Fanex.NAP.Framework.Data;

namespace Fanex.BetList.Accounting.Criteria
{
    public class CustIDByUsernameCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "custname")]
        public string CustomerName { get; set; }

        public override string GetSettingKey()
        {
            return "GetCustIdByUsername";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
