using System;
using System.Collections.Generic;

namespace BackendJPMAnalysis.Models
{
    public partial class CompanyUser
    {
        public CompanyUser()
        {
            UserEntitlements = new HashSet<UserEntitlement>();
        }

        public string AccessId { get; set; } = null!;
        public string? UserName { get; set; }
        public bool? UserStatus { get; set; }
        public string? UserType { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmailAddress { get; set; }
        public string? UserLocation { get; set; }
        public string? UserCountry { get; set; }
        /// <summary>
        /// RSA Token o Password
        /// </summary>
        public string? UserLogonType { get; set; }
        /// <summary>
        /// Se debe hacer una conversión
        /// </summary>
        public DateTime? UserLastLogonDt { get; set; }
        public string? UserLogonStatus { get; set; }
        public string? UserGroupMembership { get; set; }
        public string? UserRole { get; set; }
        public string? ProfileId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Profile? Profile { get; set; }
        public virtual ICollection<UserEntitlement> UserEntitlements { get; set; }
    }
}
