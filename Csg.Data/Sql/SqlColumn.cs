using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Provides a base implementation for a class that renders a column in a T-SQL SELECT statement.
    /// </summary>    
    public abstract class SqlColumnBase : ISqlColumn
    {
        /// <summary>
        /// Initializes an instance with the given table interface.
        /// </summary>
        /// <param name="table"></param>
        protected SqlColumnBase(ISqlTable table)
        {
            this.Table = table;            
        }

        /// <summary>
        /// Initiliazes an instance with the given table interface and column alias.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="alias"></param>
        protected SqlColumnBase(ISqlTable table, string alias)
            : this(table)
        {
            this.Alias = alias;
        }

        /// <summary>
        /// Gets or sets the table interface associated with this field.
        /// </summary>
        public virtual ISqlTable Table { get; set; }

        /// <summary>
        /// Gets or sets the column alias.
        /// </summary>
        public virtual string Alias { get; set; }

        /// <summary>
        /// Gets or sets the aggregate function that will be applied to the column.
        /// </summary>
        public virtual SqlAggregate Aggregate { get; set; }

        /// <summary>
        /// Renders the portion of a SELECT column that would be rendered before the AS keyword.
        /// </summary>
        /// <param name="writer">An instance of a T-SQL compatible text writer.</param>
        /// <param name="args">An instance of <see cref="SqlBuildArguments"/>.</param>
        protected abstract void RenderValueExpression(SqlTextWriter writer, SqlBuildArguments args);

        /// <summary>
        /// Renders the T-SQL to the given text writer.
        /// </summary>
        /// <param name="writer">An instance of a T-SQL compatible text writer.</param>
        /// <param name="args">An instance of <see cref="SqlBuildArguments"/>.</param>
        protected abstract void Render(SqlTextWriter writer, SqlBuildArguments args);

        /// <summary>
        /// Gets the portion of a SELECT column that would be renderd after the AS keyword.
        /// </summary>
        /// <returns>A string</returns>
        protected virtual string GetAlias()
        {
            return this.Alias;
        }
        
        #region Interface Members

        bool ISqlColumn.IsAggregate
        {
            get
            {
                return this.Aggregate != SqlAggregate.None;
            }
        }

        string ISqlColumn.GetAlias()
        {
            return this.GetAlias();
        }

        void ISqlColumn.RenderValueExpression(SqlTextWriter writer, SqlBuildArguments args)
        {
 	        this.RenderValueExpression(writer, args);
        }

        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
 	        this.Render(writer,args);
        }

        #endregion
    }

    /// <summary>
    /// Renders a column reference.
    /// </summary>
    public class SqlColumn : SqlColumnBase
    {
        /// <summary>
        /// Initializes a new instance with the given table and column name.
        /// </summary>
        /// <param name="table">The table expression that this column references.</param>
        /// <param name="columnName">The name of the column.</param>
        public SqlColumn(ISqlTable table, string columnName) 
            : base(table, columnName)
        {
            this.ColumnName = columnName;
        }

        /// <summary>
        /// Initializes a new instance with the given table, column name, and column alias.
        /// </summary>
        /// <param name="table">The table expression that this column references.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="alias">The alias to use when selecting the column.</param>
        public SqlColumn(ISqlTable table, string columnName, string alias) 
            : base(table, alias)
        {
            this.ColumnName = columnName;
        }

        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Renders only the column name and any aggregate function, but not the alias name.
        /// </summary>
        /// <param name="writer">The SQL text writer to write to.</param>
        /// <param name="args">The build arguments</param>
        protected override void RenderValueExpression(SqlTextWriter writer, SqlBuildArguments args)
        {
            if (this.Aggregate == SqlAggregate.None)
            {
                writer.WriteColumnName(this.ColumnName, args.TableName(this.Table));
            }
            else
            {
                writer.WriteAggregate(this.ColumnName, args.TableName(this.Table), this.Aggregate);
            }
        }

        /// <summary>
        /// Renders the column name, table expression reference, and alias.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        protected override void Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            if (this.Aggregate == SqlAggregate.None)
            {
                writer.WriteColumnName(this.ColumnName, args.TableName(this.Table), this.Alias);
            }
            else
            {
                writer.WriteAggregateColumn(this.ColumnName, args.TableName(this.Table), this.Aggregate, this.Alias);
            }
        }

        /// <summary>
        /// Creates a <see cref="SqlColumn"/> from the given string expression and table expression.
        /// </summary>
        /// <param name="table">The table expression that this column references.</param>
        /// <param name="columnExpression">A column expression in the form of a single name, or name and alias.</param>
        /// <returns></returns>
        public static SqlColumn Parse(ISqlTable table, string columnExpression)
        {
            string[] parts = columnExpression.Split(new string[] { "AS" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                return new SqlColumn(table, parts[0].Trim(), parts[1].Trim());
            }
            else if (parts.Length == 1)
            {
                return new SqlColumn(table, parts[0].Trim());
            }
            else
            {
                throw new FormatException("The format of the columnExpression is not valid.");
            }
        }

    }
}
