namespace Fanex.BetList.Accounting.Models
{
    public class ThirdPartyResultViewModel
    {
        public ThirdPartyResultViewModel(string productName, string result)
        {
            Title = productName;
            Result = result;
        }

        public string Title { get; }
        public string Result { get; }
    }
}
