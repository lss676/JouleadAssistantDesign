using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JouleadAssistantDesign.Models;
using JouleadAssistantDesign.Services;
using JouleadAssistantDesign.Views;
using Microsoft.Win32;                       // SaveFileDialog
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JouleadAssistantDesign.Data;   // EF Core 上下文
using Microsoft.EntityFrameworkCore;  // For Include if needed

namespace JouleadAssistantDesign.ViewModels
{
    public partial class ProjectListViewModel : ObservableObject
    {
        public ObservableCollection<Project> Projects { get; } = new();

        private readonly JouleadDbContext _db;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ExportProjectCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteProjectCommand))]
        private Project? selectedProject;

        public ProjectListViewModel()
        {
            _db = new JouleadDbContext();
            _db.Database.Migrate();

            var list = _db.Projects.ToList();
            foreach (var p in list)
                Projects.Add(p);
        }

        [RelayCommand]
        private void NewProject()
        {
            var dlg = new NewProjectWindow() { Owner = Application.Current.MainWindow };
            if (dlg.ShowDialog() == true)
            {
                var project = new Project
                {
                    Name = dlg.ProjectName,
                    Manager = dlg.ProjectManager,
                    CreatedDate = DateTime.Now,
                    HandoverDate = dlg.HandoverDate
                };
                _db.Projects.Add(project);
                _db.SaveChanges();
                Projects.Add(project);
            }
        }

        [RelayCommand(CanExecute = nameof(CanModify))]
        private void ExportProject()
        {
            if (SelectedProject == null)
                return;

            // 弹出文件保存对话框
            var dlg = new SaveFileDialog
            {
                Title = "导出项目至 PDF",
                Filter = "PDF 文件 (*.pdf)|*.pdf",
                FileName = $"Joulead_{SelectedProject.Name}_{DateTime.Now:yyyyMMdd}.pdf"
            };
            if (dlg.ShowDialog() != true)
                return;

            try
            {
                // 调用 PdfExportService，传入项目主键而不是整个实体
                var svc = new PdfExportService();
                svc.Export(SelectedProject.Id, dlg.FileName!);

                MessageBox.Show(
                    "导出成功！",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"导出失败：{ex.Message}",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        [RelayCommand(CanExecute = nameof(CanModify))]
        private void DeleteProject()
        {
            if (SelectedProject == null) return;
            var msg = $"确定要删除项目 “{SelectedProject.Name}” 及其所有配置吗？";
            if (MessageBox.Show(msg, "删除确认", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            _db.Projects.Remove(SelectedProject);
            _db.SaveChanges();
            Projects.Remove(SelectedProject);
            SelectedProject = null;
            MessageBox.Show("删除成功。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanModify() => SelectedProject != null;
    }
}
