using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JouleadAssistantDesign.Models
{
    /// <summary>
    /// 自定义项模板（1–16）
    /// </summary>
    public class TemplateItem
    {
        /// <summary>主键 ID，1–16</summary>
        public int Id { get; set; }

        /// <summary>模板名称，如"自定义1"</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>是否勾选（用于UI显示）</summary>
        public bool IsSelected { get; set; }

        /// <summary>该自定义项下的元素模板列表</summary>
        public ICollection<TemplateElement> Elements { get; set; } = new List<TemplateElement>();

        /// <summary>反向导航属性：使用该模板项的项目列表</summary>
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
