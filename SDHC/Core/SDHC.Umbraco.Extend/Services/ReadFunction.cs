using SpxUmbracoMember.Umbraco.Extend.Models.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
  public delegate void UpdateCache<T, K, C>(T key, K value, bool? alive, IDictionary<T, C> cache, out MethodResponse response) where C : CacheBase, new();
  public static class GetFunc
  {
    public static K GetContentByKey<T, K, C>(
        T key, bool? alive,
        Func<T, K> service,
        IDictionary<T, C> cache,
        UpdateCache<T, K, C> updateCacheFunction = null
        ) where C : CacheBase, new()
    {
      if (updateCacheFunction == null)
        updateCacheFunction = DefaultUpdateCache;
      K content;
      if (cache.ContainsKey(key))
        content = cache[key].GetContent<K>();
      else
        content = service(key);
      MethodResponse result;
      try
      {
        updateCacheFunction(key, content, alive, cache, out result);

      }
      catch
      {

      }
      return content;
    }

    public static IEnumerable<K> GetContentByKeys<T, K, C>(
        IEnumerable<T> keys, bool? alive, Func<IEnumerable<T>, IEnumerable<K>> serviceFunction, Func<IEnumerable<T>, IDictionary<T, C>, IEnumerable<C>> cacheFunction,
        IDictionary<T, C> cacheInput, Func<K, T> getKey, int expireMin = 30, bool isNewQuery = false, UpdateCache<T, K, C> updateCacheFunction = null
        ) where C : CacheBase, new()
    {
      if (updateCacheFunction == null)
        updateCacheFunction = DefaultUpdateCache;
      IEnumerable<K> content;
      if (isNewQuery)
        goto NewQuery;
      var cache = new Dictionary<T, C>(cacheInput);
      var cacheResult = cacheFunction(keys, cache);
      if (cacheResult.IsExpire(expireMin))
        goto NewQuery;
      content = cacheResult.Select(b => b.GetContent<K>());
      goto UpdateCache;
      NewQuery:
      content = serviceFunction(keys).ToList();
      UpdateCache:
      try
      {
        return content.Select(r => { MethodResponse response; updateCacheFunction(getKey(r), r, alive, cacheInput, out response); return r; }).ToList();
      }
      catch
      {
        return content;
      }

    }

    public static IEnumerable<K> GetChildContentsByKey<T, K, C>(
        Func<T, IEnumerable<K>> service,
        Func<T, IDictionary<T, C>, IEnumerable<C>> getFromCache,
        Func<K, T> getKey,
        IDictionary<T, C> cacheInput,
        T key,
        bool? alive,
        int expireMin,
        Func<K, bool> where = null, bool newQuery = false,
        UpdateCache<T, K, C> updateCacheFunction = null
        ) where C : CacheBase, new()
    {
      IEnumerable<K> result;
      if (updateCacheFunction == null)
        updateCacheFunction = DefaultUpdateCache;
      if (newQuery)
        goto GetFromService;
      IDictionary<T, C> cache = new Dictionary<T, C>(cacheInput);
      var resultIncache = getFromCache(key, cache);
      if (where != null)
        resultIncache = resultIncache.Where(b => where(b.GetContent<K>()));
      if (resultIncache.IsExpire(expireMin))
        goto GetFromService;

      result = resultIncache.Select(b => b.GetContent<K>());

      goto UpdateCache;

      GetFromService:
      result = service(key).ToList();
      UpdateCache:
      try
      {
        result.Select(r => { MethodResponse response; updateCacheFunction(getKey(r), r, alive, cacheInput, out response); return r; }).ToList();
        if (where != null)
          result = result.Where(where);
        return result;
      }
      catch
      {
        return result;
      }

    }

    public static IEnumerable<K> GetChildContentsByQuery<Key, K, C>(
        Func<IEnumerable<K>> getFromService,
        Func<IDictionary<Key, C>, IEnumerable<C>> getFromCache,
        Func<K, Key> getKey,
        IDictionary<Key, C> cacheInput,
        Func<K, bool> where = null,
        bool? alive = null,
        int expireMin = 30, bool newQuery = false,
        UpdateCache<Key, K, C> updateCacheFunction = null
        ) where C : CacheBase, new()
    {
      if (updateCacheFunction == null)
        updateCacheFunction = DefaultUpdateCache;
      var cache = new Dictionary<Key, C>(cacheInput);
      IEnumerable<K> result;
      if (newQuery)
        goto GetFromServer;
      var resultFromCache = getFromCache(cache);
      if (where != null)
        resultFromCache = resultFromCache.Where(b => where(b.GetContent<K>()));
      if (resultFromCache.IsExpire(expireMin))
        goto GetFromServer;

      result = resultFromCache.Select(b => b.GetContent<K>());
      goto UpdateCache;

      GetFromServer:
      result = getFromService().ToList();
      
      UpdateCache:
      try
      {
        result.Select(r =>
        {
          Key key = getKey(r);
          MethodResponse response;
          updateCacheFunction(key, r, alive, cacheInput, out response);
          return r;
        }).ToList();
        if (where != null)
          result = result.Where(where);
        return result;
      }
      catch
      {
        return result;
      }

    }

    public static void DefaultUpdateCache<TKey, TValue, TCache>(
      TKey key, TValue value, bool? alive, IDictionary<TKey, TCache> cache, out MethodResponse response)
        where TCache : CacheBase, new()
    {
      response = new MethodResponse();
      try
      {
        if (value == null)
          goto RemoveCache;
        if (cache.ContainsKey(key))
        {
          var newValue = new TCache();
          newValue.Set(value, alive);
          cache[key] = newValue;
        }
        else
        {
          var newValue = new TCache();
          newValue.Set(value, alive);
          cache.Add(key, newValue);
        }
        response.IsOk = true;
        return;
        RemoveCache:
        if (cache.ContainsKey(key))
        {
          cache.Remove(key);
        }
        response.IsOk = true;
      }
      catch (Exception ex)
      {
        response.Error = ex.Message;
      }

    }


  }
}
