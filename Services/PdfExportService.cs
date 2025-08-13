using iTextSharp.text;
using iTextSharp.text.pdf;
using JouleadAssistantDesign.Data;
using JouleadAssistantDesign.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace JouleadAssistantDesign.Services
{
    public class PdfExportService
    {
        /// <summary>
        /// 导出单个项目，包括已选自定义项和每个自定义项下的元素配置
        /// </summary>
        /// <param name="projectId">项目主键</param>
        /// <param name="filePath">输出 PDF 路径</param>
        public void Export(int projectId, string filePath)
        {
            // 1) 从数据库一次性拉出这个项目及它的关联数据
            using var db = new JouleadDbContext();
            var project = db.Projects
                            .Include(p => p.SelectedItems)
                            .Include(p => p.ElementConfigs)
                            .FirstOrDefault(p => p.Id == projectId)
                          ?? throw new InvalidOperationException($"未找到项目 ID={projectId}");

            // 2) 准备文件流与文档
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            var doc = new Document(PageSize.A4, 36, 36, 54, 36);
            PdfWriter.GetInstance(doc, fs);
            doc.Open();

            // 3) 加载中文字体
            var fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            var ttcFile = Path.Combine(fontsDir, "msyh.ttc");
            var bf = BaseFont.CreateFont(ttcFile + ",0",
                                         BaseFont.IDENTITY_H,
                                         BaseFont.EMBEDDED);
            var fontTitle = new Font(bf, 18, Font.BOLD);
            var fontChapter = new Font(bf, 16, Font.BOLD);
            var fontSection = new Font(bf, 14, Font.BOLD);
            var fontHeader = new Font(bf, 12, Font.BOLD);
            var fontBody = new Font(bf, 10, Font.NORMAL);
            var fontSmall = new Font(bf, 9, Font.NORMAL);

            // 4) 封面页
            AddCoverPage(doc, project, fontTitle, fontBody);

            // 5) 目录页
            doc.NewPage();
            AddTableOfContents(doc, project, fontChapter, fontBody);

            // 6) 项目基本信息页
            doc.NewPage();
            AddProjectInfo(doc, project, fontSection, fontHeader, fontBody);

            // 7) 每个自定义项的元素配置（每个卡片一个章节）
            int chapterNumber = 1;
            foreach (var customItem in project.SelectedItems.OrderBy(ci => ci.Id))
            {
                doc.NewPage();
                AddCustomItemChapter(doc, project, customItem, chapterNumber++, fontChapter, fontSection, fontHeader, fontBody, fontSmall);
            }

            // 8) 完成并关闭
            doc.Close();
        }

        private void AddCoverPage(Document doc, Project project, Font fontTitle, Font fontBody)
        {
            // 项目标题
            var title = new Paragraph($"Joulead 辅助设计系统", fontTitle)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 50f
            };
            doc.Add(title);

            // 项目名称
            var projectTitle = new Paragraph($"项目：{project.Name}", fontTitle)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 30f
            };
            doc.Add(projectTitle);

            // 生成日期
            var date = new Paragraph($"生成日期：{DateTime.Now:yyyy年MM月dd日 HH:mm:ss}", fontBody)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f
            };
            doc.Add(date);

            // 项目信息
            var infoTable = new PdfPTable(2)
            {
                WidthPercentage = 60,
                HorizontalAlignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f
            };
            infoTable.SetWidths(new float[] { 1f, 2f });

            AddTableRow(infoTable, "项目经理：", project.Manager, fontBody);
            AddTableRow(infoTable, "创建日期：", project.CreatedDate.ToString("yyyy-MM-dd"), fontBody);
            if (project.HandoverDate.HasValue)
            {
                AddTableRow(infoTable, "交接日期：", project.HandoverDate.Value.ToString("yyyy-MM-dd"), fontBody);
            }

            doc.Add(infoTable);
        }

        private void AddTableOfContents(Document doc, Project project, Font fontChapter, Font fontBody)
        {
            var tocTitle = new Paragraph("目录", fontChapter)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f
            };
            doc.Add(tocTitle);

            var tocTable = new PdfPTable(2)
            {
                WidthPercentage = 100,
                SpacingAfter = 20f
            };
            tocTable.SetWidths(new float[] { 4f, 1f });

            // 添加目录项
            tocTable.AddCell(new PdfPCell(new Phrase("项目基本信息", fontBody)) { Border = 0 });
            tocTable.AddCell(new PdfPCell(new Phrase("3", fontBody)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });

            int pageNumber = 4;
            foreach (var customItem in project.SelectedItems.OrderBy(ci => ci.Id))
            {
                tocTable.AddCell(new PdfPCell(new Phrase($"第{pageNumber - 3}章 {customItem.Name}", fontBody)) { Border = 0 });
                tocTable.AddCell(new PdfPCell(new Phrase(pageNumber.ToString(), fontBody)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                pageNumber++;
            }

            doc.Add(tocTable);
        }

        private void AddProjectInfo(Document doc, Project project, Font fontSection, Font fontHeader, Font fontBody)
        {
            var title = new Paragraph("项目基本信息", fontSection)
            {
                SpacingAfter = 15f
            };
            doc.Add(title);

            var infoTable = new PdfPTable(2)
            {
                WidthPercentage = 100,
                SpacingAfter = 20f
            };
            infoTable.SetWidths(new float[] { 1f, 3f });

            AddTableRow(infoTable, "项目名称：", project.Name, fontHeader, fontBody);
            AddTableRow(infoTable, "项目经理：", project.Manager, fontHeader, fontBody);
            AddTableRow(infoTable, "创建日期：", project.CreatedDate.ToString("yyyy-MM-dd"), fontHeader, fontBody);
            if (project.HandoverDate.HasValue)
            {
                AddTableRow(infoTable, "交接日期：", project.HandoverDate.Value.ToString("yyyy-MM-dd"), fontHeader, fontBody);
            }

            doc.Add(infoTable);

            // 已选自定义项列表
            var selectedTitle = new Paragraph("已选自定义项：", fontSection)
            {
                SpacingAfter = 10f
            };
            doc.Add(selectedTitle);

            var selectedTable = new PdfPTable(2)
            {
                WidthPercentage = 100,
                SpacingAfter = 20f
            };
            selectedTable.SetWidths(new float[] { 1f, 4f });

            // 表头
            selectedTable.AddCell(new PdfPCell(new Phrase("序号", fontHeader)) { HorizontalAlignment = Element.ALIGN_CENTER });
            selectedTable.AddCell(new PdfPCell(new Phrase("自定义项名称", fontHeader)) { HorizontalAlignment = Element.ALIGN_CENTER });

            // 内容
            int index = 1;
            foreach (var customItem in project.SelectedItems.OrderBy(ci => ci.Id))
            {
                selectedTable.AddCell(new PdfPCell(new Phrase(index.ToString(), fontBody)) { HorizontalAlignment = Element.ALIGN_CENTER });
                selectedTable.AddCell(new PdfPCell(new Phrase(customItem.Name, fontBody)));
                index++;
            }

            doc.Add(selectedTable);
        }

        private void AddCustomItemChapter(Document doc, Project project, TemplateItem customItem, int chapterNumber, Font fontChapter, Font fontSection, Font fontHeader, Font fontBody, Font fontSmall)
        {
            // 章节标题
            var chapterTitle = new Paragraph($"第{chapterNumber}章 {customItem.Name}", fontChapter)
            {
                SpacingAfter = 15f
            };
            doc.Add(chapterTitle);

            // 获取该自定义项的所有元素配置
            var elementConfigs = project.ElementConfigs
                .Where(ec => ec.CustomItemId == customItem.Id)
                .OrderBy(ec => ec.Id)
                .ToList();

            if (!elementConfigs.Any())
            {
                var noData = new Paragraph("暂无配置数据", fontBody)
                {
                    SpacingAfter = 20f
                };
                doc.Add(noData);
                return;
            }

            // 元素配置表格
            var configTitle = new Paragraph("元素配置详情", fontSection)
            {
                SpacingAfter = 10f
            };
            doc.Add(configTitle);

            var table = new PdfPTable(5)
            {
                WidthPercentage = 100,
                SpacingAfter = 20f
            };
            table.SetWidths(new float[] { 0.5f, 1.5f, 2.5f, 2.5f, 1.5f });

            // 表头
            AddTableHeader(table, "序号", fontHeader);
            AddTableHeader(table, "名称", fontHeader);
            AddTableHeader(table, "标准内容", fontHeader);
            AddTableHeader(table, "此项目内容", fontHeader);
            AddTableHeader(table, "备注", fontHeader);

            // 内容行
            foreach (var config in elementConfigs)
            {
                AddTableDataCell(table, config.Id.ToString(), fontBody);
                AddTableDataCell(table, config.Name, fontBody);
                AddTableDataCell(table, config.StandardContent, fontBody);
                AddTableDataCell(table, config.ProjectContent, fontBody);
                AddTableDataCell(table, config.Remark, fontBody);
            }

            doc.Add(table);

            // 配置统计信息
            var statsTitle = new Paragraph("配置统计", fontSection)
            {
                SpacingAfter = 10f
            };
            doc.Add(statsTitle);

            var statsTable = new PdfPTable(2)
            {
                WidthPercentage = 60,
                SpacingAfter = 20f
            };
            statsTable.SetWidths(new float[] { 1f, 2f });

            var totalElements = elementConfigs.Count;
            var configuredElements = elementConfigs.Count(ec => !string.IsNullOrEmpty(ec.ProjectContent));
            var remarkedElements = elementConfigs.Count(ec => !string.IsNullOrEmpty(ec.Remark));

            AddTableRow(statsTable, "总元素数：", totalElements.ToString(), fontHeader, fontBody);
            AddTableRow(statsTable, "已配置元素：", $"{configuredElements} ({configuredElements * 100.0 / totalElements:F1}%)", fontHeader, fontBody);
            AddTableRow(statsTable, "有备注元素：", $"{remarkedElements} ({remarkedElements * 100.0 / totalElements:F1}%)", fontHeader, fontBody);

            doc.Add(statsTable);
        }

        private void AddTableRow(PdfPTable table, string label, string value, Font fontLabel, Font fontValue)
        {
            table.AddCell(new PdfPCell(new Phrase(label, fontLabel)) { Border = 0 });
            table.AddCell(new PdfPCell(new Phrase(value, fontValue)) { Border = 0 });
        }

        private void AddTableRow(PdfPTable table, string label, string value, Font font)
        {
            table.AddCell(new PdfPCell(new Phrase(label, font)) { Border = 0 });
            table.AddCell(new PdfPCell(new Phrase(value, font)) { Border = 0 });
        }

        private void AddTableHeader(PdfPTable table, string text, Font font)
        {
            table.AddCell(new PdfPCell(new Phrase(text, font)) 
            { 
                HorizontalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = BaseColor.LIGHT_GRAY
            });
        }

        private void AddTableDataCell(PdfPTable table, string text, Font font)
        {
            table.AddCell(new PdfPCell(new Phrase(text, font)) 
            { 
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 5f
            });
        }
    }
}

