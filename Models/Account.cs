using System;
using System.Collections.Generic;

namespace BackendJPMAnalysis.Models
{
    public partial class Account
    {
        public Account()
        {
            Clients = new HashSet<Client>();
            UserEntitlements = new HashSet<UserEntitlement>();
        }

        /// <summary>
        /// Los valores que sean null se reemplazan por un 0
        /// </summary>
        public string AccountNumber { get; set; } = null!;
        public string? AccountName { get; set; }
        public string? AccountType { get; set; }
        public string? BankCurrency { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<UserEntitlement> UserEntitlements { get; set; }
    }
}
