using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace SpxUmbracoMember.Umbraco.Extend.Models
{
  public class ContentTypeView
  {
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "";
    public string PageType { get; set; } = "";
    public ContentStatus Status { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public bool Publish { get; set; }
    public List<ContentTypeView> Children = new List<ContentTypeView>();
    public Dictionary<string, dynamic> Property = new Dictionary<string, dynamic>();
    public ContentTypeView Parent { get; set; } = null;
  }
}
