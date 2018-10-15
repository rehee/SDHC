using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.Calture
{
  public interface ICaltureService
  {
    int DefaultCalture { get; set; }
    int CurrentCalture();
    void ChangeCalture(int type);
    string Text(Dictionary<int, string> input);
    string Raw(Dictionary<int, string> input);
  }
}
