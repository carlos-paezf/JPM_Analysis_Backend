namespace BackendJPMAnalysis.Helpers
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string id)
            : base($"El registro con el id '{id}' no fue encontrado") { }
    }

    public class DuplicateException : Exception
    {
        public DuplicateException(string id)
            : base($"Ya se encuentra un registro con el id '{id}' registrado") { }
    }
}