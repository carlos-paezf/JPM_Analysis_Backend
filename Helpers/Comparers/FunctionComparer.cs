using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Helpers
{
    public class FunctionComparer : IEqualityComparer<FunctionModel>
    {
        public bool Equals(FunctionModel? x, FunctionModel? y)
        {
            return x!.Id == y!.Id
                && x.FunctionName == y.FunctionName;
        }

        public int GetHashCode(FunctionModel obj)
        {
            return HashCode.Combine(obj.Id, obj.FunctionName);
        }
    }
}