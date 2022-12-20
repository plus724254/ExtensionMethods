using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HuashanCRM.Extension
{
    public static class LinqExtension
    {
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }
        
        public static IQueryable<TSource> SkipIf<TSource>(this IQueryable<TSource> source, bool condition, int? skipValue)
        {
            if (condition)
                return source.Skip(skipValue.Value);
            else
                return source;
        }

        public static IEnumerable<TSource> SkipIf<TSource>(this IEnumerable<TSource> source, bool condition, int? skipValue)
        {
            if (condition)
                return source.Skip(skipValue.Value);
            else
                return source;
        }

        public static IQueryable<TSource> TakeIf<TSource>(this IQueryable<TSource> source, bool condition, int? takeValue)
        {
            if (condition)
                return source.Take(takeValue.Value);
            else
                return source;
        }

        public static IEnumerable<TSource> TakeIf<TSource>(this IEnumerable<TSource> source, bool condition, int? takeValue)
        {
            if (condition)
                return source.Take(takeValue.Value);
            else
                return source;
        }

        public static IQueryable<TSource> OrderByDescendingIf<TSource, TKey>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, TKey>> keySelector)
        {
            if (condition)
                return source.OrderByDescending(keySelector).AsQueryable();
            else
                return source;
        }
    }
}
