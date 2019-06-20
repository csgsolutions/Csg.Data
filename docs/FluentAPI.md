## QueryBuilder Fluent API

### Note about parameterization and SQL text
Unless otherwise noted, the literal values shown in the example rendered queries would always
be rendered as parameter references. The parameter values are shown as literals for ease of 
understanding. In addition, the example SQL output is not exactly what is rendered, but is simplified
for demonstration purposes.

### QueryBuilder Extension Method
Querybuilder is an extension method for IDbConnection, so it can be used
with most DB connection types, such as SqlConnection, DbConnection, 
etc. by adding a ```using Csg.Data;``` in your code file.
```csharp
using Csg.Data;
...
void SomeMethod()
{
    IDbConnection connection = ...;
    // QueryBuilder is an extension method for IDbConnection
    var query = connection.QueryBuilder(...);'
...
}
```

### Create a querybuilder using a Table or View Name

```csharp
var query = connection.QueryBuilder("dbo.Product");
```

### Create a querybuilder using a SQL query

```csharp
var query = connection.QueryBuilder("SELECT ProductID, Name FROM dbo.Product");
```

### Add a simple equality filter
```csharp
var query = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldEquals<bool>("IsActive", true));
// SELECT * FROM dbo.Product WHERE IsActive=1;
```
Which is equivalent to
```csharp
var query = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldMatch<bool>("IsActive", SqlOperator.Equal, true));
// SELECT * FROM dbo.Product WHERE IsActive=1;
```

### Add a comparison filter
```csharp
var query1 = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldMatch<decimal>("Price", SqlOperator.GreaterThan, 100M));
// SELECT * FROM dbo.Product WHERE Price > 100;
var query2 = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldMatch<decimal>("Price", SqlOperator.LessThanOrEqual, 100M));
// SELECT * FROM dbo.Product WHERE Price <= 100;
```

### Add a between filter
```csharp
var query = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldBetween<decimal>("Price", 100, 200))   
// SELECT * FROM dbo.Product WHERE Price >= 100 AND Price <= 200; 
```

### Add a NULL or NOT NULL filter
```csharp
var query1 = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldIsNull("ExpirationDate"))   
// SELECT * FROM dbo.Product WHERE ExpirationDate IS NULL;
var query2 = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldIsNotNull("ExpirationDate"))   
// SELECT * FROM dbo.Product WHERE ExpirationDate IS NOT NULL;
```

### String Comparisons
It is important to specify IsAnsi: true when using FieldEquals(), FieldMatch() or StringMatch() with string
values defined in the database as char or varchar so the correct parameter data types are generated. This
can affect the performance of the executed query dramatically.
```csharp
var query = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldEquals<string>("NameNVarChar", "Red Apple"));

var query = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldEquals<string>("NameVarChar", "Red Apple", isAnsi: true));
```

To generate pattern matching (LIKE) filters, use the .StringMatch() method. The SqlWildcardDecoration
enum parameter indicates whether the given value should be automatically wrapped 
with SQL wildcard characters as %value% value%, %value or not at all. If the input string
contains asterisk (*) or question marks (?) they will be replaced with the SQL 
equivalent of % and _ respectively.
```csharp
var query1 = connection.QueryBuilder("dbo.Product")
    .Where(x => x..StringMatch("Description", SqlWildcardDecoration.Contains, "Fruit"));
// SELECT * FROM dbo.Product WHERE Description LIKE '%Fruit%';
var query2 = connection.QueryBuilder("dbo.Product")
    .Where(x => x..StringMatch("Description", SqlWildcardDecoration.BeginsWith, "Fruit"));
// SELECT * FROM dbo.Product WHERE Description LIKE 'Fruit%';
var query3 = connection.QueryBuilder("dbo.Product")
    .Where(x => x..StringMatch("Description", SqlWildcardDecoration.None, "Fruit"));
// SELECT * FROM dbo.Product WHERE Description LIKE 'Fruit';
```

### Manually specify a parameter data type
Type mapping from .NET types to System.Data.DbType is performed when creating parameters, however sometimes it may be 
necessary to specify the type or size of the generated parameter manually. FieldEquals(), FieldMatch() and FieldBetween() support specifying
both the type and size, and StringMatch supports specifying the length. 
```csharp
var query = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldEquals("Code", 100, dbType: System.Data.DbType.Byte));
// SELECT * FROM dbo.Product WHERE Code = @Code;
// @Code would be declared as tinyint in SQL Server

var query = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldEquals<string>("Code", "AB", dbType: System.Data.DbType.AnsiStringFixedLength, length: 2)));
// SELECT * FROM dbo.Product WHERE Code = @Code;
// @Code would be declared as char(2) in SQL Server
```

### List filters
WHERE IN (list,of,values) and WHERE NOT IN (list,of,values) filters can be built using FieldIn() and FieldNotIn()

```csharp
var query1 = connection.QueryBuilder("dbo.Product")
    .Where(x => .FieldIn("CategoryID", new int[] { 10, 20, 30 }));
// SELECT * FROM dbo.Product WHERE CategoryID IN (10,20,30);

var query2 = connection.QueryBuilder("dbo.Product")
    .Where(x => .FieldNotIn("CategoryID", new int[] { 10, 20, 30 }));
// SELECT * FROM dbo.Product WHERE CategoryID NOT IN (10,20,30);
```

In the above examples, the values in the comma seperated list in the generated SQL text would be parameterized in
the form of (@p0, @p1, @p2) etc. If you want to generate literal values for numeric data types, specify the useLiteralNumbers argument. 
This feature is currently only supported for Int16, Int32, and Int64 data types. All other data types will be rendered parameterized
regardless of the useLiteralNumbers value. Boolean values are always rendered as literal 1 and 0.
```csharp
var query1 = connection.QueryBuilder("dbo.Product")
    .Where(x => .FieldIn("CategoryID", new int[] { 10, 20, 30 }, useLiteralNumbers: true));
// SELECT * FROM dbo.Product WHERE CategoryID IN (10, 20, 30);

var query2 = connection.QueryBuilder("dbo.Product")
    .Where(x => .FieldIn<bool>("CategoryID", new bool[] { true, false, true }));
// SELECT * FROM dbo.Product WHERE CategoryID IN (1, 0, 1);
```

### Build and execute a query

```csharp
var cmd = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldEquals<bool>("IsActive", true))
    .CreateCommand();

/*
Generates an IDbCommand instance with the CommandText property and parameters collection
initialized similiar to:
SELECT ProductID, Name FROM dbo.Product WHERE IsActive=1;
*/
```

### Render a query to a string and a parameter collection
If you need the SQL text and parameter values, render the query with Render()

```csharp
var statement = connection.QueryBuilder("dbo.Product").Render();
// statement.CommandText is a string contaiing the rendered SQL statement.
// statement.Parameters is a collection of parameter value objects.
```

builder.CreateCommand()

### Query Cloning
The fluent API Where() and WhereAny() methods create a clones of the original query
before making modifications. Multiple forks of the same query can be created by assigning
the results of Where() and WhereAny() to variables.

```csharp
var query = connection.QueryBuilder("dbo.Product")
    .Where(x => x.FieldEquals<bool>("IsActive", true));

var query1 = query.Where(x => x.FieldEquals<string>("Color", "Red"));
//  SELECT * FROM dbo.Product WHERE IsActive=1 AND Color='Red;

var query2 = query.Where(x => x.FieldEquals<string>("IsActive", "Green"));
// SELECT * FROM dbo.Product WHERE IsActive=1 AND Color='Green';

var query3 = query1.Where(x => x.FieldEquals<string>("Category", "Veggies"));
// SELECT * FROM dbo.Product WHERE IsActive=1 AND Color='Red' AND Category='Veggies' ;

// adding additional filters to the same field doesn't modify the filter already in place
var query4 = query1.Where(x => x.FieldEquals<bool>("IsActive", false));
// SELECT * FROM dbo.Product WHERE IsActive=1 AND Color='Red' AND Category='Veggies' AND IsActive=0;
```

### 

### Filter a query using a collection of criteria

Joining each set of criteria using OR. Tuples are used for this example, but the collection can be of any type.

```csharp
var query = connection.QueryBuilder("dbo.Product");

var listOfThings1 = new string[] { "a", "b", "c" };
var listOfThings2 = new string[] { "d", "e", "f" };

var listOfCriteria = new Tuple<int, int, string[]>[]
{
    new Tuple<int,int,string[]>(123, 456, listOfThings1),
    new Tuple<int,int,string[]>(123, 456, listOfThings2)
};

query = query.Where(x => x.FieldEquals("Foo", "Bar"));
query = query.WhereAny(
    listOfCriteria,
    (x, f, i) => x.FieldEquals("ProductCategoryID", f.Item1)
            .FieldEquals("SupplierID", f.Item2)
            .FieldIn("ThingName", f.Item3)
);


/* Query Result (formatted manually)
SELECT * 
FROM [dbo].[Product] AS [t0] 
WHERE ([t0].[Foo] = @p0)
    AND (
        (([t0].[ProductCategoryID]=@p1) AND ([t0].[SupplierID]=@p2) AND ([t0].[ThingName] IN (@p3,@p4,@p5))) 
        OR (([t0].[ProductCategoryID]=@p6) AND ([t0].[SupplierID]=@p7) AND ([t0].[ThingName] IN (@p8,@p9,@p10)))
    );
*/
```