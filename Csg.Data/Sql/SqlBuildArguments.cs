using System;
using System.Collections.Generic;

namespace Csg.Data.Sql
{
    /// <summary>
    /// The collection of tables and parameters accumulated during a query render.
    /// </summary>
    public class SqlBuildArguments
    {
        private const string SqlTableNameFormat = "t{0}";
        private List<DbParameterValue> _params;
        private List<ISqlTable> _tables;     
        private int _paramIndex = 0;

        public SqlBuildArguments()
        {
            _params = new List<DbParameterValue>();
            _paramIndex = -1;
            _tables = new List<ISqlTable>();
        }

        /// <summary>
        /// Gets or sets a value that indicates if the build process should be cancelled.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the collection of parameters accumulated during query rendering.
        /// </summary>
        public IList<DbParameterValue> Parameters
        {
            get
            {
                return _params;
            }
        }

        /// <summary>
        /// Creates a new SqlParameter, optionally specifying the dbtype.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public string CreateParameter(object value, System.Data.DbType dbType, int? size = null)
        {
            _paramIndex++;

            var p = new DbParameterValue()
            {
                ParameterName = string.Concat("p", this._paramIndex.ToString()),
                Value = util.ConvertValue(value, dbType),
                DbType = dbType,
                Size = size
            };
            
            this.Parameters.Add(p);

            return p.ParameterName;
        }

        /// <summary>
        /// Assigns an alias to a table if it does not already exist.
        /// </summary>
        /// <param name="table"></param>
        public void AssignAlias(ISqlTable table)
        {
            int index = _tables.IndexOf(table);
            if (index < 0)
            {
                _tables.Add(table);
            }
        }

        /// <summary>
        /// Gets the table alias assigned to a given table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string TableName(ISqlTable table)
        {
            int index = _tables.IndexOf(table);
            if (index < 0)
            {
                throw new InvalidOperationException("The specified table is not associated with the query.");
            }
            return string.Format(SqlTableNameFormat, index);
        }
    }
}
