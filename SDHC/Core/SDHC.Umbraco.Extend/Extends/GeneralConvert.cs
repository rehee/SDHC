using SDHC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;

namespace System
{
  public static class GeneralConvert
  {
    public static T MyConvertTo<T>(this object input,
      Dictionary<string, Func<string, object>> jsonObjectMap = null,
      Dictionary<string, Func<object, string>> objectJsonMap = null) where T : new()
    {
      if (jsonObjectMap == null)
        jsonObjectMap = JsonObjectMap;
      if (objectJsonMap == null)
        objectJsonMap = ObjectJsonMap;
      try
      {
        var result = new T();
        var resultType = typeof(T);
        var inputType = input.GetType();
        var resultProperty = resultType.GetProperties();
        var inputProperty = inputType.GetProperties();
        foreach (var p in resultProperty)
        {
          var inputP = inputProperty.Where(b => b.Name == p.Name).FirstOrDefault();
          if (inputP == null)
            continue;
          var inputIsJson = inputP.CustomAttributes.Where(b=>b.AttributeType == typeof(JsonAttribute)).FirstOrDefault();
          var resultIsJson = p.CustomAttributes.Where(b => b.AttributeType == typeof(JsonAttribute)).FirstOrDefault();
          if (inputIsJson != null && resultIsJson != null)
          {
            if (p.PropertyType == typeof(String))
            {
              var jsonString = inputP.GetValue(input).ConvertToJson();
              p.SetValue(result, jsonString);
            }
            else
            {
              var inputV = inputP.GetValue(input);
              if (inputV == null)
                continue;
              var jsonObject = ((string)inputV).ConvertJsonToObject(p.PropertyType);
              p.SetValue(result, jsonObject);
            }


            continue;
          }
          var inputValue = inputP.GetValue(input);
          if (inputValue == null)
          {
            p.SetValue(result, p.PropertyType.GetDefaultValue());
            continue;
          }
          p.SetPropertyValue(result, inputValue);
        }
        return result;
      }
      catch
      {
        return default(T);
      }
    }
    public static Dictionary<string, Func<string, object>> JsonObjectMap { get; set; } = new Dictionary<string, Func<string, object>>();
    public static Dictionary<string, Func<object, string>> ObjectJsonMap { get; set; } = new Dictionary<string, Func<object, string>>();
  }
}
