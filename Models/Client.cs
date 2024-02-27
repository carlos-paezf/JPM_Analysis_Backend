using System;
using System.Collections.Generic;

namespace BackendJPMAnalysis.Models
{
    public partial class Client
    {
        /// <summary>
        /// Autoincremental
        /// </summary>
        public int Id { get; set; }
        public string? ProductId { get; set; }
        public string? AccountNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Account? AccountNumberNavigation { get; set; }
        public virtual Product? Product { get; set; }
    }
}
