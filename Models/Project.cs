using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace JouleadAssistantDesign.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string Manager { get; set; } = string.Empty;
        public DateTime? HandoverDate { get; set; }
        /// <summary>本项目所选的模板项列表</summary>
        // 把 SelectedItems 定义为 ICollection<TemplateItem>，EF Core 会自动建关联表
        public ICollection<TemplateItem> SelectedItems { get; set; } = new List<TemplateItem>();

        public ICollection<ElementConfig> ElementConfigs { get; set; } = new List<ElementConfig>();
    }
}
