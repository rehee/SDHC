using SpxUmbracoMember.PdfConvert.Extends;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.PdfConvert
{
  public class PdfService : IPdf
  {
    public HtmlToPdf HtmlToPdfFunction { get; set; }
    public CreatePdfFileFunction FileGenerateFunction { get; set; }
    public PageSize Size { get; set; }
    public int MarginBottom { get; set; }
    public int MarginLeft { get; set; }
    public int MarginRight { get; set; }
    public int MarginTop { get; set; }
    public string Css { get; set; }
    #region default css
        = @"
                * {
                    page-break-inside: avoid;
                }
                .header {
                position: fixed;
                left: 20px;
                top: 20px;
                }
                .footer{
                    position: fixed;
                    top: 770px;
                }
                p {
                    page-break-inside: avoid;
                }
            ";
    #endregion
    public PdfService(string serverBasePath, PageSize size = PageSize.A4,
        int marginBottom = 70, int marginLeft = 20, int marginRight = 20, int marginTop = 70, string css = "")
    {
      this.Size = size;
      this.MarginBottom = marginBottom;
      this.MarginLeft = marginLeft;
      this.MarginRight = marginRight;
      this.MarginTop = marginTop;
      if (!String.IsNullOrEmpty(css))
      {
        this.Css = css;
      }
      this.FileGenerateFunction = this.fileGenerateFunction(serverBasePath, this.Size, this.MarginBottom, this.MarginLeft, this.MarginRight, this.MarginTop, this.Css);
      this.HtmlToPdfFunction = this.htmlToPdfFunction(this.Size, this.MarginBottom, this.MarginLeft, this.MarginRight, this.MarginTop, this.Css);
    }

    public void CreatePdfFile(string serverBasePath, SpxUmbracoMember.PdfConvert.PageSize size, int marginBottom, int marginLeft, int marginRight, int marginTop, string css, string html, string fileName, string path, out string url, out MethodResponse response)
    {
      response = new MethodResponse();
      try
      {
        var pdf = html.Text().GeneratePdfByHtml((PdfSharp.PageSize)((int)size), marginBottom, marginLeft, marginRight, marginTop, css);
        pdf.Save($"{serverBasePath}\\{path}\\{fileName}.pdf");
        url = $"/{path}/{fileName}.pdf";
        response.IsOk = true;
      }
      catch (Exception ex)
      {
        url = "";
        response.ResponseObject = ex;
      }
    }
    public CreatePdfFileFunction fileGenerateFunction(string serverBasePath, PageSize size = PageSize.A4, int marginBottom = 70, int marginLeft = 20, int marginRight = 20, int marginTop = 70, string css = "")
    {
      return (string html, string fileName, string path, out string url, out MethodResponse response) =>
      {
        CreatePdfFile(serverBasePath, size, marginBottom, marginLeft, marginRight, marginTop, css, html, fileName, path, out url, out response);
      };
    }
    public byte[] CreatePdfByte(PageSize size, int marginBottom, int marginLeft, int marginRight, int marginTop, string css, string html)
    {
      Byte[] res = null;
      using (MemoryStream ms = new MemoryStream())
      {
        var pdf = html.Text().GeneratePdfByHtml((PdfSharp.PageSize)size, marginBottom, marginLeft, marginRight, marginTop, css);
        pdf.Save(ms);
        res = ms.ToArray();
      }
      return res;
    }
    public HtmlToPdf htmlToPdfFunction(PageSize size = PageSize.A4, int marginBottom = 70, int marginLeft = 20, int marginRight = 20, int marginTop = 70, string css = "")
    {
      return (string html) =>
      {
        return CreatePdfByte(size, marginBottom, marginLeft, marginRight, marginTop, css, html);
      };
    }
  }
}
