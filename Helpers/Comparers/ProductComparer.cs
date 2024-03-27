using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Helpers
{
    public class ProductComparer : IEqualityComparer<ProductModel>
    {
        public bool Equals(ProductModel? x, ProductModel? y)
        {
            return x!.ProductName == y!.ProductName
                && x.SubProduct == y.SubProduct;
        }

        public int GetHashCode(ProductModel obj)
        {
            return HashCode.Combine(obj.ProductName, obj.SubProduct);
        }
    }
}