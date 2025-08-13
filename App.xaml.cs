using System.Configuration;
using System.Data;
using System.Windows;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using PdfSharp.Fonts;
using JouleadAssistantDesign.Services;

namespace JouleadAssistantDesign
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 注册对所有系统代码页（包括 windows-1252）的支持
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            base.OnStartup(e);
        }
    }
}