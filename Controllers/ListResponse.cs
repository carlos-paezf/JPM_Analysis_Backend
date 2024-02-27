namespace BackendJPMAnalysis.Controllers
{
    public class ListResponse<T>
    {
        public ListResponse()
        {
            Data = new List<T>();
        }

        public int TotalResults { get; set; }
        public List<T> Data { get; set; }
    }
}