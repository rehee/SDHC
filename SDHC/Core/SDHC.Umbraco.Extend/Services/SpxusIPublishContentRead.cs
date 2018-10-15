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
  public delegate IPublishedContent SDHCGetIPublishedContentByKey<T>(T key);
  public delegate IEnumerable<IPublishedContent> SDHCGetIPublishedContentsByKey<T>(T key, Func<IPublishedContent, bool> where, bool? alive = null, string queryName = null);
  public delegate IEnumerable<IPublishedContent> SDHCGetIPublishedContentsByKeys<T>(IEnumerable<T> key, Func<IPublishedContent, bool> where, bool? alive = null, string queryName = null);
  public delegate IEnumerable<IPublishedContent> SDHCGetIPublishedContentByString(string name, Func<IPublishedContent, bool> where, bool? alive = null, string queryName = null);

  public static class SDHCIPublishedContentRead
  {
    public static SDHCGetIPublishedContentByKey<Key> GetIPublishContentById<Key, T>(
      Func<Key, IPublishedContent> serviceFunction, IDictionary<Key, T> cache) where T : CacheBase, new()
    {
      return (id) =>
      {
        return GetFunc.GetContentByKey<Key, IPublishedContent, T>(id, false, serviceFunction, cache);
      };
    }

    public static SDHCGetIPublishedContentsByKey<Key> GetChildIPublishContentByRootId<Key, T>(
        Func<Key, IEnumerable<IPublishedContent>> serviceFunction,
        Func<Key, IDictionary<Key, T>, IEnumerable<T>> cacheFunction,
        Func<IPublishedContent, Key> getKeyFunction,
        IDictionary<Key, T> cache,
        Func<string, bool> checkQueryName,
        int expireMins = 30
        ) where T : CacheBase, new()
    {
      return (rootId, where, alive, queryName) =>
      {
        var queryNameCheck = G.Text(queryName);
        if (queryNameCheck == "")
        {
          queryNameCheck = $"children {G.Text(rootId)}";
        }
        return GetFunc.GetChildContentsByKey<Key, IPublishedContent, T>(
            serviceFunction, cacheFunction, getKeyFunction, cache, rootId, alive, expireMins, where, checkQueryName(queryNameCheck));
      };
    }


    public static SDHCGetIPublishedContentsByKeys<Key> GetIPublishContentsByIds<Key, T>(
        Func<IEnumerable<Key>, IEnumerable<IPublishedContent>> serverFunction,
        Func<IEnumerable<Key>, IDictionary<Key, T>, IEnumerable<T>> cacheFunction,
        Func<IPublishedContent, Key> getKeyFunction,
        IDictionary<Key, T> cache,
        Func<string, bool> checkQueryName,
        int expireMins = 30
        ) where T : CacheBase, new()
    {
      return (ids, where, alive, queryName) =>
      {
        var queryNameCheck = G.Text(queryName);
        if (queryNameCheck == "")
        {
          queryNameCheck = String.Join(",", ids);
        }
        var result = GetFunc.GetContentByKeys<Key, IPublishedContent, T>(
          ids, alive, serverFunction, cacheFunction, cache, getKeyFunction, expireMins, checkQueryName(queryNameCheck));
        if (where == null)
          return result;
        return result.Where(where);
      };
    }

    public static SDHCGetIPublishedContentByString GetIPublishContentsByContentTypeName<Key, T>(
        Func<string, Func<IEnumerable<IPublishedContent>>> serviceFunction,
        Func<string, Func<IDictionary<Key, T>, IEnumerable<T>>> cacheFunction,
        Func<IPublishedContent, Key> getKeyFunction,
        IDictionary<Key, T> cache,
        Func<string, bool> checkQueryName,
        int expireMins = 30
        ) where T : CacheBase, new()
    {
      return (name, where, alive, queryName) =>
      {
        var queryNameCheck = G.Text(queryName);
        if (queryNameCheck == "")
        {
          queryNameCheck = $"documentType {name}";
        }
        return GetFunc.GetChildContentsByQuery<Key, IPublishedContent, T>(
          serviceFunction(name), cacheFunction(name), getKeyFunction, cache, where, alive, expireMins, checkQueryName(queryNameCheck));
      };
    }
  }
}
