using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Extensions
{
    public class AutoMapperExtensions
    {
        /// <summary>
        /// 生成lambda表达式，类似：
        /// p => new TResult(){
        /// xxx = p.xxx,
        /// yyy= p.yyy
        /// }
        ///
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Expression<Func<TSource, TResult>> MapExp<TSource, TResult>() where TResult : class, new()
        {

            var props = typeof(TResult).GetProperties();
            var sourceProps = typeof(TSource).GetProperties();
            var sourceParam = Expression.Parameter(typeof(TSource), "p");

            var newExp = Expression.New(typeof(TResult));

            List<MemberBinding> memberBindings = new List<MemberBinding>();
            foreach (var p in props)
            {
                var resultPropertyName = p.Name;
                var mapProperty = (MapPropertyAttribute)p.GetCustomAttribute(typeof(MapPropertyAttribute));
                if (mapProperty != null && !string.IsNullOrEmpty(mapProperty.Name))
                {
                    resultPropertyName = mapProperty.Name;
                }

                //如果source类型存在结果类型的属性名才添加，以结果类型的属性为基准
                if (sourceProps.Any(a => a.Name == resultPropertyName))
                {
                    var propertyExp = Expression.PropertyOrField(sourceParam, resultPropertyName);
                    memberBindings.Add(Expression.Bind(typeof(TResult).GetMember(p.Name)[0], propertyExp));
                }
            }
            var body = Expression.MemberInit(newExp, memberBindings);
            var exp = Expression.Lambda<Func<TSource, TResult>>(body, sourceParam);
            return exp;

        }
    }
}
