using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.Core.Models
{
  public static class UmbracoContentConvertExtend
  {
    public static void GetImplementedInterfacesType(Type input, List<Type> types)
    {
      ((TypeInfo)input).ImplementedInterfaces.ToList().ForEach(
          b =>
          {
            GetImplementedInterfacesType(b, types);
          });

      types.Add(input);
    }
    public static T IContentTo<T>(this IContent input) where T : IIdentifyId, new()
    {
      try
      {
        var result = new T();
        var type = typeof(T);
        var propertys = type.GetProperties();
        foreach (var p in propertys)
        {
          try
          {
            if (input.Properties.IndexOfKey(p.Name) < 0)
            {
              continue;
            }
            var value = input.GetValue(p.Name);
            if (p.PropertyType == typeof(bool))
            {
              p.SetValue(result, value.ToBool());
              continue;
            }
            p.SetValue(result, value);
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
        }
        result.Id = input.Id;
        return result;
      }
      catch
      {
        return default(T);
      }
    }
    public static T IContentTo<T>(this IPublishedContent input) where T : IIdentifyId, new()
    {
      try
      {
        var result = new T();
        var type = typeof(T);
        var propertys = type.GetProperties();
        foreach (var p in propertys)
        {
          try
          {
            if (input.Properties.Where(b => b.PropertyTypeAlias.Equals(p.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault() == null)
            {
              continue;
            }
            var value = input.GetProperty(p.Name).Value;
            if (p.PropertyType == typeof(bool))
            {
              p.SetValue(result, value.ToBool());
              continue;
            }
            p.SetValue(result, value);
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
        }
        result.Id = input.Id;
        return result;
      }
      catch
      {
        return default(T);
      }
    }
    public static IContent SetDynamicModel(this IContent content, dynamic model)
    {
      try
      {
        Dictionary<string, object> propertys = O.ConvertToDictionary(model);
        foreach (string k in propertys.Keys)
        {
          try
          {
            content.SetValue(k, propertys[k]);
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }

        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      return content;
    }
    public static IContent SetModel<T>(this IContent content, dynamic model)
    {
      try
      {
        var type = typeof(T);
        var propertys = new List<PropertyInfo>();
        var types = new List<Type>();
        GetImplementedInterfacesType(type, types);
        types.ForEach(
            b => b.GetProperties().ToList()
            .ForEach(c => propertys.Add(c))
            );
        //var obj = ((object)model).ConvertToDictionary();
        foreach (var p in propertys)
        {
          if (content.Properties.Where(b => b.Alias.Equals(p.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() != null)
          {
            continue;
          }
          try
          {
            var key = G.Text(p).Split(' ').FirstOrDefault();
            //var value = obj[p.Name];
            var value = p.GetValue(model);
            if (p.PropertyType == typeof(System.Web.HttpPostedFileBase) && value == null)
              continue;
            content.SetValue(p.Name, value);
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      return content;
    }
  }
}

