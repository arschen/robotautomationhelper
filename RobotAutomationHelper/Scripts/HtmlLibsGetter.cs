using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace RobotAutomationHelper.Scripts
{
    internal static class HtmlLibsGetter
    {

        internal static void GetSeleniumLib()
        {
            string page = DownloadAsString("http://robotframework.org/SeleniumLibrary/SeleniumLibrary.html");

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);

            string temp = doc.Text;

            List<List<string>> table = doc.DocumentNode.SelectSingleNode("//div[@id='keywords-container']//table[@class='keywords']")
                        .Descendants("tr")
                        .Skip(1)
                        .Where(tr => tr.Elements("td").Count() > 1)
                        .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                        .ToList();

            int a = 1;
        }

        internal static string DownloadAsString(string url)
        {
            string pageSource = String.Empty;
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.UserAgent = "MyCrawler/1.0";
            req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            var resp = (HttpWebResponse)req.GetResponse();

            // is this even html and not an image, or video       
            if (resp.ContentType.Contains("text/html"))
            {
                var sb = new StringBuilder();
                var buffer = new char[8192];
                // get the stream
                using (var stream = resp.GetResponseStream())
                using (var sr = new StreamReader(stream, Encoding.UTF8))
                {
                    // start copying in blocks of 8K
                    var read = sr.ReadBlock(buffer, 0, buffer.Length);
                    while (read > 0)
                    {
                        sb.Append(buffer);
                        if (!sb.ToString().Contains("<div id=\"keywords - container\"><h2 id=\"Keywords\">Keywords</h2>"))
                            sb.Clear();
                        if (sb.ToString().Contains("< div id = \"footer-container\" >< p class=\"footer\">"))
                            break;
                        // max allowed chars per source
                        if (sb.Length > 50000)
                        {
                            sb.Append(" ... source truncated due to size");
                            // stop early 
                            break;
                        }
                        read = sr.ReadBlock(buffer, 0, buffer.Length);
                    }
                    pageSource = sb.ToString();
                }
            }
            return pageSource;
        }
    }
}
