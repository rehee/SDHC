using PdfSharp;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace SpxUmbracoMember.PdfConvert.Extends
{
  public static class SpxusPdfEdtend
  {
    public static PdfDocument GeneratePdfByHtml(this string html, PdfSharp.PageSize pageSize = PdfSharp.PageSize.A4, int MarginBottom = 70, int MarginLeft = 20, int MarginRight = 20, int MarginTop = 70, string css = "")
    {
      var config = new PdfGenerateConfig()
      {
        MarginBottom = MarginBottom,
        MarginLeft = MarginLeft,
        MarginRight = MarginRight,
        MarginTop = MarginTop,
        PageSize = pageSize
      };
      var Css = PdfGenerator.ParseStyleSheet(css);
      return PdfGenerator.GeneratePdf(html, config, Css);
    }
  }
}
