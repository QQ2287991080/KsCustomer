using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.YiFang.Api.Help
{
  public  static class EnumerableExtensions
    {
        /// <summary>
        /// 扩展去重方法
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            //定义hashset存储
            HashSet<TKey> seenKeys = new HashSet<TKey>();//hashset特点：这是一组不包含重复元素的集合，并且其中存储的元素没有特定的顺序。
                                                         //迭代处理数据源
            foreach (TSource element in source)
            {
                //判断是否可以存入hashset
                if (seenKeys.Add(keySelector(element)))
                {
                    //返回去重后的数据
                    yield return element;
                }
            }
        }
        #region 等同于上面的方法
        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{

        //    HashSet<TKey> seenKeys = new HashSet<TKey>();
        //    List<TSource> list = new List<TSource>();
        //    foreach (TSource element in source)
        //    {

        //        if (seenKeys.Add(keySelector(element)))
        //        {
        //            list.Add(element);
        //        }
        //    }
        //    return list;
        //}
        #endregion
    }
}
