using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    public static class SqlJoinExtensions
    {
        public static void AddCross(this ICollection<ISqlJoin> collection, ISqlTable leftTable, ISqlTable rightTable)
        {
            collection.Add(new SqlJoin(leftTable, SqlJoinType.Cross, rightTable));
        }

        public static void AddInner(this ICollection<ISqlJoin> collection, ISqlTable leftTable, ISqlTable rightTable, params ISqlFilter[] conditions)
        {
            collection.Add(leftTable, SqlJoinType.Inner, rightTable, conditions);
        }

        public static void AddInner(this ICollection<ISqlJoin> collection, ISqlTable leftTable, ISqlTable rightTable, string key)
        {
            collection.Add(leftTable, SqlJoinType.Inner, rightTable, key);
        }

        //public static void AddInner(this ICollection<ISqlJoin> collection, ISqlTable leftTable, ISqlTable rightTable, string[] keyPairs)
        //{
        //    collection.Add(leftTable, SqlJoinType.Inner, rightTable, keyPairs);
        //}

        public static void AddLeft(this ICollection<ISqlJoin> collection, ISqlTable leftTable, ISqlTable rightTable, params ISqlFilter[] conditions)
        {
            collection.Add(leftTable, SqlJoinType.Left, rightTable, conditions);
        }

        public static void AddLeft(this ICollection<ISqlJoin> collection, ISqlTable leftTable, ISqlTable rightTable, string key)
        {
            collection.Add(leftTable, SqlJoinType.Left, rightTable, key);
        }

        //public static void AddLeft(this ICollection<ISqlJoin> collection, ISqlTable leftTable, ISqlTable rightTable, string[] keyPairs)
        //{
        //    collection.Add(leftTable, SqlJoinType.Left, rightTable, keyPairs);
        //}

        public static void Add(this ICollection<ISqlJoin> collection, ISqlTable leftTable, SqlJoinType joinType, ISqlTable rightTable, IEnumerable<ISqlFilter> conditions)
        {
            collection.Add(new SqlJoin(leftTable, joinType, rightTable, conditions));
        }

        public static void Add(this ICollection<ISqlJoin> collection, ISqlTable leftTable, SqlJoinType joinType, ISqlTable rightTable, string key)
        {
            var cond = new List<ISqlFilter>();
            //TODO: This sucks, Fix it
            cond.Add(new SqlColumnCompareFilter(leftTable, key, SqlOperator.Equal, rightTable));
            collection.Add(leftTable, joinType, rightTable, cond);
        }

        //public static void Add(this ICollection<ISqlJoin> collection, ISqlTable leftTable, SqlJoinType joinType, ISqlTable rightTable, string[] keyPairs)
        //{
        //    var cond = new List<ISqlFilter>();
        //    foreach (var keyPair in keyPairs)
        //    {
        //        cond.Add(new SqlColumnCompareFilter(leftTable, keyPair[0], SqlOperator.Equal, rightTable, keyPair[1]));
        //    }
        //    collection.Add(leftTable, joinType, rightTable, cond);
        //}

    }
}
