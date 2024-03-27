using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class ProductAccountModel : BaseModel
    {
        private string? _accountNumber;

        /// <summary>
        /// Auto-incremental ID
        /// </summary>
        [Key]
        public string Id { get; private set; } = null!;
        public string? ProductId { get; set; }
        public string? AccountNumber
        {
            get => _accountNumber!;
            set
            {
                _accountNumber = value;
                Id ??= StringUtil.SnakeCase(ProductId + '_' + AccountNumber);
            }
        }

        [JsonIgnore]
        public virtual AccountModel? Account { get; set; }

        [JsonIgnore]
        public virtual ProductModel? Product { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, ProductId: {ProductId}, AccountNumber: {AccountNumber}";
        }
    }


    public static class ProductAccountModelExtensions
    {
        public static string GetId(this ProductAccountModel productAccount)
        {
            return EntityExtensions.GetId(productAccount);
        }
    }
}
