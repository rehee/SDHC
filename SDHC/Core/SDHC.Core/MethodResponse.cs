using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public class MethodResponse
  {
    public bool IsOk { get; set; } = false;
    public string Message { get; set; } = "";
    public string Error { get; set; } = "";
    public int ResponseCode { get; set; } = 0;
    public dynamic ResponseObject { get; set; }
  }
}
