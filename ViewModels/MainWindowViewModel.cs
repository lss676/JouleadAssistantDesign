using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JouleadAssistantDesign.Models;
using JouleadAssistantDesign.ViewModels;
using JouleadAssistantDesign.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace JouleadAssistantDesign.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserControl currentView;

        public MainWindowViewModel()
        {
            // 启动默认显示项目管理
            CurrentView = new ProjectListView();
        }

        [RelayCommand]
        private void ShowProjectList()
        {
            CurrentView = new ProjectListView();
        }

        [RelayCommand]
        private void ShowProjectConfig()
        {
            // 如果来自 P1 并且确实选中了项目，就将其存到全局并导航
            if (CurrentView is ProjectListView plv
                && plv.DataContext is ProjectListViewModel plvm
                && plvm.SelectedProject != null)
            {
                Application.Current.Properties["CurrentProject"] = plvm.SelectedProject;
                CurrentView = new ProjectConfigView();
            }
            // 否则只要全局已有 CurrentProject，就允许回到 P2
            else if (Application.Current.Properties.Contains("CurrentProject")
                     && Application.Current.Properties["CurrentProject"] is Project)
            {
                CurrentView = new ProjectConfigView();
            }
            else
            {
                MessageBox.Show(
                    "请先在项目列表中选中一个项目。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        private void ShowElementConfig()
        {
            // 确保先在 P2 里调用了 SelectCustomItem
            if (!Application.Current.Properties.Contains("CurrentCustomItemId"))
            {
                MessageBox.Show("请先在项目配置页中，点击一个自定义项卡片。",
                                "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CurrentView = new ElementConfigView();
        }
        [RelayCommand]
        private void ShowTemplateList() => CurrentView = new TemplateListView();

        [RelayCommand]
        private void ShowTemplateElements() => CurrentView = new TemplateElementView();

    }
}
