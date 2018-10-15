using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace SpxUmbracoMember.Umbraco.Extend.Models
{
  public abstract class ContentCacheBase
  {
    public static bool IsPageCacheUsad { get; set; } = false;
    public static int CacheTimeMinute { get; set; } = 25;
    public static int CacheCheckMaxTime { get; set; } = 30;
    public virtual dynamic Content { get; set; }
    public bool KeepAlive { get; set; } = false;
    public DateTime Time { get; set; }
  }

  public class ContentCacheAndTime : ContentCacheBase
  {
    public new IContent Content { get; set; }
  }
}
