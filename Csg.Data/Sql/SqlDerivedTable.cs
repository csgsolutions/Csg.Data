namespace Csg.Data.Sql
{
    public class SqlDerivedTable : SqlTableBase
    {
        public SqlDerivedTable(string commandText) : base()
        {
            this.CommandText = commandText;
        }
                
        public string CommandText { get; set; }

        protected override void RenderInternal(Abstractions.ISqlTextWriter writer)
        {
            writer.Render(this);            
        }        
    }
}
