using System;
using System.Collections.Generic;

namespace BackendJPMAnalysis.Models
{
    public partial class Profile
    {
        public Profile()
        {
            CompanyUsers = new HashSet<CompanyUser>();
            ProfilesFunctions = new HashSet<ProfilesFunction>();
        }

        /// <summary>
        /// Profile name in snake_case
        /// </summary>
        public string Id { get; set; } = null!;
        public string? ProfileName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<CompanyUser> CompanyUsers { get; set; }
        public virtual ICollection<ProfilesFunction> ProfilesFunctions { get; set; }
    }
}
