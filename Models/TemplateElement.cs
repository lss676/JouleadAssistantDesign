using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JouleadAssistantDesign.Models
{
    /// <summary>
    /// 单个元素模板（属于某个 TemplateItem） 
    /// </summary>
    public class TemplateElement
    {
        /// <summary>行号/序号</summary>
        public int Id { get; set; }

        /// <summary>所属的自定义项模板 ID（外键）</summary>
        public int TemplateItemId { get; set; }

        /// <summary>反向导航到父模板项</summary>
        public TemplateItem TemplateItem { get; set; } = null!;

        /// <summary>元素名称，如"元素1"</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>标准内容，模板定义的默认值</summary>
        public string StandardContent { get; set; } = string.Empty;

        /// <summary>下拉菜单选项列表，用分号分隔</summary>
        public string DropdownOptions { get; set; } = string.Empty;

        /// <summary>是否启用下拉菜单模式</summary>
        public bool IsDropdownEnabled { get; set; } = false;
    }
}
