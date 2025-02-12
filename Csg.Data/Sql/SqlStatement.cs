using System.Collections.Generic;
using System.Data;

namespace Csg.Data.Sql;

public class SqlStatement
{
    public SqlStatement()
    {
    }

    public SqlStatement(string commandText, ICollection<DbParameterValue> parameters)
    {
        CommandText = commandText;
        Parameters = parameters;
    }

    /// <summary>
    ///     Gets or sets the SQL command text for the statement.
    /// </summary>
    public string CommandText { get; set; }

    /// <summary>
    ///     Gets or sets the collection of parameter values
    /// </summary>
    public ICollection<DbParameterValue> Parameters { get; set; }

    /// <summary>
    ///     Creates a command object initialized with the command text and parameters from the statement object.
    /// </summary>
    /// <param name="connection"></param>
    public IDbCommand CreateCommand(IDbConnection connection)
    {
        var cmd = connection.CreateCommand();
        SetupCommand(cmd);
        return cmd;
    }

    /// <summary>
    ///     Configures the CommandText and Parameters on an existing IDbCommand.
    /// </summary>
    /// <param name="cmd"></param>
    public void SetupCommand(IDbCommand cmd)
    {
        cmd.CommandText = CommandText;

        foreach (var param in Parameters) param.AddToCommand(cmd);
    }
}