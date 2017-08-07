namespace Fanex.BetList.Accounting.Models
{
    public class BetType
    {
        public BetType()
        {
        }

        public BetType(int id, string name)
        {
            BetTypeID = id;
            BetTypeName = name;
        }

        public int BetTypeID { get; set; }

        public string BetTypeName { get; set; }
    }
}
