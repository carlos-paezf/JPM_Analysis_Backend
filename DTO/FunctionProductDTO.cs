using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendJPMAnalysis.DTO
{
    public class FunctionProductDTO
    {
        public FunctionProductDTO()
        {
        }

        public string FunctionId { get; set; } = null!;
        public string FunctionType { get; set; } = null!;
        public string ProductId { get; set; } = null!;

        public FunctionProductDTO(string functionId, string functionType, string productId)
        {
            FunctionId = functionId;
            FunctionType = functionType;
            ProductId = productId;
        }
    }
}