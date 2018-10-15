using Newtonsoft.Json.Serialization;
using SpxUmbracoMember.Umbraco.Extend.Extends;
using SpxUmbracoMember.Umbraco.Extend.Models.Caches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace Umbraco.Web
{
  public class CustomUmbracoApplication : UmbracoApplication
  {
    protected override IBootManager GetBootManager()
    {
      return new CustomWebBootManager(this);
    }

    public class CustomWebBootManager : WebBootManager
    {
      public CustomWebBootManager(UmbracoApplicationBase umbracoApplication) : base(umbracoApplication)
      {
      }

      public override IBootManager Complete(Action<ApplicationContext> afterComplete)
      {
        var rootPath = HostingEnvironment.ApplicationPhysicalPath;
        var config = System.IO.File.OpenRead($@"{rootPath}\Web.config");
        G.SetSettingStream(config);
        config.Close();
        var rootid = G.AppSettings("homeID");
        E.Init(
            rootid.Int32(),
            SpxCaltureType.Chinese
        );
        GlobalConfiguration.Configure(con =>
        {
          con.EnableCors();
          var jsonFormat = con.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
          jsonFormat.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        });
        AreaRegistration.RegisterAllAreas();
        GlobalConfiguration.Configure(ApiAuth.Register);
        Umbraco.Core.Services.ContentService.Saved += ContentService_Saved;
        Umbraco.Core.Services.ContentService.Deleted += ContentService_Deleted;
        Umbraco.Core.Services.ContentService.EmptiedRecycleBin += ContentService_EmptiedRecycleBin;
        return base.Complete(afterComplete);
      }
      private void ContentService_EmptiedRecycleBin(IContentService sender, Core.Events.RecycleBinEventArgs e)
      {
        try
        {
          e.Ids.ToList().ForEach(
            b => {
              E.MyCache.Remove(b);
              E.IPublishContentCache.Remove(b);
            });
        }
        catch { }

      }
      private void ContentService_Deleted(IContentService sender, Core.Events.DeleteEventArgs<IContent> e)
      {
        try
        {
          e.DeletedEntities.ToList().ForEach(b => {
            E.MyCache.Remove(b.Id);
            E.IPublishContentCache.Remove(b.Id);
          });
        }
        catch { }

      }

      private void ContentService_Saved(Core.Services.IContentService sender, Core.Events.SaveEventArgs<Core.Models.IContent> e)
      {
        var user = HttpContext.Current.User;
        e.SavedEntities.ToList()
            .ForEach(b =>
            {
              try
              {
                MethodResponse r;
                Services.GetFunc.DefaultUpdateCache<int, IContent, CacheIContent>(b.Id, b, null, E.MyCache, out r);
                var IPublish = b.ToPublishedContent();
                Services.GetFunc.DefaultUpdateCache<int, IPublishedContent, CacheIPublishContent>(b.Id, b.ToPublishedContent(), null, E.IPublishContentCache, out r);
                E.Services.ContentService.DeleteVersions(b.Id, b.UpdateDate.AddMinutes(-0.5));
              }
              catch { }
            }
            );
        return;
      }
    }
  }
}
