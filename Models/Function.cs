using System;
using System.Collections.Generic;

namespace BackendJPMAnalysis.Models
{
    public partial class Function
    {
        public Function()
        {
            ProfilesFunctions = new HashSet<ProfilesFunction>();
            UserEntitlements = new HashSet<UserEntitlement>();
        }

        /// <summary>
        /// Function name in snake_case
        /// </summary>
        public string Id { get; set; } = null!;
        public string? FunctionName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<ProfilesFunction> ProfilesFunctions { get; set; }
        public virtual ICollection<UserEntitlement> UserEntitlements { get; set; }
    }
}
