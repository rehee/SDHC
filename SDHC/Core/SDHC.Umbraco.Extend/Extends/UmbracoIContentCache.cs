using SpxUmbracoMember.Umbraco.Extend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Umbraco.Core.Services
{
  public delegate IContent UpdateIContentCache(IContent content, bool? alive, Dictionary<int, ContentCacheAndTime> pageCache);

  public static partial class UmbracoContentExtend
  {
    public static Dictionary<int, ContentCacheAndTime> PageCache { get; set; } = new Dictionary<int, ContentCacheAndTime>();
    public static GetContentValue<T> GetContentValue<T>(IContentService service,
        Dictionary<int, ContentCacheAndTime> pageCache = null,
        UpdateIContentCache updateCatch = null, Action cacheCheck = null)
    {
      return (int pageId, string key, bool? keepAlive) =>
      {
        try
        {
          return GetContentById(pageId, service, pageCache, updateCatch, cacheCheck)
              .GetValueNull<T>(key);
        }
        catch (Exception ex)
        {
          var e = ex;
          return default(T);
        }

      };
    }

    public static GetContentValue GetContentValue(IContentService service, Dictionary<int, ContentCacheAndTime> pageCache = null, UpdateIContentCache updateCatch = null, Action cacheCheck = null)
    {
      
      return (int pageId, string key, bool? alive) =>
      {
        try
        {
          return GetContentById(pageId, service, pageCache, updateCatch, cacheCheck);
        }
        catch (Exception ex)
        {
          return null;
        }

      };
    }

    public static GetContent GetContent(IContentService service, Dictionary<int, ContentCacheAndTime> pageCache = null, UpdateIContentCache updateCatch = null, Action cacheCheck = null)
    {
      return (int pageId, bool? alive) =>
      {
        return GetContentById(pageId, service, pageCache, updateCatch, cacheCheck);
      };
    }

    public static IContent UpdateICache(IContent content, bool? alive, Dictionary<int, ContentCacheAndTime> pageCache)
    {
      try
      {
        if (pageCache == null)
          goto Return;
        if (pageCache.ContainsKey(content.Id))
        {
          pageCache[content.Id].Time = DateTime.Now;
          pageCache[content.Id].Content = content;
          if (alive != null)
            PageCache[content.Id].KeepAlive = (bool)alive;
        }
        else
        {
          pageCache.Add(content.Id,
              new ContentCacheAndTime()
              {
                Time = DateTime.Now,
                Content = content,
                KeepAlive = alive == null ? false : (bool)alive
              });
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      Return:
      return content;
    }

    public static IContent GetContentById(int id, IContentService service,
        Dictionary<int, ContentCacheAndTime> pageCache = null,
        UpdateIContentCache updateCatch = null,
        Action cacheCheck = null)
    {
      if (pageCache == null)
        pageCache = PageCache;
      IContent content = null;
      try
      {
        if (cacheCheck == null)
        {
          cacheCheck = DefaultCacheCheck(pageCache);
        }
        if (updateCatch == null)
        {
          updateCatch = UpdateICache;
        }
        if (!PageCache.ContainsKey(id))
        {
          content = updateCatch(service.GetById(id), false, pageCache);
        }
        else
        {
          content = updateCatch(pageCache[id].Content, pageCache[id].KeepAlive, pageCache);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      return content;
    }


    public static CheckPageCachePerMin CheckPageCache { get; set; } =
    (int maxMins, int cacheTime, Dictionary<int, ContentCacheAndTime> pageCache) =>
    {
      System.Threading.Tasks.Task t = System.Threading.Tasks.Task.Run(
              () =>
            {
              int count = 0;
              do
              {
                pageCache.Keys.ToList().ForEach(b =>
                {
                  if (pageCache[b].KeepAlive == false && pageCache[b].Time.AddMinutes(cacheTime) < DateTime.Now)
                  {
                    pageCache.Remove(b);
                  }
                });
                count++;
                System.Threading.Thread.Sleep(60 * 1000);
                if (pageCache.Count == 0)
                {
                  break;
                }
              } while (count <= maxMins);
              ContentCacheAndTime.IsPageCacheUsad = false;
            });
      t.Start();
    };
    public static Action DefaultCacheCheck(Dictionary<int, ContentCacheAndTime> pageCache)
    {
      return () =>
      {
        if (!ContentCacheAndTime.IsPageCacheUsad)
        {
          ContentCacheAndTime.IsPageCacheUsad = true;
          try
          {
            CheckPageCache(ContentCacheAndTime.CacheCheckMaxTime, ContentCacheAndTime.CacheTimeMinute, pageCache);
          }
          catch
          {

          }
        }
      };
    }

    public static IEnumerable<IContent> GetContentsByIds(
        IContentService services, IEnumerable<int> ids = null, int rootId = -1, int cacheTime = 60,
        Func<IContent, bool> where = null,
        bool? alive = null, Dictionary<int, ContentCacheAndTime> pageCache = null, UpdateIContentCache updateCatch = null, Action cacheCheck = null)
    {
      if (pageCache == null)
        pageCache = PageCache;
      if (updateCatch == null)
        updateCatch = UpdateICache;
      if (cacheCheck == null)
        cacheCheck = DefaultCacheCheck(pageCache);

      cacheCheck();

      var cacheCopy = new Dictionary<int, ContentCacheAndTime>(pageCache);
      IEnumerable<IContent> result = null;
      Func<List<ContentCacheAndTime>, int, bool> checkAvaliable = (list, time) => list.Count > 0 && (DateTime.Now - list.First().Time).TotalMinutes >= time;
      if (ids != null)
      {
        var pages = cacheCopy
            .Where(b => ids.Contains(b.Value.Content.Id))
            .Select(b => b.Value).OrderBy(b => b.Time).ToList();
        if (checkAvaliable(pages, cacheTime))
        {
          result = pages.Select(b => b.Content);
          goto ReturnCheck;
        }
        result = services.GetByIds(ids);
        goto ReturnCheck;
      }
      if (rootId > 0)
      {
        var contents = cacheCopy.Where(b => rootId == b.Value.Content.ParentId);
        if (where != null)
          contents = contents.Where(b => where(b.Value.Content));
        var pages = contents.Select(b => b.Value).OrderBy(b => b.Time).ToList();
        if (checkAvaliable(pages, cacheTime))
        {
          result = pages.Select(b => b.Content);
          goto ReturnCheck;
        }
        result = services.GetChildren(rootId);
        if (where != null)
          result.Where(where);
        goto ReturnCheck;
      }
      ReturnCheck:
      var check = result.OrderBy(b => b.SortOrder).ToList();
      check.ForEach(b => updateCatch(b, alive, pageCache));
      return check;
    }
  }
}
