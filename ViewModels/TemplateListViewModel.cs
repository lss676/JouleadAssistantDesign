using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JouleadAssistantDesign.Data;
using JouleadAssistantDesign.Models;
using JouleadAssistantDesign.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JouleadAssistantDesign.ViewModels
{
    public partial class TemplateListViewModel : ObservableObject
    {
        private readonly JouleadDbContext _db = new JouleadDbContext();

        public ObservableCollection<TemplateItem> Items { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditElementsCommand))]
        private TemplateItem? selectedItem;

        public TemplateListViewModel()
        {
            // 加载所有模板项（1–16）
            var list = _db.TemplateItems.OrderBy(t => t.Id).ToList();

            // 如果表为空，先插入 1–16
            if (!list.Any())
            {
                for (int i = 1; i <= 16; i++)
                    _db.TemplateItems.Add(new TemplateItem { Id = i, Name = $"自定义{i}" });
                _db.SaveChanges();
                list = _db.TemplateItems.OrderBy(t => t.Id).ToList();
            }

            foreach (var t in list)
                Items.Add(t);
        }

        [RelayCommand]
        private void Save()
        {
            _db.SaveChanges();
            MessageBox.Show("模板列表已保存。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand(CanExecute = nameof(CanEditElements))]
        private void EditElements()
        {
            if (SelectedItem == null)
                return;

            // 1) 存下要编辑的模板项 ID
            Application.Current.Properties["EditingTemplateItemId"] = SelectedItem.Id;

            // 2) 导航到 TemplateElementView（通过调用 MainWindowViewModel）
            if (Application.Current.MainWindow is MainWindow mw &&
                mw.DataContext is MainWindowViewModel shellVm)
            {
                shellVm.ShowTemplateElementsCommand.Execute(null);
            }
        }

        private bool CanEditElements() => SelectedItem != null;
    }
}

