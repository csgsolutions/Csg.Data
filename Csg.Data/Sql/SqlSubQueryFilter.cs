using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    public class SqlSubQueryFilter : ISqlFilter
    {
        public SqlSubQueryFilter(ISqlTable table, ISqlTable subQueryTable)
        {
            this.Table = table;
            this.SubQueryTable = subQueryTable;
        }

        public SqlSubQueryFilter(ISqlTable table, string subQueryTable)
        {
            this.Table = table;
            this.SubQueryTable = SqlTable.Create(subQueryTable);
        }

        /// <summary>
        /// Gets or sets the table to filter.
        /// </summary>
        public ISqlTable Table { get; set; }

        /// <summary>
        /// Gets or sets the column in the left table to compare to the sub-query.
        /// </summary>
        public string ColumnName { get;set; }

        /// <summary>
        /// Gets or sets the column in the sub-query to match the left column against.
        /// </summary>
        public string SubQueryColumn { get; set; }
        
        /// <summary>
        /// Gets or sets the sub-query table expression
        /// </summary>
        public ISqlTable SubQueryTable { get; set; }

        /// <summary>
        /// Gets a collection of filters to apply to the sub-query.
        /// </summary>
        public ICollection<ISqlFilter> SubQueryFilters
        {
            get
            {
                if (_subQueryFilters == null)
                {
                    _subQueryFilters = new List<ISqlFilter>();
                }
                return _subQueryFilters;
            }
        }
        private List<ISqlFilter> _subQueryFilters;

        /// <summary>
        /// Gets or sets a value that indicates if the operator applied is 'NOT IN' or 'IN'
        /// </summary>
        public SubQueryMode SubQueryMode { get; set; }

        /// <summary>
        /// Gets or sets the count value to use for comparison when SubQueryMode is Count
        /// </summary>
        public int CountValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the operator to use when SubQueryMode is Count
        /// </summary>
        public SqlOperator CountOperator { get; set; } = SqlOperator.GreaterThanOrEqual;
                        
        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.WriteBeginGroup();

            if (this.SubQueryMode == SubQueryMode.InList || this.SubQueryMode == SubQueryMode.NotInList)
            {
                writer.WriteColumnName(this.ColumnName, args.TableName(this.Table));
                writer.WriteSpace();

                if (this.SubQueryMode == SubQueryMode.NotInList)
                {
                    writer.Write(SqlConstants.NOT);
                    writer.WriteSpace();
                }

                writer.Write(SqlConstants.IN);
            }

            writer.WriteSpace();

            args.AssignAlias(this.SubQueryTable);
            
            var builder = new SqlSelectBuilder(this.SubQueryTable);
            var subQueryColumn = new SqlColumn(this.SubQueryTable, this.SubQueryColumn);
            if (this.SubQueryMode == SubQueryMode.Count)
            {
                subQueryColumn.Aggregate = SqlAggregate.Count;
                subQueryColumn.Alias = "Cnt";
            }

            builder.Columns.Add(subQueryColumn);

            foreach (var filter in this.SubQueryFilters)
            {
                builder.Filters.Add(filter);
            }

            writer.WriteBeginGroup();
            builder.Render(writer, args);
            writer.WriteEndGroup();

            if (this.SubQueryMode == SubQueryMode.Count)
            {
                writer.WriteSpace();
                writer.WriteOperator(this.CountOperator);
                writer.WriteSpace();
                writer.WriteParameter(args.CreateParameter(this.CountValue, System.Data.DbType.Int32));
            }

            writer.WriteEndGroup();
        }
    }

    public enum SubQueryMode
    {
        InList,
        NotInList,
        Count
    }

}
