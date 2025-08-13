using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JouleadAssistantDesign.Views
{
    public partial class DropdownOptionsWindow : Window
    {
        /// <summary>
        /// 最终用户输入的下拉菜单选项（用分号分隔）
        /// </summary>
        public string DropdownOptions { get; private set; } = string.Empty;

        /// <summary>
        /// 是否启用下拉菜单模式
        /// </summary>
        public bool IsDropdownEnabled { get; private set; } = true;

        public DropdownOptionsWindow(string initialOptions = "", bool isDropdownEnabled = true)
        {
            InitializeComponent();
            
            // 初始化选项
            DropdownOptions = initialOptions;
            IsDropdownEnabled = isDropdownEnabled;
            
            // 设置初始值
            OptionsTextBox.Text = ConvertOptionsToString(initialOptions);
            EnableDropdownCheckBox.IsChecked = isDropdownEnabled;
            
            // 更新预览
            UpdatePreview();
            
            // 聚焦到编辑框
            OptionsTextBox.Focus();
        }

        private void AddDefaultOptions_Click(object sender, RoutedEventArgs e)
        {
            var defaultOptions = new List<string>
            {
                "选项1",
                "选项2", 
                "选项3",
                "选项4",
                "选项5",
                "选项6",
                "选项7",
                "选项8",
                "选项9",
                "选项10"
            };
            
            OptionsTextBox.Text = string.Join("\n", defaultOptions);
            UpdatePreview();
        }

        private void ClearOptions_Click(object sender, RoutedEventArgs e)
        {
            OptionsTextBox.Text = string.Empty;
            UpdatePreview();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // 保存用户修改过的内容
            DropdownOptions = ConvertStringToOptions(OptionsTextBox.Text);
            IsDropdownEnabled = EnableDropdownCheckBox.IsChecked ?? false;
            DialogResult = true;
        }

        private void UpdatePreview()
        {
            var options = ConvertStringToOptions(OptionsTextBox.Text);
            var optionList = options.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
            
            // 更新预览下拉框
            PreviewComboBox.Items.Clear();
            foreach (var option in optionList)
            {
                PreviewComboBox.Items.Add(option.Trim());
            }
            
            // 更新计数
            OptionsCountText.Text = $"当前选项数量：{optionList.Count}/10";
            
            // 如果选项数量超过10个，显示警告
            if (optionList.Count > 10)
            {
                OptionsCountText.Foreground = System.Windows.Media.Brushes.Red;
                OptionsCountText.Text += " (超过限制)";
            }
            else
            {
                OptionsCountText.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        /// <summary>
        /// 将分号分隔的选项字符串转换为换行符分隔的字符串
        /// </summary>
        private string ConvertOptionsToString(string options)
        {
            if (string.IsNullOrEmpty(options))
                return string.Empty;
                
            return options.Replace(";", "\n");
        }

        /// <summary>
        /// 将换行符分隔的字符串转换为分号分隔的选项字符串
        /// </summary>
        private string ConvertStringToOptions(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
                
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(";", lines.Select(line => line.Trim()));
        }

        private void OptionsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }
    }
}

