namespace Csg.Data
{
    public static partial class DbQueryBuilderExtensions
    {
        public class DbColumnMetadata
        {
            public string Name { get; set; }

            public System.Data.DbType DbType { get; set; } = System.Data.DbType.String;

            public int? Size { get; set; }

            public int? Scale { get; set; }
        }


    }
}
