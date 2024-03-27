using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Helpers
{
    public class UserEntitlementComparer : IEqualityComparer<UserEntitlementModel>
    {
        public bool Equals(UserEntitlementModel? x, UserEntitlementModel? y)
        {
            return x!.Id == y!.Id
                && x.AccessId == y.AccessId
                && x.AccountNumber == y.AccountNumber
                && x.ProductId == y.ProductId
                && x.FunctionId == y.FunctionId
                && x.FunctionType == y.FunctionType;
        }

        public int GetHashCode(UserEntitlementModel obj)
        {
            return HashCode.Combine(obj.Id, obj.AccessId, obj.AccountNumber, obj.ProductId, obj.FunctionId, obj.FunctionType);
        }
    }
}