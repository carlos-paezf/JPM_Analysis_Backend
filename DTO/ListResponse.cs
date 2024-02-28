namespace BackendJPMAnalysis.DTO
{
    public class ListResponseDTO<T>
    {
        public ListResponseDTO()
        {
            Data = new List<T>();
        }

        public int TotalResults { get; set; }
        public List<T> Data { get; set; }
    }
}