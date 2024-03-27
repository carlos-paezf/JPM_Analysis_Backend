using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Helpers
{
    public class ProductAccountComparer : IEqualityComparer<ProductAccountModel>
    {
        public bool Equals(ProductAccountModel? x, ProductAccountModel? y)
        {
            return x!.Id == y!.Id
                && x.ProductId == y.ProductId
                && x.AccountNumber == y.AccountNumber;
        }

        public int GetHashCode(ProductAccountModel obj)
        {
            return HashCode.Combine(obj.Id, obj.ProductId, obj.AccountNumber);
        }
    }
}