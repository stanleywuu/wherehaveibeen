namespace Storage
{
    public interface IContextInfoProvider
    {
        string ConnectionString { get; }
    }

    public class ContextInfo : IContextInfoProvider
    {
        public string ConnectionString { get; set; }
    }
}