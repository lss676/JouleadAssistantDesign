using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JouleadAssistantDesign.Models
{
    /// <summary>
    /// 单个自定义项下的元素配置
    /// </summary>
    public class ElementConfig
    {
        public int Id { get; set; }                // 序号
        public string Name { get; set; } = string.Empty;             // 名称
        public string StandardContent { get; set; } = string.Empty; // 标准内容
        public string ProjectContent { get; set; } = string.Empty;  // 此项目内容
        public string Remark { get; set; } = string.Empty;          // 备注信息

        // 新增：项目外键和导航
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int CustomItemId { get; set; }
    }
}
