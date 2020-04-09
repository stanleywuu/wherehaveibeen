using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Application")]
namespace Storage
{
    public interface IContextProvider
    {
        IDbConnection Conn { get; }
    }

    public sealed class ContextProvider : IContextProvider
    {
        private IContextInfoProvider contextInfo;

        public static IContextProvider Instance { set; internal get; }

        /// <summary>
        /// Initialize with context information
        /// </summary>
        /// <param name="contextInfoProvider"></param>
        public ContextProvider(IContextInfoProvider contextInfoProvider)
        {
            contextInfo = contextInfoProvider;
        }

        public IDbConnection Conn
        {
            get
            {
                return new SqlConnection(contextInfo.ConnectionString);
            }
        }
    }
}
