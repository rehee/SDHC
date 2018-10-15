using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        var type = typeof(T);
        var propertys = type.GetProperties();
        var obj = input.ConvertToDictionary();
        foreach (var p in propertys)
        {
          try
          {
            var key = G.Text(p).Split(' ').FirstOrDefault();
            var item = obj[p.Name];
            if (jsonObjectMap.ContainsKey(key))
            {
              p.SetValue(result, jsonObjectMap[key]((string)item));
              continue;
            }
            var key3 = G.Text(item);
            if (key.Equals("system.string", StringComparison.OrdinalIgnoreCase) && objectJsonMap.ContainsKey(key3))
            {
              p.SetValue(result, objectJsonMap[key3](item));
              continue;
            }
            p.SetValue(result, item);
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
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
