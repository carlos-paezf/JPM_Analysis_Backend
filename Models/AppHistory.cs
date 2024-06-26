﻿using System.Text.Json.Serialization;


namespace BackendJPMAnalysis.Models
{
    public partial class AppHistoryModel
    {
        /// <summary>
        /// Autoincremental
        /// </summary>
        public int Id { get; set; }
        public string? AppUserId { get; set; }
        public string? AppFunction { get; set; }
        public string? AppTable { get; set; }

        [JsonIgnore]
        public virtual AppUserModel? AppUser { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, AppUserId: {AppUserId}, AppFunction: {AppFunction}, AppTable: {AppTable}";
        }
    }
}
