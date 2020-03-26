using SQLite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Storage
{
    public sealed class ContextProvider
    {
        private static Lazy<SQLiteAsyncConnection> connection = new Lazy<SQLiteAsyncConnection>(() => new SQLiteAsyncConnection("db.sqlite"));
        public static SQLiteAsyncConnection Conn => connection.Value;
    }
}
