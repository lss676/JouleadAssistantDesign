using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace JouleadAssistantDesign.Views
{
    public partial class MultiLineInputWindow : Window
    {
        /// <summary>
        /// 最终用户输入的内容
        /// </summary>
        public string ContentText { get; private set; } = string.Empty;

        public MultiLineInputWindow(string initialContent)
        {
            InitializeComponent();

            // 把传进来的初始文本放到编辑框
            TextEditor.Text = initialContent;
            // 聚焦到编辑框
            TextEditor.Focus();
            TextEditor.CaretIndex = TextEditor.Text.Length;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // 保存用户修改过的内容
            ContentText = TextEditor.Text;
            DialogResult = true;
        }
    }
}
