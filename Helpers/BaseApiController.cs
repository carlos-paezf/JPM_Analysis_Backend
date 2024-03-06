using BackendJPMAnalysis.DTO;
using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Helpers
{
    public interface IBaseApiController<Model, DTOSimple> where Model : class where DTOSimple : class
    {
        public abstract Task<ActionResult<ListResponseDTO<Model>>> GetAll();
        public abstract Task<ActionResult<Model>> GetByPk([FromRoute] string pk);
        public abstract Task<ActionResult> Post([FromBody] Model body);
        public abstract Task<ActionResult> UpdateByPK([FromRoute] string pk, [FromBody] DTOSimple body);
        public abstract Task<ActionResult> Delete([FromRoute] string pk);
        public abstract Task<ActionResult> Restore([FromRoute] string pk);
    }
}