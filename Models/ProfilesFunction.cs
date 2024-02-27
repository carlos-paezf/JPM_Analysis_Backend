using System;
using System.Collections.Generic;

namespace BackendJPMAnalysis.Models
{
    public partial class ProfilesFunction
    {
        /// <summary>
        /// Autoincremental
        /// </summary>
        public int Id { get; set; }
        public string? ProfileId { get; set; }
        public string? FunctionId { get; set; }

        public virtual Function? Function { get; set; }
        public virtual Profile? Profile { get; set; }
    }
}
