using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.Umbraco.Extend.Models
{
  public class ViewTempAndContent
  {
    public static Func<dynamic> Helper { get; set; }
    public string ViewPath { get; set; }
    public int ContentId { get; set; }
    public object GetModel()
    {
      return Helper().TypedContent(this.ContentId);
    }
    public ViewTempAndContent(string viewPath = "", int contentId = 0)
    {
      this.ViewPath = viewPath;
      this.ContentId = contentId;
    }
  }
}
