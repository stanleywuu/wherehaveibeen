using Dapper.Contrib.Extensions;
using Storage;
using System.Threading.Tasks;

namespace Application.Data
{
    public static class DataAccess
    {
        public static async Task<T> Get<T, TKey>(TKey key)
            where T : class
        {
            using (var conn = ContextProvider.Instance.Conn)
            {
                return await conn.GetAsync<T>(key);
            }
        }

        public static async Task<int> Insert<T>(this T item)
            where T : class
        {
            using (var conn = ContextProvider.Instance.Conn)
            {
                return await conn.InsertAsync(item);
            }
        }

        public static async Task<bool> UpdateAsync<T>(this T item)
            where T : class
        {
            using (var conn = ContextProvider.Instance.Conn)
            {
                return await conn.UpdateAsync(item);
            }
        }
    }
}
