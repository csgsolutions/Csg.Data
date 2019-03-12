using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Sql
{
    public class SqlTextWriter2 : ISqlTextWriter
    {
        private int _indent = 0;
        private System.IO.TextWriter _writer;     
        
        bool UseQuotedIdentifiers { get; set; }

        protected virtual void WriteIndent()
        {
            if (_indent > 0)
            {
                this.Write(new string('\t', _indent));
            }
        }

        protected virtual void WriteSpace()
        {
            this.Write(" ");
        }
             


        public virtual void WriteIdentifier(string name)
        {
            Write(SqlTextWriter.FormatSqlServerIdentifier(name, this.UseQuotedIdentifiers));
        }

        public void WriteColumnName(string columnName)
        {
            if (columnName.Equals("*"))
            {
                Write("*");
            }
            else
            {
                WriteIdentifier(columnName);
            }
        }

        public void WriteColumnName(string columnName, string tableName)
        {
            WriteTableName(tableName);
            Write(SqlConstants.DOT);
            WriteColumnName(columnName);
        }

        public void WriteColumnName(string columnName, string tableName, string alias)
        {
            WriteTableName(tableName);
            Write(SqlConstants.DOT);
            WriteColumnName(columnName);

            if (!string.IsNullOrEmpty(alias) && !alias.Equals(columnName))
            {
                WriteSpace();
                Write(SqlConstants.AS);
                WriteSpace();
                WriteIdentifier(columnName);
            }
        }

        public void WriteTableName(string tableName)
        {
            this.WriteTableName(tableName, null);
        }

        public void WriteTableName(string tableName, string alias)
        {
            WriteIdentifier(tableName);
            if (!string.IsNullOrEmpty(alias))
            {
                WriteSpace();
                Write(SqlConstants.AS);
                WriteSpace();
                WriteIdentifier(alias);
            }
        }

        public void WriteParameterReference(string parameterName)
        {
            this.Write("@");
            this.Write(parameterName);
        }

        public void WriteBeginGroup()
        {
            Write("(");
        }

        public void WriteEndGroup()
        {
            Write(")");
        }

        public void WriteAggregateColumn(string columnName, string tableName, SqlAggregate aggregateType, string outputName)
        {
            Write(ConvertSqlAggregateToString(aggregateType));
            WriteBeginGroup();
            if (aggregateType == SqlAggregate.CountDistinct)
            {
                Write(SqlConstants.DISTINCT);
                WriteSpace();
            }
            WriteColumnName(columnName, tableName);
            WriteEndGroup();
            WriteSpace();
            Write(SqlConstants.AS);
            WriteSpace();
            WriteColumnName(outputName);
        }

        public void WriteAggregate(string columnName, string tableName, SqlAggregate aggregateType)
        {
            Write(ConvertSqlAggregateToString(aggregateType));
            WriteBeginGroup();
            WriteColumnName(columnName, tableName);
            WriteEndGroup();
        }

        public static string ConvertSqlOperatorToString(SqlOperator oper)
        {
            switch (oper)
            {
                case SqlOperator.Equal: return "=";
                case SqlOperator.GreaterThan: return ">";
                case SqlOperator.GreaterThanOrEqual: return ">=";
                case SqlOperator.LessThan: return "<";
                case SqlOperator.LessThanOrEqual: return "<=";
                case SqlOperator.NotEqual: return "<>";
                default: throw new InvalidOperationException("Invalid operator");
            }
        }

        public static string ConvertSqlAggregateToString(SqlAggregate aggregateType)
        {
            switch (aggregateType)
            {
                case SqlAggregate.Minimum: return "MIN";
                case SqlAggregate.Maximum: return "MAX";
                case SqlAggregate.Sum: return "SUM";
                case SqlAggregate.Count: return "COUNT";
                case SqlAggregate.CountDistinct: return "COUNT";
                case SqlAggregate.Average: return "AVG";
                case SqlAggregate.StDev: return "STDEV";
                default: throw new InvalidOperationException("Invalid aggregate");
            }
        }

        #region ISqlTextWriter

        public void Write(string value)
        {
            _writer.Write(value);
        }

        public void WriteLine(string value)
        {
            this.WriteIndent();
            this.Write(value);
            this.Write(Environment.NewLine);
        }

        public void PopIndent()
        {
            _indent = Math.Max(0, _indent - 1);
        }

        public void PushIndent()
        {
            _indent++;
        }

        public void Render(SqlColumn column, SqlBuildArguments args)
        {
          
        }

        public void Render(ISqlTable table, SqlBuildArguments args)
        {
            table.Compile(args);   
        }

        public void WriteFragment(SqlColumn column, SqlBuildArguments args, bool valueOnly = false)
        {
            string tableName = args.TableName(column.Table);
            if (column.Aggregate == SqlAggregate.None)
            {
                WriteColumnName(column.ColumnName, tableName);
            }
            else
            {
                Write(ConvertSqlAggregateToString(aggregateType));
                WriteBeginGroup();
                if (column.Aggregate == SqlAggregate.CountDistinct)
                {
                    Write(SqlConstants.DISTINCT);
                    WriteSpace();
                }
                WriteColumnName(columnName, tableName);
                WriteEndGroup();
            }

            if (column.Aggregate == SqlAggregate.None)
            {
                this.WriteColumnName(column.ColumnName, args.TableName(column.Table), column.Alias);
            }
            else
            {
                //writer.WriteAggregateColumn(column.ColumnName, args.TableName(column.Table), column.Aggregate, column.Alias);
            }
        }

        public void WriteFragment(SqlTable table, SqlBuildArguments args)
        {
            throw new NotImplementedException();
        }

        public void WriteFragment(SqlCompareFilter filter, SqlBuildArguments args)
        {
            throw new NotImplementedException();
        }

        public void WriteFragment(SqlColumnCompareFilter filter, SqlBuildArguments args)
        {
            throw new NotImplementedException();
        }

        #endregion  
    }
}
