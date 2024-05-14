using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Helpers
{
    public class CompanyUserComparer : IEqualityComparer<CompanyUserModel>
    {
        public bool Equals(CompanyUserModel? x, CompanyUserModel? y)
        {
            return x!.AccessId == y!.AccessId
                && x.UserName == y.UserName
                && x.UserStatus == y.UserStatus
                && x.UserType == y.UserType
                && x.EmployeeId == y.EmployeeId
                && x.EmailAddress == y.EmailAddress
                && x.UserLocation == y.UserLocation
                && x.UserCountry == y.UserCountry
                && x.UserLogonType == y.UserLogonType
                // TODO: Evaluar la necesidad de usar estas propiedades en la comparación
                // && x.UserLastLogonDt == y.UserLastLogonDt
                // && x.UserLogonStatus == y.UserLogonStatus
                && x.UserGroupMembership == y.UserGroupMembership
                && x.UserRole == y.UserRole
                && x.ProfileId == y.ProfileId;
        }

        public int GetHashCode(CompanyUserModel obj)
        {
            return HashCode.Combine(obj.AccessId, obj.UserName, obj.UserStatus, obj.UserType, obj.ProfileId);
        }
    }
}