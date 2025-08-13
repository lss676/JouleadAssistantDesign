using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JouleadAssistantDesign.Models
{
    public class CustomItem
    {
        /// <summary>
        /// 自定义项编号 1–16
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 自定义项名称（如 “自定义1”、“自定义2”）
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 是否勾选
        /// </summary>
        public bool IsSelected { get; set; }

        // 反向导航属性
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
