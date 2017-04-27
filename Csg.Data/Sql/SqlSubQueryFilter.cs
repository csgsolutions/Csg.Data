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
        public string ColumnName { get;set;}

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
        public bool NotInList
        {
            get;
            set;
        }
        
        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.WriteBeginGroup();

            writer.WriteColumnName(this.ColumnName, args.TableName(this.Table));
            writer.WriteSpace();
            
            if (this.NotInList)
            {
                writer.Write(SqlConstants.NOT);
                writer.WriteSpace();
            }
            writer.Write(SqlConstants.IN);
            writer.WriteSpace();


            args.AssignAlias(this.SubQueryTable);
            
            var builder = new SqlSelectBuilder(this.SubQueryTable);
            builder.Columns.Add(new SqlColumn(this.SubQueryTable, this.SubQueryColumn));

            foreach (var filter in this.SubQueryFilters)
            {
                builder.Filters.Add(filter);
            }

            writer.WriteBeginGroup();
            builder.Render(writer, args);
            writer.WriteEndGroup();

            writer.WriteEndGroup();
        }
    }
}
