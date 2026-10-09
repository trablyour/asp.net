namespace PracticeApi.Services
{
    // результат операции: успех, не найдено или ошибка с текстом
    public class OperationResult<T>
    {
        public bool Success { get; private set; }
        public bool NotFound { get; private set; }
        public string? Error { get; private set; }
        public T? Value { get; private set; }

        public static OperationResult<T> Ok(T value)
        {
            return new OperationResult<T> { Success = true, Value = value };
        }

        public static OperationResult<T> Fail(string error)
        {
            return new OperationResult<T> { Error = error };
        }

        public static OperationResult<T> Missing(string error)
        {
            return new OperationResult<T> { NotFound = true, Error = error };
        }
    }
}
