using BackendJPMAnalysis.DTO;


namespace BackendJPMAnalysis.Helpers
{
    public interface IBaseService<Model, DTOEager, DTOSimple> where Model : class where DTOEager : class where DTOSimple : class
    {
        public abstract Task<ListResponseDTO<Model>> GetAll();
        public abstract Task<DTOEager?> GetByPk(string pk);
        public abstract Task<Model?> GetByPkNoTracking(string pk);
        public abstract Task Post(Model postBody);
        public abstract Task<DTOSimple> UpdateByPK(string pk, DTOSimple updatedBody);
        public abstract Task Delete(string pk);
        public abstract Task Restore(string pk);
    }
}