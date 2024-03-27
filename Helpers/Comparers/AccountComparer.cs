using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Helpers
{
    public class AccountComparer : IEqualityComparer<AccountModel>
    {
        public bool Equals(AccountModel? x, AccountModel? y)
        {
            return x!.AccountNumber == y!.AccountNumber
                && x.AccountName == y.AccountName
                && x.AccountType == y.AccountType
                && x.BankCurrency == y.BankCurrency;
        }

        public int GetHashCode(AccountModel obj)
        {
            return HashCode.Combine(obj.AccountNumber, obj.AccountName, obj.AccountType, obj.BankCurrency);
        }
    }
}