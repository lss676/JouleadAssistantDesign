using JouleadAssistantDesign.Models;
using JouleadAssistantDesign.ViewModels;
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
    public partial class ProjectConfigView : UserControl
    {
        public ProjectConfigView()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 这里先不做真正的持久化，后面再补
            MessageBox.Show("项目配置保存功能待实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // 切回项目管理界面
            if (Application.Current.MainWindow is MainWindow mw &&
                mw.DataContext is MainWindowViewModel vm)
            {
                vm.ShowProjectListCommand.Execute(null);
            }
        }
        private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 点击哪个元素，就把它当作参数传给导航命令
            if ((sender as FrameworkElement)?.DataContext is TemplateItem item)
            {
                // 设置全局上下文的"当前元素ID"
                Application.Current.Properties["CurrentCustomItemId"] = item.Id;

                // 显示选中信息
                MessageBox.Show($"已选择自定义项: {item.Name}", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}