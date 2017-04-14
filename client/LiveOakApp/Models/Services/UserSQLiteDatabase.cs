using System;
using SQLite;

namespace LiveOakApp.Models.Services
{
    public class UserSQLiteDatabase : SQLiteDatabase
    {
        string UserId;

        public event EventHandler OnCanGetConnectionChanged;

        public bool CanGetConnection()
        {
            return UserId != null;
        }

        public override SQLiteAsyncConnection GetConnection()
        {
            if (!CanGetConnection())
            {
                throw new NoDatabaseException();
            }
            return base.GetConnection();
        }

        public void UseDatabaseForUserId(string userId)
        {
            UserId = userId;
            UseDatabaseName(string.Format("{0}", UserId), "leads");
            OnCanGetConnectionChanged.Invoke(this, EventArgs.Empty);
        }
    }

    public class NoDatabaseException : Exception
    {
        public NoDatabaseException() { }

        public NoDatabaseException(string message) : base(message) { }

        public NoDatabaseException(string message, Exception inner) : base(message, inner) { }
    }
}
