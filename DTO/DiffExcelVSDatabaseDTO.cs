namespace BackendJPMAnalysis.DTO
{
    public class DiffExcelVSDatabaseDTO<Model> where Model : class
    {
        public int CountRowsInDatabase { get; set; }
        public int CountRowsInExcel { get; set; }
        public int CountNewEntities { get; private set; }
        public ICollection<Model> NewEntities { get; set; }
        public int CountToDeleteEntities { get; private set; }
        public ICollection<Model> ToDeleteEntities { get; set; }
        public int CountEntitiesWithDifferences { get; private set; }
        public ICollection<DifferenceEntity<Model>> EntitiesWithDifferences { get; set; }


        public DiffExcelVSDatabaseDTO(int countRowsInDatabase, int countRowsInExcel, ICollection<Model> newEntities, ICollection<Model> toDeleteEntities, ICollection<DifferenceEntity<Model>> entitiesWithDifferences)
        {
            CountRowsInDatabase = countRowsInDatabase;
            CountRowsInExcel = countRowsInExcel;
            NewEntities = newEntities;
            ToDeleteEntities = toDeleteEntities;
            EntitiesWithDifferences = entitiesWithDifferences;
            CountNewEntities = newEntities.Count;
            CountToDeleteEntities = toDeleteEntities.Count;
            CountEntitiesWithDifferences = entitiesWithDifferences.Count;
        }
    }


    public class DifferenceEntity<Model> where Model : class
    {
        public Model ExcelEntity { get; set; } = null!;
        public Model DatabaseEntity { get; set; } = null!;
    }
}