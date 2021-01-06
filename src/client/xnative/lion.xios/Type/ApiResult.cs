namespace Lion.XIOS.Type
{
    public class ApiResult<T>
    {
        public bool success;
        public string message;
        public T result;
    }
}