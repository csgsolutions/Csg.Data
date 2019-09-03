using Csg.Data.Abstractions;

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
        /// Renders the T-SQL to the given text writer.
        /// </summary>
        /// <param name="writer">An instance of a T-SQL compatible text writer.</param>
        /// <param name="args">An instance of <see cref="SqlBuildArguments"/>.</param>
        protected abstract void Render(Abstractions.ISqlTextWriter writer);

        /// <summary>
        /// Gets the portion of a SELECT column that would be renderd after the AS keyword.
        /// </summary>
        /// <returns>A string</returns>
        protected virtual string GetAlias()
        {
            return this.Alias;
        }

        /// <summary>
        /// Renders the portion of a SELECT column that would be rendered before the AS keyword.
        /// </summary>
        /// <param name="writer">An instance of a T-SQL compatible text writer.</param>
        /// <param name="args">An instance of <see cref="SqlBuildArguments"/>.</param>
        protected abstract void RenderValueExpression(ISqlTextWriter writer);

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

        void ISqlStatementElement.Render(Abstractions.ISqlTextWriter writer)
        {
 	        this.Render(writer);
        }

        void ISqlColumn.RenderValueExpression(ISqlTextWriter writer)
        {
            this.RenderValueExpression(writer);
        }

        #endregion
    }
}
