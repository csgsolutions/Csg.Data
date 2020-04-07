namespace Csg.Data.Sql
{
    public class SqlDerivedTable : SqlTableBase
    {
        public SqlDerivedTable(string commandText) : base()
        {
            this.CommandText = TrimCommandText(commandText);
        }
                
        public string CommandText { get; set; }

        protected override void RenderInternal(Abstractions.ISqlTextWriter writer)
        {
            writer.Render(this);            
        }        

        public static string TrimCommandText(string commandText)
        {
            return commandText.TrimEnd(new char[] { '\r', '\n', ';', ' ', '\t'});
        }
    }
}
