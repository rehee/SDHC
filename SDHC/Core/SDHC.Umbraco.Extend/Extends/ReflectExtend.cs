using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public static class ReflectExtend
  {
    public static T ConvertToGeneric<T>(this Object obj)
    {
      if (obj == null)
        return default(T);
      var targetType = typeof(T);
      var inputType = obj.GetType();
      if (targetType == inputType)
        return (T)obj;
      if (inputType == typeof(string))
      {
        var result = TryChangeType((string)obj, targetType);
        if (result == null)
          return default(T);
        return (T)result;
      }
      try
      {
        var convertResult = Convert.ChangeType(obj, targetType);
        if (convertResult == null)
          return default(T);
        return (T)convertResult;
      }
      catch
      {
        return default(T);
      }
    }
    public static object TryChangeType(this string value, Type type)
    {
      if (StringTryConvert.ContainsKey(type))
      {
        return StringTryConvert[type](value);
      }
      var TryParse = type.GetMethods().Where(b => b.Name == "TryParse" && b.GetParameters().Count() == 2).FirstOrDefault();
      if (TryParse == null)
        return Convert.ChangeType(value, type);
      var paramsList = new object[] { value, null };
      var result = TryParse.Invoke(null, paramsList);
      return paramsList[1];
    }
    #region Type TryConvert Function Dictionary
    public static Dictionary<Type, Func<string, object>> StringTryConvert { get; set; } = new Dictionary<Type, Func<string, object>>()
    {
    };
    #endregion
  }

}
