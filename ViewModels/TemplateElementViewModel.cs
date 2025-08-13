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
using static iTextSharp.text.pdf.AcroFields;
using Microsoft.Win32;
using System.IO;



namespace JouleadAssistantDesign.ViewModels
{
    public partial class TemplateElementViewModel : ObservableObject
    {
        private readonly JouleadDbContext _db;
        private readonly TemplateItem _template;

        /// <summary>
        /// 用于展示在 DataGrid 中的 DTO 列表
        /// </summary>
        public ObservableCollection<TemplateElementDisplay> Elements { get; }
            = new ObservableCollection<TemplateElementDisplay>();

        /// <summary>
        /// DataGrid 选中行
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveRowCommand))]
        private TemplateElementDisplay? selectedElement;

        [ObservableProperty]
        private string title = string.Empty;

        public TemplateElementViewModel()
        {
            _db = new JouleadDbContext();

            // 1) 取要编辑的模板项 ID
            if (!Application.Current.Properties.Contains("EditingTemplateItemId"))
                throw new InvalidOperationException("未找到要编辑的模板项 ID。");
            var templateItemIdObj = Application.Current.Properties["EditingTemplateItemId"];
            if (templateItemIdObj == null)
                throw new InvalidOperationException("要编辑的模板项 ID 为空。");
            
            int templateItemId;
            if (templateItemIdObj is int id)
            {
                templateItemId = id;
            }
            else
            {
                throw new InvalidOperationException($"要编辑的模板项ID类型错误：{templateItemIdObj.GetType().Name}");
            }

            // 2) 从数据库加载 TemplateItem 及其 Elements
            _template = _db.TemplateItems
                           .Include(t => t.Elements)
                           .FirstOrDefault(t => t.Id == templateItemId)
                       ?? throw new InvalidOperationException($"找不到模板项 ID={templateItemId}");

            // 3) 设置窗口标题
            Title = $"编辑模板：{_template.Name}";

            // 4) 把已有的元素模板按照 Id 升序加载到 Elements
            var dbList = _template.Elements
                                  .OrderBy(e => e.Id)
                                  .ToList();
            for (int i = 0; i < dbList.Count; i++)
            {
                var tpl = dbList[i];
                                        Elements.Add(new TemplateElementDisplay
                        {
                            Id = tpl.Id,
                            DisplayIndex = i + 1,
                            Name = tpl.Name,
                            StandardContent = tpl.StandardContent
                        });
            }
        }

        /// <summary>
        /// 新增一行（在集合末尾）
        /// </summary>
        [RelayCommand]
        private void AddRow()
        {
            int nextIndex = Elements.Count + 1;
            Elements.Add(new TemplateElementDisplay
            {
                Id = 0, // 新行，主键留 0，让 EF 自动生成
                DisplayIndex = nextIndex,
                Name = string.Empty,
                StandardContent = string.Empty
            });
        }

        /// <summary>
        /// 删除选中的行
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanRemoveRow))]
        private void RemoveRow()
        {
            if (SelectedElement != null)
                Elements.Remove(SelectedElement);
        }

        private bool CanRemoveRow() => SelectedElement != null;

        /// <summary>
        /// 保存到数据库，并重新加载最新数据
        /// </summary>
        [RelayCommand]
        private void Save()
        {
            try
            {
                var templateItemId = _template.Id;

                using var db = new JouleadDbContext();

                // 1) 加载数据库中，这个模板项下所有已存在的实体行
                var dbElements = db.TemplateElements
                                   .Where(e => e.TemplateItemId == templateItemId)
                                   .ToDictionary(e => e.Id);

                // 2) 找出要删除的：在 dbElements 里有、但在 Elements 集合里已经没有的那些 Id
                var toDelete = dbElements.Keys
                                 .Except(Elements.Where(d => d.Id > 0).Select(d => d.Id))
                                 .ToList();
                if (toDelete.Any())
                {
                    // 批量删除
                    var deletedEntities = toDelete.Select(id => dbElements[id]);
                    db.TemplateElements.RemoveRange(deletedEntities);
                }

                // 3) 处理更新与新增
                foreach (var dto in Elements)
                {
                    if (dto.Id > 0 && dbElements.TryGetValue(dto.Id, out var existing))
                    {
                        // 已有的一条，更新字段
                        existing.Name = dto.Name;
                        existing.StandardContent = dto.StandardContent;
                    }
                    else if (dto.Id <= 0)
                    {
                        // 新增行：Id<=0 标记它是新行
                        db.TemplateElements.Add(new TemplateElement
                        {
                            TemplateItemId = templateItemId,
                            Name = dto.Name,
                            StandardContent = dto.StandardContent
                        });
                    }
                }

                // 4) 提交
                db.SaveChanges();

                // 5) 可选：刷新 DTO 集合，让它和数据库真实状态保持一致
                {
                    var savedList = db.TemplateElements
                                      .Where(e => e.TemplateItemId == templateItemId)
                                      .OrderBy(e => e.Id)
                                      .ToList();

                    Elements.Clear();
                    for (int i = 0; i < savedList.Count; i++)
                    {
                        var e = savedList[i];
                        Elements.Add(new TemplateElementDisplay
                        {
                            Id = e.Id,
                            DisplayIndex = i + 1,
                            Name = e.Name,
                            StandardContent = e.StandardContent
                        });
                    }
                }

                MessageBox.Show("元素模板已保存。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        /// <summary>
        /// 编辑下拉菜单选项
        /// </summary>
        [RelayCommand]
        private void EditDropdownOptions()
        {
            if (SelectedElement == null)
            {
                MessageBox.Show("请先选择一个元素行。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 从数据库获取当前元素的下拉菜单选项
            var currentElement = _db.TemplateElements
                                   .FirstOrDefault(e => e.Id == SelectedElement.Id);
            
            string currentOptions = currentElement?.DropdownOptions ?? "";
            bool isDropdownEnabled = currentElement?.IsDropdownEnabled ?? false;

            // 弹出下拉菜单选项编辑窗口
            var win = new DropdownOptionsWindow(currentOptions, isDropdownEnabled)
            {
                Owner = Application.Current.MainWindow
            };
            
            if (win.ShowDialog() == true)
            {
                // 更新数据库中的下拉菜单选项
                if (currentElement != null)
                {
                    currentElement.DropdownOptions = win.DropdownOptions;
                    currentElement.IsDropdownEnabled = win.IsDropdownEnabled;
                    _db.SaveChanges();
                    
                    // 更新显示的标准内容
                    if (win.IsDropdownEnabled && !string.IsNullOrEmpty(win.DropdownOptions))
                    {
                        var options = win.DropdownOptions.Split(';', StringSplitOptions.RemoveEmptyEntries);
                        SelectedElement.StandardContent = string.Join(" | ", options);
                    }
                    else
                    {
                        // 如果不启用下拉菜单模式，清空标准内容或恢复原始内容
                        SelectedElement.StandardContent = currentElement?.StandardContent ?? "";
                    }
                    
                    MessageBox.Show("下拉菜单选项已保存。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// 点击"返回"导航回模板列表
        /// </summary>
        [RelayCommand]
        private void Back()
        {
            if (Application.Current.MainWindow is MainWindow mw &&
                mw.DataContext is MainWindowViewModel vm)
            {
                vm.ShowTemplateListCommand.Execute(null);
            }
        }
    }
}