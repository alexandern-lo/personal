using System;
using System.Collections;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using SQLite;

namespace LiveOakApp.Models.Services
{
    public class SQLiteDatabase
    {
        string sqliteFilename;
        string sqliteFolderName;

        public SQLiteDatabase(string dbFileName)
        {
            UseDatabaseName(dbFileName);
        }

        protected SQLiteDatabase()
        {
        }

        protected void UseDatabaseName(string dbName, string folderName = null)
        {
            sqliteFilename = string.Format("{0}.db", dbName);
            sqliteFolderName = folderName;
        }

        public virtual SQLiteAsyncConnection GetConnection()
        {
            // TODO: connection can be reused? Close them in ServiceLocator .Terminate() and in .DropInMemoryCaches()
            return CreateConnection();
        }

        #region Connections

        SQLiteAsyncConnection CreateConnection()
        {
            var path = LocalPathForSQLiteFilename();
            return new SQLiteAsyncConnection(path, false);
        }

        string LocalPathForSQLiteFilename()
        {
            string documentsPath = "";
#if __IOS__
            documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            documentsPath = Path.Combine(documentsPath, "..", "Library", "Application Support");
#endif

#if __ANDROID__
            documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#endif
            var folderPath = sqliteFolderName == null ? documentsPath : Path.Combine(documentsPath, sqliteFolderName);
            Directory.CreateDirectory(folderPath);
            return Path.Combine(folderPath, sqliteFilename);
        }

        #endregion
    }

    public static class SQLiteExtensions
    {
        public static int InsertAllNonTransaction(this SQLiteConnection db, IEnumerable objects)
        {
            int count = 0;
            foreach (var r in objects)
            {
                count += db.Insert(r);
            }
            return count;
        }

        public static int Delete<T>(this TableQuery<T> tableQuery, Expression<Func<T, bool>> predExpr)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = tableQuery.GetType();
            MethodInfo method = type.GetMethod("CompileExpr", flags);

            if (predExpr.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)predExpr;
                var pred = lambda.Body;

                var args = new List<object>();

                var w = method.Invoke(tableQuery, new object[] { pred, args });
                var compileResultType = w.GetType();
                var prop = compileResultType.GetProperty("CommandText");
                string commandText = prop.GetValue(w, null).ToString();

                var cmdText = "delete from \"" + tableQuery.Table.TableName + "\"";
                cmdText += " where " + commandText;
                var command = tableQuery.Connection.CreateCommand(cmdText, args.ToArray());

                int result = command.ExecuteNonQuery();
                return result;
            }
            throw new NotSupportedException("Must be a predicate");
        }
    }
}
