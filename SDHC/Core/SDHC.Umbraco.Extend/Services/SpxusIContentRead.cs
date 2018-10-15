using SpxUmbracoMember.Umbraco.Extend.Models.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace Services
{
  public delegate IContent SpxusGetIContentByKey<T>(T key);
  public delegate IEnumerable<IContent> SpxusGetIContentsByKey<T>(T key, Func<IContent, bool> where, bool? alive = null, string queryName = null);
  public delegate IEnumerable<IContent> SpxusGetIContentsByKeys<T>(IEnumerable<T> key, Func<IContent, bool> where, bool? alive = null, string queryName = null);
  public delegate IEnumerable<IContent> SpxusGetIContentsByString(string name, Func<IContent, bool> where, bool? alive = null, string queryName = null);

  public static class SpxusIContentRead
  {
    public static SpxusGetIContentByKey<Key> GetIContentById<Key, T>(
      Func<Key, IContent> serviceFunction, IDictionary<Key, T> cache) where T : CacheBase, new()
    {
      return (id) =>
      {
        return GetFunc.GetContentByKey<Key, IContent, T>(id, false, serviceFunction, cache);
      };
    }

    public static SpxusGetIContentsByKey<int> GetChildIContentByRootId<T>(
        Func<int, IEnumerable<IContent>> serviceFunction,
        Func<int, IDictionary<int, T>, IEnumerable<T>> cacheFunction,
        IDictionary<int, T> cache,
        Func<string, bool> checkQueryName,
        int expireMins = 30
        ) where T : CacheBase, new()
    {
      return (rootId, where, alive, queryName) =>
      {
        var queryNameCheck = G.Text(queryName);
        if (queryNameCheck == "")
        {
          queryNameCheck = $"icontent children {G.Text(rootId)}";
        }
        return GetFunc.GetChildContentsByKey<int, IContent, T>(
            serviceFunction, cacheFunction, b => b.Id, cache, rootId, alive, expireMins, where, checkQueryName(queryNameCheck));
      };
    }

    public static SpxusGetIContentsByKeys<int> GetIContentsByIds<T>(
        Func<IEnumerable<int>, IEnumerable<IContent>> serverFunction,
        Func<IEnumerable<int>, IDictionary<int, T>, IEnumerable<T>> cacheFunction,
        IDictionary<int, T> cache,
        Func<string, bool> checkQueryName,
        int expireMins = 30
        ) where T : CacheBase, new()
    {
      return (ids, where, alive, queryName) =>
      {
        var queryNameCheck = G.Text(queryName);
        if (queryNameCheck == "")
        {
          queryNameCheck = $"Ids content {String.Join(",", ids)}";
        }
        var result = GetFunc.GetContentByKeys<int, IContent, T>(ids, alive, serverFunction, cacheFunction, cache, b => b.Id, expireMins, checkQueryName(queryNameCheck));
        if (where == null)
          return result;
        return result.Where(where);
      };
    }
    public static SpxusGetIContentsByString GetIContentsByContentTypeName<T>(
        Func<string, Func<IEnumerable<IContent>>> serviceFunction,
        Func<string, Func<IDictionary<int, T>, IEnumerable<T>>> cacheFunction,
        IDictionary<int, T> cache,
        Func<string, bool> checkQueryName,
        int expireMins = 30
        ) where T : CacheBase, new()
    {
      return (name, where, alive, queryName) =>
      {
        var queryNameCheck = G.Text(queryName);
        if (queryNameCheck == "")
        {
          queryNameCheck = $"Icontent Type name {name}";
        }
        return GetFunc.GetChildContentsByQuery(serviceFunction(name), cacheFunction(name), b => b.Id, cache, where, alive, expireMins, checkQueryName(queryNameCheck));
      };
    }
  }
}
