using System.Data;

namespace Csg.Data;

public static partial class DbQueryBuilderExtensions
{
    /// <summary>
    ///     Metadata about a data column
    /// </summary>
    public class DbColumnMetadata
    {
        /// <summary>
        ///     Gets or sets the column name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the column's data type
        /// </summary>
        public DbType DbType { get; set; } = DbType.String;

        /// <summary>
        ///     Gets or sets the column's size
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        ///     Gets or sets the column's scale
        /// </summary>
        public int? Scale { get; set; }
    }
}