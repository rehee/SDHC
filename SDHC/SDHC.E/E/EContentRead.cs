using SpxUmbracoMember.Umbraco.Extend.Models.Caches;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
namespace System
{
  public static partial class E
  {
    public static Dictionary<int, CacheIContent> MyCache { get; set; } = new Dictionary<int, CacheIContent>();
    public static Dictionary<int, CacheIPublishContent> IPublishContentCache { get; set; } = new Dictionary<int, CacheIPublishContent>();
    public static Dictionary<string, CacheQuery> MyQuerys { get; set; } = new Dictionary<string, CacheQuery>();
    public static Func<string, bool> MyQueryIsNew =
    query =>
    {
      if (MyQuerys.ContainsKey(query))
      {
        var isexpire = ((CacheBase)MyQuerys[query]).IsExpire(30);
        if (isexpire)
          MyQuerys[query].Time = DateTime.Now;
        return isexpire;
      }
      else
      {
        MyQuerys.Add(query, new CacheQuery());
        return true;
      }
    };

    public static Func<int, IEnumerable<IContent>> GetChildContentsByRootIdFromService(
      Func<int, IEnumerable<IContent>> serviceFunction)
    {
      return rootId => serviceFunction(rootId);
    }
    public static Func<int, IDictionary<int, CacheIContent>, IEnumerable<CacheIContent>> GetChildContentsByRootIdFromCache()
    {
      return (id, cache) =>
      {
        var copy = new Dictionary<int, CacheIContent>(cache);
        return copy.Where(b => b.Value.Content.ParentId == id).Select(b => b.Value);
      };
    }
    public static Func<string, Func<IEnumerable<IContent>>> MyGetIContentsByTypeFromService(IContentService service, IContentTypeService typeService)
    {
      return (typeName) =>
      {
        var type = typeService.GetContentType(typeName);
        return () =>
              {
                return service.GetContentOfContentType(type.Id);
              };
      };
    }
    public static Func<string, Func<IDictionary<int, CacheIContent>, IEnumerable<CacheIContent>>> MyGetContentByTypeFromCache()
    {
      return (typeName) =>
      {
        return (cachInput) =>
              {
                var cache = new Dictionary<int, CacheIContent>(cachInput);
                return cache
                          .Where(b => b.Value.Content.ContentType.Alias.Equals(typeName, StringComparison.InvariantCultureIgnoreCase))
                          .Select(b => b.Value);
              };
      };

    }
    public static Func<IEnumerable<int>, IEnumerable<IContent>> GetContentsByIdsFromService(IContentService service)
    {
      return ids =>
      {
        return service.GetByIds(ids);
      };
    }
    public static Func<IEnumerable<int>, IDictionary<int, CacheIContent>, IEnumerable<CacheIContent>> GetContentsByIdsFromCache()
    {
      return (ids, cacheInput) =>
      {
        var cache = new Dictionary<int, CacheIContent>(cacheInput);
        return cache.Where(b => ids.Contains(b.Key)).Select(b => b.Value);
      };
    }

    public static Func<IEnumerable<int>, IDictionary<int, CacheIPublishContent>, IEnumerable<CacheIPublishContent>> GetIPublishContentCacheByIdsFromCache()
    {
      return (ids, cacheInput) =>
      {
        var cache = new Dictionary<int, CacheIPublishContent>(cacheInput);
        return cache.Where(b => ids.Contains(b.Key)).Select(b => b.Value);
      };
    }
    public static Func<int, IDictionary<int, CacheIPublishContent>, IEnumerable<CacheIPublishContent>> GetChildIPublishContentsByRootIdFromCache()
    {
      return (id, cache) =>
      {
        var copy = new Dictionary<int, CacheIPublishContent>(cache);
        return copy.Where(b => b.Value.Content.Parent.Id == id).Select(b => b.Value);
      };
    }

    public static SpxusGetIContentByKey<int> MyGetContentByid { get; set; }
    public static SpxusGetIContentsByKey<int> MyGetChildByRootId { get; set; }
    public static SpxusGetIContentsByKeys<int> MyGetContentsByIds { get; set; }
    public static SpxusGetIContentsByString MyGetIContentsByType { get; set; }

    public static Func<int, IEnumerable<IPublishedContent>> GetChildIPublishContentsByRootIdFromService(
      Func<int, IEnumerable<IPublishedContent>> serviceFunction)
    {
      return rootId => serviceFunction(rootId);
    }

    public static Func<string, Func<IDictionary<int, CacheIPublishContent>, IEnumerable<CacheIPublishContent>>> MyGetIPublichContentByTypeFromCache()
    {
      return (typeName) =>
      {
        return (cachInput) =>
        {
          var cache = new Dictionary<int, CacheIPublishContent>(cachInput);
          return cache
                    .Where(b => b.Value.Content.DocumentTypeAlias.Equals(typeName, StringComparison.InvariantCultureIgnoreCase))
                    .Select(b => b.Value);
        };
      };

    }

    public static Func<string, Func<IEnumerable<IContent>>> MyGetIPublishContentsByTypeFromService(IContentService service, IContentTypeService typeService)
    {
      return (typeName) =>
      {
        var type = typeService.GetContentType(typeName);
        return () =>
        {
          return service.GetContentOfContentType(type.Id);
        };
      };
    }
    public static Func<string, Func<IDictionary<int, CacheIContent>, IEnumerable<CacheIContent>>> MyGetIPublishContentByTypeFromCache()
    {
      return (typeName) =>
      {
        return (cachInput) =>
        {
          var cache = new Dictionary<int, CacheIContent>(cachInput);
          return cache
                    .Where(b => b.Value.Content.ContentType.Alias.Equals(typeName, StringComparison.InvariantCultureIgnoreCase))
                    .Select(b => b.Value);
        };
      };

    }
    public static Func<IEnumerable<int>, IEnumerable<IContent>> GetIPublishContentsByIdsFromService(IContentService service)
    {
      return ids =>
      {
        return service.GetByIds(ids);
      };
    }
    public static Func<IEnumerable<int>, IDictionary<int, CacheIContent>, IEnumerable<CacheIContent>> GetIPublishContentsByIdsFromCache()
    {
      return (ids, cacheInput) =>
      {
        var cache = new Dictionary<int, CacheIContent>(cacheInput);
        return cache.Where(b => ids.Contains(b.Key)).Select(b => b.Value);
      };
    }
    public static SDHCGetIPublishedContentByKey<int> MyGetIPublichContentByid { get; set; }
    public static SDHCGetIPublishedContentsByKeys<int> MyGetIPublichContentsByIds { get; set; }
    public static SDHCGetIPublishedContentsByKey<int> MyGetIPublichContentsChildByRootId { get; set; }
    public static SDHCGetIPublishedContentByString MyGetIPublishContentsByType { get; set; }

    public static void InitMyContentGet(IContentService service, IContentTypeService typeService)
    {
      Func<int, IEnumerable<IContent>> getChildrenFromHelper = id =>
       {

         return service.GetChildren(id);
       };
      Func<int, IEnumerable<IPublishedContent>> getIPublishContentChildrenFromHelper = id =>
      {
        return E.Helper.TypedContent(id).Descendants();
      };
      MyGetContentByid = SpxusIContentRead.GetIContentById<int, CacheIContent>(service.GetById, MyCache);
      MyGetContentsByIds = SpxusIContentRead.GetIContentsByIds<CacheIContent>(service.GetByIds, GetContentsByIdsFromCache(), MyCache, MyQueryIsNew, 30);
      MyGetIContentsByType = SpxusIContentRead.GetIContentsByContentTypeName<CacheIContent>(MyGetIContentsByTypeFromService(service, typeService), MyGetContentByTypeFromCache(), MyCache, MyQueryIsNew, 30);
      MyGetChildByRootId = SpxusIContentRead.GetChildIContentByRootId<CacheIContent>(GetChildContentsByRootIdFromService(getChildrenFromHelper)
      , GetChildContentsByRootIdFromCache(), MyCache, MyQueryIsNew, 30);

      MyGetIPublichContentByid = SDHCIPublishedContentRead.GetIPublishContentById<int, CacheIPublishContent>(
        key => E.Helper.TypedContent(key), IPublishContentCache);
      MyGetIPublichContentsByIds = SDHCIPublishedContentRead.GetIPublishContentsByIds<int, CacheIPublishContent>(
        ids => E.Helper.TypedContent(ids), GetIPublishContentCacheByIdsFromCache(), b => b.Id, IPublishContentCache, MyQueryIsNew, 30);
      MyGetIPublichContentsChildByRootId = SDHCIPublishedContentRead.GetChildIPublishContentByRootId<int, CacheIPublishContent>(
        GetChildIPublishContentsByRootIdFromService(getIPublishContentChildrenFromHelper), GetChildIPublishContentsByRootIdFromCache(), b => b.Id, IPublishContentCache, MyQueryIsNew, 30);
      MyGetIPublishContentsByType = SDHCIPublishedContentRead.GetIPublishContentsByContentTypeName<int, CacheIPublishContent>(
        type => () => E.Helper.TypedContentAtRoot().DescendantsOrSelf(type), MyGetIPublichContentByTypeFromCache(), b => b.Id, IPublishContentCache, MyQueryIsNew, 30);
    }
  }
}
