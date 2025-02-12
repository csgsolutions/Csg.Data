using System.Data;

namespace Csg.Data.Sql;

public class DbParameterValue
{
    public string ParameterName { get; set; }

    public object Value { get; set; }

    public DbType DbType { get; set; }

    public int? Size { get; set; }

    public virtual IDbDataParameter AddToCommand(IDbCommand command)
    {
        var p = command.CreateParameter();
        p.ParameterName = ParameterName;
        p.Value = Value;
        p.DbType = DbType;
        if (Size.HasValue) p.Size = Size.Value;
        command.Parameters.Add(p);
        return p;
    }
}