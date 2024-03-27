using BackendJPMAnalysis.DTO;


namespace BackendJPMAnalysis.Helpers
{
    public class FunctionProductComparer : IEqualityComparer<FunctionProductDTO>
    {
        public bool Equals(FunctionProductDTO? x, FunctionProductDTO? y)
        {
            return x!.FunctionId == y!.FunctionId
                && x.FunctionType == y.FunctionType
                && x.ProductId == y.ProductId;
        }

        public int GetHashCode(FunctionProductDTO obj)
        {
            return HashCode.Combine(obj.FunctionId, obj.FunctionType, obj.ProductId);
        }
    }
}