namespace Lion.XDroid.Type
{
    public class ApiResult<T>
    {
        public bool success;
        public string message;
        public T result;
    }
}