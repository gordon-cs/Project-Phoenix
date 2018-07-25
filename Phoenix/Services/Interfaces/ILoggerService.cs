namespace Phoenix.Services
{
    public interface ILoggerService
    {
        void Error(string message);
        void Info(string message);
    }
}