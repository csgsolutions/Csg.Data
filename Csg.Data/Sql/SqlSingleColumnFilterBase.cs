using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{

    /// <summary>
    /// Provides a base class for a filter that compares against a single column
    /// </summary>
    public abstract class SqlSingleColumnFilterBase : ISqlFilter
    {
        public SqlSingleColumnFilterBase(ISqlTable table, string columnName) : this(table, columnName, System.Data.DbType.Object)
        {
        }

        public SqlSingleColumnFilterBase(ISqlTable table, string columnName, System.Data.DbType dataType)
        {
            this.Table = table;
            this.ColumnName = columnName;
            this.DataType = dataType;
        }


        /// <summary>
        /// Gets or sets the related table.
        /// </summary>
        public ISqlTable Table { get; set; }

        /// <summary>
        /// Gets or sets the data column name
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the value data type.
        /// </summary>
        public System.Data.DbType DataType { get; set; }

        /// <summary>
        /// Gets or sets the maximum size, in bytes, of the value.
        /// </summary>
        public int? Size { get; set; }

        #region ISqlFilter Members

        protected abstract void RenderInternal(Abstractions.ISqlTextWriter writer);

        #endregion

        #region ISqlStatementElement Members

        void ISqlStatementElement.Render(Abstractions.ISqlTextWriter writer)
        {
            this.RenderInternal(writer);
        }

        #endregion
    }
}
