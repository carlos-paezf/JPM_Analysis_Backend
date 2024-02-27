using System;
using System.Collections.Generic;

namespace BackendJPMAnalysis.Models
{
    public partial class Product
    {
        public Product()
        {
            Clients = new HashSet<Client>();
            UserEntitlements = new HashSet<UserEntitlement>();
        }

        /// <summary>
        /// Product name in snake_case
        /// </summary>
        public string Id { get; set; } = null!;
        public string? ProductName { get; set; }
        public string? SubProduct { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<UserEntitlement> UserEntitlements { get; set; }
    }
}
