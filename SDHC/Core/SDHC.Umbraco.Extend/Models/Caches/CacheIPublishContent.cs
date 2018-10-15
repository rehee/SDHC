using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace SpxUmbracoMember.Umbraco.Extend.Models.Caches
{
  public class CacheIPublishContent : CacheBase
  {
    public new IPublishedContent Content { get; set; }
    public override T GetContent<T>()
    {
      return (T)Content;
    }
    public override void Set(dynamic content, bool? keepAlive = null, DateTime? time = null)
    {
      SetAliveAndTime(keepAlive, time);
      this.Content = content;
    }
  }

  //public class CacheQuery : CacheBase
  //{

  //}
}
