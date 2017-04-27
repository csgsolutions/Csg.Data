using System.Collections.Generic;

namespace Csg.Data.Sql
{
    public class SqlStatement
    {
        string _commandText;
        ICollection<DbParameterValue> _parameters;

        public SqlStatement()
        {
        }

        public SqlStatement(string commandText, ICollection<DbParameterValue> parameters)
        {
            _commandText = commandText;
            _parameters = parameters;
        }

        /// <summary>
        /// Gets or sets the SQL command text for the statement.
        /// </summary>
        public string CommandText
        {
            get
            {
                return _commandText;
            }
            set
            {
                _commandText = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of parameter values
        /// </summary>
        public ICollection<DbParameterValue> Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        /// <summary>
        /// Creates a command object initialized with the command text and parameters from the statement object.
        /// </summary>
        /// <param name="connection"></param>
        public System.Data.IDbCommand CreateCommand(System.Data.IDbConnection connection)
        {
            var cmd = connection.CreateCommand();
            this.SetupCommand(cmd);
            return cmd;
        }

        /// <summary>
        /// Configures the CommandText and Parameters on an existing IDbCommand.
        /// </summary>
        /// <param name="cmd"></param>
        public void SetupCommand(System.Data.IDbCommand cmd)
        {
            cmd.CommandText = this.CommandText;

            foreach (var param in this.Parameters)
            {
                param.AddToCommand(cmd);                
            }
        }
    }
}
