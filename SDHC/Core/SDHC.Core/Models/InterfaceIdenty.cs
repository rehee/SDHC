using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
  public interface IIdentifyId
  {
    int Id { get; set; }
  }
  public interface IIdentifyKey
  {
    Guid Key { get; set; }
  }
  public interface IIdentifyName
  {
    string Name { get; set; }
  }
}

