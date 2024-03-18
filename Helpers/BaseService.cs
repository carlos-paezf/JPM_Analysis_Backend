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
    }


    public interface ISoftDeleteService
    {
        public abstract Task SoftDelete(string pk);
        public abstract Task Restore(string pk);
    }


    public interface IHardDeleteService
    {
        public abstract Task HardDelete(string pk);
    }


    public interface IBulkPostService<Model> where Model : class
    {
        public abstract Task<ListResponseDTO<Model>> BulkPost(ICollection<Model> collectionBody);
    }


    public interface IBulkHardDeleteService
    {
        public abstract Task<int> BulkHardDelete(ICollection<string> pks);
    }
}