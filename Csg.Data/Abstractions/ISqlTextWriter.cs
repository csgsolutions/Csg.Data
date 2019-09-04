using Csg.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Abstractions
{
    /// <summary>
    /// Represents a class that is responsible for rendering query components to the appropriate SQL representation.
    /// </summary>
    public interface ISqlTextWriter
    {

        ISqlProvider Provider { get; }

        SqlBuildArguments BuildArguments { get; }

        
        bool Format { get; set; }

        void Render(SqlColumn src);

        void RenderValue(SqlColumn src);

        void RenderValue(SqlExpressionSelectColumn src);

        void RenderValue<T>(SqlLiteralColumn<T> src);

        void Render(SqlColumnCompareFilter src);

        void Render(SqlCompareFilter src);

        void Render(SqlCountFilter src);

        void Render(SqlDateTimeFilter src);

        void Render(SqlExistFilter src);

        void Render(SqlExpressionSelectColumn src);

        void Render(SqlFilterCollection src);

        void Render(SqlJoin src);

        void Render(SqlListFilter src);

        void Render<T>(SqlLiteralColumn<T> src);

        void Render(SqlNullFilter src);

        void Render(SqlOrderColumn src);

        void Render(SqlParameterFilter src);

        void Render(SqlRankColumn src);

        void Render(SqlRawFilter src);

        void Render(SqlStringMatchFilter src);

        void Render(SqlSubQueryFilter src);

        void Render(SqlTable src);

        void Render(SqlDerivedTable src);

        void Render(SqlSelectBuilder selectBuilder, bool wrapped = false, bool aliased = false);

        void WriteEndStatement();

        string ToString();
    }
}
