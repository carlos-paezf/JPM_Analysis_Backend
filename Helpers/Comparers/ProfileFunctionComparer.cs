using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Helpers
{
    public class ProfileFunctionComparer : IEqualityComparer<ProfileFunctionModel>
    {
        public bool Equals(ProfileFunctionModel? x, ProfileFunctionModel? y)
        {
            return x!.Id == y!.Id
                && x.ProfileId == y.ProfileId
                && x.FunctionId == y.FunctionId;
        }

        public int GetHashCode(ProfileFunctionModel obj)
        {
            return HashCode.Combine(obj.Id, obj.ProfileId, obj.FunctionId);
        }
    }
}