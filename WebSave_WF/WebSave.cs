using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ExpertPdf;
using ExpertPdf.HtmlToPdf;

namespace WebSave_WF
{
    public partial class WebSave : Form
    {
        #region 属性
        #endregion
        #region 构造器
        public WebSave()
        {
            InitializeComponent();
        }
        #endregion

        #region 事件
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateURL();
                saveFileDialog1.Filter = "pdf files(*.pdf)|*.pdf";//设置文件选择器
                saveFileDialog1.DefaultExt = "pdf";//设置后缀
                //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
                saveFileDialog1.AddExtension = true;
                //保存对话框是否记忆上次打开的目录
                saveFileDialog1.RestoreDirectory = true;
                //默认桌面
                saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string htmlTitle = GetHTMLTitle(tbxURL.Text.Trim()).Trim();
                htmlTitle = this.TrimFileName(htmlTitle);//去掉文件名不支持的字符
                saveFileDialog1.FileName = htmlTitle.Length > 255 ? htmlTitle.Substring(0, 255) : htmlTitle;
                DialogResult result= saveFileDialog1.ShowDialog();
                string localFilePath = "", fileNameExt = "", newFileName = "", FilePath = "";
                if (result == DialogResult.OK)
                {
                    //获得文件路径
                    localFilePath = saveFileDialog1.FileName.ToString();

                    //获取文件名，不带路径
                    //fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);

                    //获取文件路径，不带文件名
                    //FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));

                    //给文件名前加上时间
                    //newFileName = DateTime.Now.ToString("yyyyMMdd") + fileNameExt;

                    //在文件名里加字符
                    //saveFileDialog.FileName.Insert(1,"dameng");
                    //为用户使用 SaveFileDialog 选定的文件名创建读/写文件流。
                    //System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();//输出文件

                    //fs可以用于其他要写入的操作
                    Use_ephtmltopdf(tbxURL.Text.Trim(), localFilePath);
                    string OKMessage = string.Format("网页保存成功！");
                    MessageBox.Show(OKMessage);
                    this.WindowState = FormWindowState.Minimized;
                }

            }
            catch (WarningException warning)
            {
                MessageBox.Show(warning.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       
       

        #endregion
        #region 方法
        /// <summary>
        /// 验证URL
        /// </summary>
        private void ValidateURL()
        {
            if (0 == tbxURL.Text.Trim().Length)
                throw new WarningException("请输入URL！");
        }
        /// <summary>
        /// 获取HTML标题
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string GetHTMLTitle(string url)
        {
            string URI = url;
            HttpWebRequest request = WebRequest.Create(URI) as HttpWebRequest;
            request.Method = "GET";
            request.KeepAlive = true;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            System.IO.Stream responseStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
            //返回的页面html文本
            string srcString = reader.ReadToEnd();
            Regex regex = new Regex("<Title>[\\s\\S]*</Title>",RegexOptions.IgnoreCase);
            MatchCollection matchCollection = regex.Matches(srcString, 0);
            if (matchCollection.Count == 0) return string.Empty;
            else {
                string strTitle = matchCollection[0].Value;
                return ParseTags(strTitle);
            
            }
        }
        /// <summary>
        /// 生成PDF
        /// </summary>
        /// <param name="URL">URL</param>
        /// <param name="outFile">输入文件名</param>
        public void Use_ephtmltopdf(string URL, string outFile)
        {
            PdfConverter pdfConverter = new PdfConverter();
            pdfConverter.PdfDocumentOptions.EmbedFonts = false;
            pdfConverter.PdfDocumentOptions.ShowFooter = false;
            pdfConverter.PdfDocumentOptions.ShowHeader = false;
            pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;
            pdfConverter.SavePdfFromUrlToFile(URL, outFile);
        }
        /// <summary>
        /// 去掉文件命中不支持的字符
        /// \、/、:、*、?、"、<、>、| 。
        /// </summary>
        /// <param name="htmlTitle"></param>
        /// <returns></returns>
        /// 
        private string TrimFileName(string htmlTitle)
        {
            return Regex.Replace(htmlTitle, @"[\\\/:\*?<>\|""]", "", RegexOptions.IgnoreCase);
        }
        ///   <summary>
        ///   移除所有HTML标签
        ///   </summary>
        ///   <param   name="htmlStr">HTMLStr</param>
        public static string ParseTags(string htmlStr)
        {
            return Regex.Replace(htmlStr, "<[^>]*>", "");
        }
        //*************************************************
        ///   <summary>
        ///   去除HTML标记
        ///   </summary>
        ///   <param   name="NoHTML">包括HTML的源码   </param>
        ///   <returns>已经去除后的文字</returns>
        public static string NoHTML(string htmlString)
        {
            //删除脚本
            htmlString = Regex.Replace(htmlString, @"<script[^>]*?>.*?</script>", "",
              RegexOptions.IgnoreCase);
            //删除HTML
            htmlString = Regex.Replace(htmlString, @"<(.[^>]*)>", "",
              RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"([\r\n])[\s]+", "",
              RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"-->", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(quot|#34);", "\"",
              RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(amp|#38);", "&",
              RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(lt|#60);", "<",
              RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(gt|#62);", ">",
              RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(nbsp|#160);", "   ",
              RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(iexcl|#161);", "\xa1",
              RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(cent|#162);", "\xa2",
              RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(pound|#163);", "\xa3",
              RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(copy|#169);", "\xa9",
              RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&#(\d+);", "",
              RegexOptions.IgnoreCase);
            htmlString.Replace("<", "");
            htmlString.Replace(">", "");
            htmlString.Replace("\r\n", "");
            return htmlString;
        }
        #endregion
    }
}
