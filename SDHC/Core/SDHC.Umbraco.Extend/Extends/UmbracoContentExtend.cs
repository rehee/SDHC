using SpxUmbracoMember.Umbraco.Extend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace System
{
  public static partial class UmbracoContentFunction
  {
    public static T GetValueNull<T>(this IContent input, string propertyAlias)
    {
      try
      {
        return input.GetValue<T>(propertyAlias);
      }
      catch { }
      try
      {
        var value = input.Properties.Where(b => b.Alias.Equals(propertyAlias, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        return (T)Convert.ChangeType(value, typeof(T));
      }
      catch
      {
        return default(T);
      }
    }
    public static T GetValueNull<T>(this IPublishedContent input, string propertyAlias)
    {
      try
      {
        return input.GetPropertyValue<T>(propertyAlias);
      }
      catch { }
      try
      {
        var value = input.Properties.Where(b => b.PropertyTypeAlias.Equals(propertyAlias, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        return (T)Convert.ChangeType(value, typeof(T));
      }
      catch
      {
        return default(T);
      }
    }
  }
}

namespace Umbraco.Core.Services
{

  public static partial class UmbracoContentExtend
  {
    public static SetModelInIContent DefaultSetModelInIContent { get; set; } =
        (model, content) =>
        {
          var result = UmbracoContentConvertExtend.SetDynamicModel(content, model);
          return result;
        };

    public static GetAllContentByType GetAllContentTypeFunction(IContentTypeService typeService, IContentService service)
    {
      return (string typeAlais) =>
      {
        var type = typeService.GetContentType(typeAlais);
        if (type == null)
        {
          return null;
        }
        var contents = service.GetContentOfContentType(type.Id);
        if (contents == null)
        {
          return null;
        }
        return contents;
      };
    }
    public static void CreateIContent(
        IContentService service, int rootId, string orderTypeName, SetModelInIContent addingFunction, dynamic model,
        out int id, string name = "")
    {
      try
      {
        if (name == "")
          name = Guid.NewGuid().ToString();
        var content = service.CreateContent(name, rootId, orderTypeName);
        content = addingFunction(model, content);
        service.Save(content);
        service.Publish(content);
        id = content.Id;
      }
      catch (Exception ex)
      {
        id = -1;
      }
    }
    public static CreateContent CreateContentFunction(
        IContentService service, int rootId, string orderTypeName, SetModelInIContent addingFunction = null)
    {
      if (addingFunction == null)
      {
        addingFunction = DefaultSetModelInIContent;
      }
      return (dynamic model, out int id) =>
      {
        CreateIContent(service, rootId, orderTypeName, addingFunction, model, out id);
      };
    }
    public static CreateContentWithName CreateContentWithNameFunction(
        IContentService service, int rootId, string orderTypeName, SetModelInIContent addingFunction = null)
    {
      if (addingFunction == null)
      {
        addingFunction = DefaultSetModelInIContent;
      }
      return (string name, dynamic model, out int id) =>
      {
        CreateIContent(service, rootId, orderTypeName, addingFunction, model, out id, name);
      };
    }
    public static CreateContentForRoot CreateContentForRootFunction(
        IContentService service, string orderTypeName, SetModelInIContent addingFunction = null)
    {
      if (addingFunction == null)
      {
        addingFunction = DefaultSetModelInIContent;
      }
      return (dynamic model, int rootId, out int id) =>
      {
        CreateIContent(service, rootId, orderTypeName, addingFunction, model, out id);
      };
    }
    public static CreateContentForRootWithName CreateContentForRootWithNameFunction(
        IContentService service, string orderTypeName, SetModelInIContent addingFunction = null)
    {
      if (addingFunction == null)
      {
        addingFunction = DefaultSetModelInIContent;
      }
      return (string name, dynamic model, int rootId, out int id) =>
      {
        CreateIContent(service, rootId, orderTypeName, addingFunction, model, out id, name);
      };
    }



    public static string IContentShowNav { get; set; } = "showNav";
    public static string IContentShowChildren { get; set; } = "showChildren";
    public static Func<IEnumerable<string>, AllowUserAccess> CheckPublicAccessFunction(
        IPublicAccessService accessService, IContentService contentService)
    {
      return (IEnumerable<string> roles) =>
      {
        return (documentId) =>
        {
          return accessService.HasAccess(documentId, contentService, roles);
        };
      };
    }
    public static ContentTypeView ConvertToContentTypeView(
        this IContent input, bool menuOnly = false, bool showParent = false, bool noChildren = false,
        AllowUserAccess checkPublicAccess = null, bool showAllChildren = false,
        Func<IContent, bool> whereFunction = null, Comparison<IContent> sortFunction = null,
        Func<IContent, dynamic> orderBy = null, Func<IContent, dynamic> orderByDesc = null, int take = -1, int skip = -1)
    {
      if (checkPublicAccess != null)
      {
        if (!checkPublicAccess(input.Id))
        {
          return null;
        }
      }
      var result = new ContentTypeView();
      result.Id = input.Id;
      result.Name = input.Name;
      result.Status = input.Status;
      result.CreateDate = input.CreateDate;
      result.UpdateDate = input.UpdateDate;
      result.Publish = input.Published;
      result.PageType = input.ContentType.Alias;
      if (!menuOnly)
        foreach (var item in input.Properties)
        {
          result.Property.Add(item.Alias, item.Value);
        }
      if (!noChildren)
      {
        if (whereFunction == null)
        {
          whereFunction = (IContent inputContent) => true;
        }
        var childrenList = input.Children().Where(whereFunction);
        if (sortFunction != null)
        {
          childrenList.ToList().Sort(sortFunction);
        }
        if (orderBy != null)
        {
          childrenList = childrenList.OrderBy(orderBy);
        }
        if (orderByDesc != null)
        {
          childrenList = childrenList.OrderByDescending(orderByDesc);
        }
        if (take > -1)
        {
          childrenList.Take(take);
        }
        if (skip > -1)
        {
          childrenList.Skip(skip);
        }
        foreach (var item in childrenList)
        {
          if (menuOnly)
          {
            if (item.GetValueNull<bool>(IContentShowNav) != true)
              continue;
          }
          var showChildren = item.GetValueNull<bool>(IContentShowChildren) == true;
          if (showAllChildren == true)
            showChildren = true;
          var children = item.ConvertToContentTypeView(menuOnly, false, !showChildren, checkPublicAccess, showAllChildren, whereFunction, sortFunction, orderBy, orderByDesc, take, skip);
          result.Children.Add(children);
        }
      }
      if (showParent)
      {
        var p = input.Parent();
        if (p != null)
        {
          result.Parent = p.ConvertToContentTypeView(menuOnly, showParent, true, checkPublicAccess);
        }
      }
      return result;
    }


    public static IDataTypeService ThisDataTypeService { get; set; }
    public static string GetPreValueByIntValue(IDataTypeService service, int value)
    {
      return service.GetPreValueAsString(value);
    }
    public static string GetPreValueByAlise(IDataTypeService service, IContent content, string alias)
    {
      return GetPreValueByIntValue(service, content.GetValueNull<int>(alias));
    }
    public static string GetPreValueByIntValue(this int value)
    {
      return ThisDataTypeService.GetPreValueAsString(value);
    }
    public static string GetPreValueByAlise(this IContent content, string alias)
    {
      return GetPreValueByIntValue(ThisDataTypeService, content.GetValueNull<int>(alias));
    }
    public static string GetPreValueByAlise(this IContentBase content, string alias)
    {
      return GetPreValueByIntValue(ThisDataTypeService, ((IContent)content).GetValueNull<int>(alias));
    }
    public static Property GetPropertyByAlias(this IContent content, string alias)
    {
      try
      {
        return content.Properties.Where(b => b.Alias == "myDrop").FirstOrDefault();
      }
      catch
      {
        return null;
      }
    }
    public static int GetDataTypeDefinitionIdFromProperty(this Property property)
    {
      try
      {
        return property.PropertyType.DataTypeDefinitionId;
      }
      catch
      {
        return -1;
      }
    }
    public static PreValueCollection GetPreValue(this Property property)
    {
      try
      {
        return ThisDataTypeService.GetPreValuesCollectionByDataTypeId(property.GetDataTypeDefinitionIdFromProperty());
      }
      catch
      {
        return null;
      }
    }
    public static PreValueCollection GetPreValue(IDataTypeService service, int dataTypeDefinitionId)
    {
      try
      {
        return service.GetPreValuesCollectionByDataTypeId(dataTypeDefinitionId);
      }
      catch
      {
        return null;
      }
    }
    public static int GetPreValueIdByValue(this PreValueCollection preValue, string value)
    {
      try
      {
        return preValue.PreValuesAsDictionary.Where(b => b.Value.Value == value).FirstOrDefault().Value.Id;
      }
      catch
      {
        return -1;
      }
    }
    public static void SetDropDownListValue(this IContent content, string aliasName, string dropValue)
    {
      try
      {
        var value = content.GetPropertyByAlias(aliasName).GetPreValue().GetPreValueIdByValue(dropValue);
        content.SetValue(aliasName, value);
      }
      catch
      {

      }
    }
    public static void SetDropDownListValue(this IContentBase content, string aliasName, string dropValue)
    {
      try
      {
        var value = ((IContent)content).GetPropertyByAlias(aliasName).GetPreValue().GetPreValueIdByValue(dropValue);
        content.SetValue(aliasName, value);
      }
      catch
      {

      }
    }
  }
}

