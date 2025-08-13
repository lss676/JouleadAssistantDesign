using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JouleadAssistantDesign.Data;
using JouleadAssistantDesign.Models;
using JouleadAssistantDesign.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JouleadAssistantDesign.ViewModels
{
    public partial class ElementConfigViewModel : ObservableObject
    {
        private JouleadDbContext? _db;
        private Project? _project;
        private int _customItemId;
        private string _customItemName = string.Empty;

        public ObservableCollection<ElementConfigDisplay> Items { get; } = new();

        [ObservableProperty] private string title = string.Empty;

        public ElementConfigViewModel()
        {
            // 初始化基本属性，避免构造函数异常
            Title = "元素配置";
            
            // 延迟初始化，避免在构造函数中抛出异常
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                _db = new JouleadDbContext();
                
                // 1. 获取项目信息
                if (!Application.Current.Properties.Contains("CurrentProject"))
                {
                    MessageBox.Show("未找到当前项目信息，请重新选择项目。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                var projectObj = Application.Current.Properties["CurrentProject"];
                if (projectObj == null)
                {
                    MessageBox.Show("当前项目对象为空，请重新选择项目。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                _project = projectObj as Project;
                if (_project == null)
                {
                    MessageBox.Show("项目对象转换失败，请重新选择项目。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 2. 获取自定义项ID
                if (!Application.Current.Properties.Contains("CurrentCustomItemId"))
                {
                    MessageBox.Show("未找到当前自定义项ID，请重新选择自定义项。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                var customItemIdObj = Application.Current.Properties["CurrentCustomItemId"];
                if (customItemIdObj == null)
                {
                    MessageBox.Show("当前自定义项ID为空，请重新选择自定义项。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                if (customItemIdObj is int id)
                {
                    _customItemId = id;
                }
                else
                {
                    MessageBox.Show($"当前自定义项ID类型错误：{customItemIdObj.GetType().Name}，请重新选择自定义项。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 3. 获取自定义项名称
                try
                {
                    var templateItem = await _db.TemplateItems
                        .Where(ti => ti.Id == _customItemId)
                        .Select(ti => new { ti.Name })
                        .FirstOrDefaultAsync();
                    
                    _customItemName = templateItem?.Name ?? $"自定义项 {_customItemId}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"获取自定义项信息失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    _customItemName = $"自定义项 {_customItemId}";
                }

                // 4. 设置标题
                Title = $"元素配置：{_project.Name} · {_customItemName}";

                // 5. 加载模板元素
                var templateElements = new List<TemplateElement>();
                try
                {
                    templateElements = await _db.TemplateElements
                        .Where(te => te.TemplateItemId == _customItemId)
                        .OrderBy(te => te.Id)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载模板元素失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // 6. 加载已保存的配置
                var savedConfigs = new Dictionary<int, ElementConfig>();
                try
                {
                    var configs = await _db.ElementConfigs
                        .Where(ec => ec.ProjectId == _project.Id && ec.CustomItemId == _customItemId)
                        .ToListAsync();
                    savedConfigs = configs.ToDictionary(ec => ec.Id, ec => ec);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载已保存配置失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // 7. 构建显示数据
                Items.Clear();
                for (int i = 0; i < templateElements.Count; i++)
                {
                    var template = templateElements[i];
                    savedConfigs.TryGetValue(template.Id, out var saved);

                    var dropdownOptions = new List<string>();
                    if (template.IsDropdownEnabled && !string.IsNullOrEmpty(template.DropdownOptions))
                    {
                        dropdownOptions = template.DropdownOptions
                            .Split(';', StringSplitOptions.RemoveEmptyEntries)
                            .Select(opt => opt.Trim())
                            .ToList();
                    }

                    Items.Add(new ElementConfigDisplay
                    {
                        Id = template.Id,
                        DisplayIndex = i + 1,
                        Name = template.Name ?? string.Empty,
                        StandardContent = template.StandardContent ?? string.Empty,
                        ProjectContent = saved?.ProjectContent ?? string.Empty,
                        Remark = saved?.Remark ?? string.Empty,
                        ProjectId = _project.Id,
                        CustomItemId = _customItemId,
                        DropdownOptions = dropdownOptions,
                        IsDropdownEnabled = template.IsDropdownEnabled
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化元素配置界面失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                // 不抛出异常，让界面能够正常显示
            }
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                if (_project == null)
                {
                    MessageBox.Show("项目信息未初始化，无法保存。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using var db = new JouleadDbContext();

                // 1. 删除旧的配置
                var existingConfigs = db.ElementConfigs
                    .Where(ec => ec.ProjectId == _project.Id && ec.CustomItemId == _customItemId);
                db.ElementConfigs.RemoveRange(existingConfigs);

                // 2. 添加新的配置
                foreach (var item in Items)
                {
                    db.ElementConfigs.Add(new ElementConfig
                    {
                        ProjectId = _project.Id,
                        CustomItemId = _customItemId,
                        Id = item.Id,
                        Name = item.Name,
                        StandardContent = item.StandardContent,
                        ProjectContent = item.ProjectContent,
                        Remark = item.Remark
                    });
                }

                db.SaveChanges();
                MessageBox.Show("配置已保存。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Back()
        {
            if (Application.Current.MainWindow is MainWindow mw && mw.DataContext is MainWindowViewModel mvm)
            {
                mvm.ShowProjectConfigCommand.Execute(null);
            }
        }
    }
}

