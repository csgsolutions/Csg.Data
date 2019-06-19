# Csg.Data
Tools for data access and database query generation.

## QueryBuilder Fluent API Examples

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