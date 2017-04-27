using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Renders a T-SQL RANK_OVER() statement
    /// </summary>
    public class SqlRankColumn : SqlColumn
    {
        /// <summary>
        /// Creates a new instance for the specified table, column, and aggregate type.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="aggregateType"></param>
        /// <param name="alias"></param>
        /// <param name="rankDescending"></param>
        public SqlRankColumn(ISqlTable table, string columnName, SqlAggregate aggregateType, string alias, bool rankDescending) : base(table, alias)
        {
            this.ColumnName = columnName;
            this.Aggregate = aggregateType;
            this.RankDescending = rankDescending;
        }
        
        /// <summary>
        /// Gets or sets a value to indicate if the ranking is ascending (true) or descending (false).
        /// </summary>
        public bool RankDescending { get; set; }

        /// <summary>
        /// Renders the portion of the SQL statement that contains the value.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        protected override void RenderValueExpression(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.WriteRankOver(this.ColumnName, args.TableName(this.Table), this.Aggregate, this.RankDescending);
        }

        /// <summary>
        /// Renders the entire SQL statement to the writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        protected override void Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.WriteRankOver(this.ColumnName, args.TableName(this.Table), this.Aggregate, this.Alias, this.RankDescending);
        }
    }
}
