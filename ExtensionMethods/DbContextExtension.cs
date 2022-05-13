using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace HuashanCRM.Extension
{
    public static class DbContextExtension
    {
        /// <summary>
        /// 從本地快取移除實體
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="entity"></param>
        public static void Detach<T>(this DbContext db, T entity) where T : class
        {
            var context = ((IObjectContextAdapter)db).ObjectContext;
            context.Detach(entity);
        }

        /// <summary>
        /// 取得實體的主鍵
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetEntityKeys<T>(this DbContext db) where T : class
        {
            var context = ((IObjectContextAdapter)db).ObjectContext;
            var keys = context.CreateObjectSet<T>().EntitySet.ElementType.KeyProperties.Select(x => x.Name);

            return keys;
        }

        private static Expression<Func<T, bool>> GetFindExpression<T>(T entity, IEnumerable<string> keys) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var keyExpressions = keys.Select(x =>
            {
                var member = Expression.PropertyOrField(parameter, x);
                var entityValue = typeof(T).GetProperty(x).GetValue(entity);
                var equalExpression = Expression.Equal(member, Expression.Constant(entityValue));

                return equalExpression;
            }).ToList();

            if (keys.Count() == 1)
            {
                return Expression.Lambda<Func<T, bool>>(keyExpressions[0], new[] { parameter });
            }

            var andExpression = Expression.AndAlso(keyExpressions[0], keyExpressions[1]);

            for (var i = 2; i < keyExpressions.Count; i++)
            {
                andExpression = Expression.AndAlso(andExpression, keyExpressions[i]);
            }
            return Expression.Lambda<Func<T, bool>>(andExpression, new[] { parameter });
        }

        /// <summary>
        /// 從本地快取尋找實體
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T FindLocal<T>(this DbContext db, T entity) where T : class
        {
            var entityKeys = db.GetEntityKeys<T>();
            var findExpression = GetFindExpression<T>(entity, entityKeys).Compile();

            return db.Set<T>().Local.FirstOrDefault(findExpression);
        }

        /// <summary>
        /// 若本地快取存在實體，將他移除快取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="entity"></param>
        //public static void DetachWhenExist<T>(this DbContext db, T entity) where T : class
        //{
        //    var localEntity = db.FindLocal(entity);
        //    if (localEntity != null)
        //    {
        //        db.Detach(localEntity);
        //    }
        //}

        //public static void DetachRangeWhenExist<T>(this DbContext db, IEnumerable<T> entities) where T : class
        //{
        //    var entityTemplate = entities.FirstOrDefault();
        //    var entityKeys = db.GetEntityKeys<T>();
        //    var findExpression = GetFindExpression(entityTemplate, entityKeys).Compile();

        //    foreach (var entity in entities)
        //    {
        //        var localEntity = db.FindLocal(entityTemplate, entityKeys, findExpression);

        //        if (localEntity != null)
        //        {
        //            db.Detach(localEntity);
        //        }
        //    }
        //}

        /// <summary>
        /// 若本地快取存在實體，將他移除快取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="entity"></param>
        public static void DetachWhenExist<T>(this DbContext db, T entity) where T : class
        {
            var objContext = ((IObjectContextAdapter)db).ObjectContext;
            var objSet = objContext.CreateObjectSet<T>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);
            var exists = objContext.TryGetObjectByKey(entityKey, out var foundEntity);
            if (exists)
            {
                objContext.Detach(foundEntity);
            }
        }

        public static void DetachRangeWhenExist<T>(this DbContext db, IEnumerable<T> entities) where T : class
        {
            var objContext = ((IObjectContextAdapter)db).ObjectContext;
            var objSet = objContext.CreateObjectSet<T>();

            foreach(var entity in entities)
            {
                var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);
                if (objContext.TryGetObjectByKey(entityKey, out var foundEntity))
                {
                    objContext.Detach(foundEntity);
                }
            }
        }
    }
}
