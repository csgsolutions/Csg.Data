using System;
using System.Collections.Generic;

namespace Csg.Data.Sql
{
    /// <summary>
    /// The arguments object tracks the parameters and tables used in a query rendering process.
    /// </summary>
    public class SqlBuildArguments
    {
        /// <summary>
        /// The parameter prefix that will be used when rendering parameter names.
        /// </summary>
        public const string SqlParameterPrefix = "@";
        /// <summary>
        /// The format of a table name that will be used when rendering.
        /// </summary>
        public const string SqlTableNameFormat = "t{0}";

        private List<DbParameterValue> _params;
        private List<ISqlTable> _tables;

        private int _paramIndex;

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

        public void AssignAlias(ISqlTable table)
        {
            int index = _tables.IndexOf(table);
            if (index < 0)
            {
                _tables.Add(table);
            }
        }

        public string TableName(ISqlTable table)
        {
            int index = _tables.IndexOf(table);
            if (index < 0)
            {
                throw new InvalidOperationException("The specified table is not associated with the query.");
            }
            if (_tables.Count > 1)
            {
                index = _tables.Count - 1;
            }
            return string.Format(SqlTableNameFormat, index);
        }
    }
}