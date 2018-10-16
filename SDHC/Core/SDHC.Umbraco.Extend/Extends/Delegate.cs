using SpxUmbracoMember.Umbraco.Extend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace Umbraco.Core.Services
{
  public delegate T GetContentValue<T>(int pageId, string key, bool? keepAlive = null);
  public delegate IContent GetContent(int pageId, bool? keepAlive = null);
  public delegate dynamic GetContentValue(int pageId, string key, bool? keepAlive = null);
  public delegate IContent SetModelInIContent(dynamic model, IContent content);
  public delegate void CreateContentFunction(IContentService service, int rootId, string contentTypeAlais,
      SetModelInIContent addingFunction, dynamic model, string contentName, out int id);
  public delegate void CreateContent(dynamic model, out int id);
  public delegate void CreateContentForRoot(dynamic model, int rootId, out int id);
  public delegate IEnumerable<IContent> GetAllContentByType(string typeAlais);
  public delegate bool AllowUserAccess(int id);
  public delegate void CheckPageCachePerMin(int maxMins, int cacheTime, Dictionary<int, ContentCacheAndTime> PageCache);
  public delegate void CreateContentWithName(string name, dynamic model, out int id);
  public delegate void CreateContentForRootWithName(string name, dynamic model, int rootId, out int id);
}
