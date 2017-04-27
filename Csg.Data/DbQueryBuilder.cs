using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csg.Data.Sql;

namespace Csg.Data
{
    /// <summary>
    /// Provides a query command builder to create and execute a SELECT statement against a database.
    /// </summary>
    public class DbQueryBuilder : IDbQueryBuilder
    {
        private DbQueryBuilder(System.Data.IDbConnection connection, System.Data.IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;            
        }

        /// <summary>
        /// Creates a new instance using the given table expression and connection.
        /// </summary>
        /// <param name="sql">The name of a table, a table expression, or other object that can be the target of a SELECT query.</param>
        /// <param name="connection">The database connection.</param>
        public DbQueryBuilder(string sql, System.Data.IDbConnection connection) : this(connection, null)
        {
            this.Root = Csg.Data.Sql.SqlTable.Create(sql);
            this.Parameters = new List<DbParameterValue>();
            this.SelectColumns = new List<ISqlColumn>();
            this.Joins = new List<ISqlJoin>();
            this.Filters = new List<ISqlFilter>();
            this.OrderBy = new List<SqlOrderColumn>();
        }

        /// <summary>
        /// Creates a new instance using the given table expression, connection, and transaction.
        /// </summary>
        /// <param name="sql">The name of a table, a table expression, or other object that can be the target of a SELECT query.</param>
        /// <param name="connection">The database connection.</param>
        /// <param name="transaction">The database transaction.</param>
        public DbQueryBuilder(string sql, System.Data.IDbConnection connection, System.Data.IDbTransaction transaction) : this(sql, connection)
        {
            _transaction = transaction;
        }
        
        /// <summary>
        /// Gets or sets the collection of joins..
        /// </summary>
        protected ICollection<ISqlJoin> Joins { get; set; }

        /// <summary>
        /// Gets or sets the collection of filters.
        /// </summary>
        protected ICollection<ISqlFilter> Filters { get; set; }

        /// <summary>
        /// Returns the root table of the query. This is the table listed immmediately after the FROM clause
        /// </summary>
        public ISqlTable Root { get; protected set; }

        /// <summary>
        /// When implemented in a derived class, gets a collection of SELECT columns.
        /// </summary>
        public IList<ISqlColumn> SelectColumns { get; protected set; }

        /// <summary>
        /// Adds a JOIN to the FROM clause of the query.
        /// </summary>
        /// <param name="join"></param>
        public void AddJoin(ISqlJoin join)
        {
            this.Joins.Add(join);
        }

        /// <summary>
        /// Adds a WHERE clause to the query.
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(ISqlFilter filter)
        {
            this.Filters.Add(filter);
        }

        /// <summary>
        /// Gets the list of columns to order by.
        /// </summary>
        public IList<SqlOrderColumn> OrderBy { get; protected set; }

        /// <summary>
        /// Gets the connection associated with the query.
        /// </summary>
        public System.Data.IDbConnection Connection
        {
            get
            {
                return _connection;
            }
        }
        private System.Data.IDbConnection _connection;

        /// <summary>
        /// Gets the transaction associated with the query.
        /// </summary>
        public System.Data.IDbTransaction Transaction
        {
            get
            {
                return _transaction;
            }
        }
        private System.Data.IDbTransaction _transaction;

        /// <summary>
        /// Gets or sets the query execution timeout.
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if SELECT DISTINCT will be used.
        /// </summary>
        public bool Distinct { get; set; }

        /// <summary>
        /// Gets the parameter value collection.
        /// </summary>
        public ICollection<DbParameterValue> Parameters { get; protected set; }
        
        /// <summary>
        /// Gets a SQL statement for the given query.
        /// </summary>
        /// <returns></returns>
        public SqlStatement Render()
        {
            var builder = new SqlSelectBuilder(this.Root, this.Joins, this.SelectColumns, this.Filters, this.OrderBy)
            {
                SelectDistinct = this.Distinct,
                GenerateFormattedSql = true,
            };

            //TODO: To support xplat db platforms, we would need to pass in a writer and build args here
            var stmt = builder.Render();

            foreach(var param in this.Parameters)
            {
                stmt.Parameters.Add(param);
            }

            return stmt;
        }      

        /// <summary>
        /// Returns an initalized database command.
        /// </summary>
        /// <returns></returns>
        public System.Data.IDbCommand CreateCommand()
        {
            var stmt = this.Render();
            var cmd = stmt.CreateCommand(this.Connection);

            cmd.Transaction = this.Transaction;
            cmd.CommandTimeout = this.CommandTimeout;
            
            return cmd;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbQueryBuilder"/> configured in the same manner as the existing one.
        /// </summary>
        /// <returns></returns>
        public IDbQueryBuilder Fork()
        {
            var builder = new DbQueryBuilder(this.Connection, this.Transaction);
            builder.CommandTimeout = this.CommandTimeout;
            builder.Root = this.Root;
            builder.Joins = new List<ISqlJoin>(this.Joins);
            builder.Filters = new List<ISqlFilter>(this.Filters);
            builder.SelectColumns = new List<ISqlColumn>(this.SelectColumns);
            builder.Distinct = this.Distinct;
            builder.Parameters = new List<DbParameterValue>(this.Parameters);
            builder.OrderBy = new List<SqlOrderColumn>(this.OrderBy);
            return builder;
        }

        /// <summary>
        /// Gets the command text returned from the <see cref="Render"/> method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Render().CommandText;
        }

    }
}