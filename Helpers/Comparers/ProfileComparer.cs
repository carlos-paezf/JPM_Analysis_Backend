using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Helpers
{
    public class ProfileComparer : IEqualityComparer<ProfileModel>
    {
        public bool Equals(ProfileModel? x, ProfileModel? y)
        {
            return x!.Id == y!.Id
                && x.ProfileName == y.ProfileName;
        }

        public int GetHashCode(ProfileModel obj)
        {
            return HashCode.Combine(obj.Id, obj.ProfileName);
        }
    }
}