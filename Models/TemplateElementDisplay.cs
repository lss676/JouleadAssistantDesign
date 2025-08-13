using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace JouleadAssistantDesign.Models
{
    public partial class TemplateElementDisplay : ObservableObject
    {
        [ObservableProperty]
        private int id;  // 真正的主键
        
        [ObservableProperty]
        private int displayIndex;  // 用于排序显示
        
        [ObservableProperty]
        private string name = string.Empty;
        
        [ObservableProperty]
        private string standardContent = string.Empty;

        /// <summary>背后关联的模板项 ID</summary>
        public int TemplateItemId { get; set; }
    }
}
