using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Qoden.Validation;

namespace Avend.API.Infrastructure
{
    public enum SqlAppLockMode
    {
        Shared,
        Update,
        IntentShared,
        IntentExclusive,
        Exclusive
    }

    public enum SqlAppLockOwner
    {
        Session,
        Transaction
    }

    public class SqlAppLock : IDisposable
    {
        private readonly int _lockResult;
        private readonly DbConnection _connection;
        private readonly string _lockId;
        private readonly IDbContextTransaction _transaction;
        private SqlAppLockOwner _lockOwner;

        public SqlAppLock(DatabaseFacade facade,
            string lockId,            
            SqlAppLockMode lockMode = SqlAppLockMode.Exclusive,
            SqlAppLockOwner lockOwner = SqlAppLockOwner.Transaction,
            int lockTimeout = 60000,
            string dbPrincipal = "public")
        {
            Assert.Argument(facade, nameof(facade)).NotNull();
            Assert.Argument(lockId, nameof(lockId)).NotNull();
            if (lockOwner == SqlAppLockOwner.Transaction)
            {
                Assert.State(facade.CurrentTransaction != null, "facade")
                    .IsTrue("{Key} does not have active transaction but lockOwner is set to SqlAppLockOwner.Transaction");
            }

            _connection = facade.GetDbConnection();
            _transaction = facade.CurrentTransaction;
            _lockId = lockId;            
            _lockOwner = lockOwner;

            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            var spAppLock = _connection.CreateCommand();
            spAppLock.CommandTimeout = lockTimeout;
            spAppLock.CommandText = "sp_getapplock";
            spAppLock.CommandType = CommandType.StoredProcedure;
            spAppLock.Parameters.Add(new SqlParameter("Resource", lockId));
            spAppLock.Parameters.Add(new SqlParameter("LockMode", lockMode.ToString()));
            spAppLock.Parameters.Add(new SqlParameter("LockOwner", lockOwner.ToString()));
            if (lockOwner == SqlAppLockOwner.Transaction)
                spAppLock.Transaction = _transaction.GetDbTransaction();
            spAppLock.Parameters.Add(new SqlParameter("LockTimeout", SqlDbType.Int) { Value = lockTimeout });
            spAppLock.Parameters.Add(new SqlParameter("DbPrincipal", dbPrincipal));

            var returnValue = new SqlParameter("Result", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            spAppLock.Parameters.Add(returnValue);

            spAppLock.ExecuteNonQuery();
            _lockResult = (int)returnValue.Value;
            if (_lockResult < 0)
            {
                throw new InvalidOperationException(
                    string.Format("sp_getapplock failed with error {0}. Cannot aquire MS SQL lock '{1}'",
                        _lockResult, lockId));
            }
        }

        public void Dispose()
        {
            if (_lockResult >= 0)
            {
                var spReleaseLock = _connection.CreateCommand();
                if (_lockOwner == SqlAppLockOwner.Transaction)
                    spReleaseLock.Transaction = _transaction.GetDbTransaction();
                spReleaseLock.CommandText = "sp_releaseapplock";
                spReleaseLock.CommandType = CommandType.StoredProcedure;
                spReleaseLock.Parameters.Add(new SqlParameter("Resource", _lockId));
                spReleaseLock.Parameters.Add(new SqlParameter("LockOwner", _lockOwner.ToString()));
                spReleaseLock.ExecuteNonQuery();
            }
        }
    }
}