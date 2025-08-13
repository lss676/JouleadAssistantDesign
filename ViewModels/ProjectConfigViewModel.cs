using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JouleadAssistantDesign.Data;
using JouleadAssistantDesign.Models;
using JouleadAssistantDesign.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JouleadAssistantDesign.ViewModels
{
    public partial class ProjectConfigViewModel : ObservableObject
    {
        private readonly JouleadDbContext _db;
        private readonly Project _project;

        public ObservableCollection<CustomItem> CustomItems { get; } = new();

        [ObservableProperty] private string projectName = string.Empty;

        public ProjectConfigViewModel()
        {
            _db = new JouleadDbContext();

            // 1) 只从全局拿项目 ID，而不是整个对象
            if (!Application.Current.Properties.Contains("CurrentProject"))
                throw new InvalidOperationException("未找到当前项目 ID。");

            var projectObj = Application.Current.Properties["CurrentProject"];
            if (projectObj == null)
                throw new InvalidOperationException("当前项目对象为空。");
            
            if (projectObj is Project tempProject)
            {
                var projectId = tempProject.Id;

                // 2) 用 Include 一次性把项目及其 SelectedItems 拉出来
                _project = _db.Projects
                              .Include(p => p.SelectedItems)
                              .FirstOrDefault(p => p.Id == projectId)
                          ?? throw new InvalidOperationException("数据库中未找到该项目。");
            }
            else
            {
                throw new InvalidOperationException($"当前项目对象类型错误：{projectObj.GetType().Name}");
            }

            // 3) 设置页面标题
            ProjectName = _project.Name;

            // 2.1 从数据库把模板项加载进来
            var templates = _db.TemplateItems
                               .OrderBy(t => t.Id)
                               .ToList();

            // 2.2 用每条 TemplateItem 的 Name 来生成卡片
            foreach (var tpl in templates)
            {
                CustomItems.Add(new CustomItem
                {
                    Id = tpl.Id,
                    Name = tpl.Name,   // ← 这里取数据库里的名字
                    IsSelected = _project.SelectedItems.Any(x => x.Id == tpl.Id)
                });
            }

        }
        [RelayCommand]
        private void SelectCustomItem(int customItemId)
        {
            // 1) 把当前选中的 customItemId 保存到全局
            Application.Current.Properties["CurrentCustomItemId"] = customItemId;

            // 2) 跳转到元素配置
            if (Application.Current.MainWindow is MainWindow mw
                && mw.DataContext is MainWindowViewModel mvm)
            {
                mvm.ShowElementConfigCommand.Execute(null);
            }
        }

        [RelayCommand]
        private void EditTemplate()
        {
            // 跳转到模板管理
            if (Application.Current.MainWindow is MainWindow mw
                && mw.DataContext is MainWindowViewModel mvm)
            {
                mvm.ShowTemplateListCommand.Execute(null);
            }
        }

        [RelayCommand]
        private void Save()
        {
            // 清空原有关系
            _project.SelectedItems.Clear();
            _db.SaveChanges();

            // 重新建立关系
            foreach (var ci in CustomItems.Where(x => x.IsSelected))
            {
                // 找到数据库里的模板实体，再加到导航属性里
                var template = _db.TemplateItems.Find(ci.Id)
                              ?? throw new InvalidOperationException($"找不到 TemplateItem {ci.Id}");
                _project.SelectedItems.Add(template);
            }
            _db.SaveChanges();

            MessageBox.Show("配置已保存到数据库。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

}




