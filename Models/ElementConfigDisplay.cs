using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JouleadAssistantDesign.Views;
using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JouleadAssistantDesign.Models
{
    /// <summary>
    /// 用于在 UI 展示/编辑的元素配置 DTO，
    /// DisplayIndex 用于在界面显示序号，Id 对应模板行号。
    /// </summary>
    public partial class ElementConfigDisplay : ObservableObject
    {
        /// <summary>
        /// 模板行号（非自增，唯一标识同一模板元素）
        /// </summary>
        [ObservableProperty]
        private int id;

        /// <summary>
        /// 界面显示序号，从 1 开始（可选，只作界面序号，不做主键）
        /// </summary>
        [ObservableProperty]
        private int displayIndex;

        /// <summary>
        /// 元素名称
        /// </summary>
        [ObservableProperty]
        private string name = string.Empty;

        /// <summary>
        /// 标准内容（从模板库加载）
        /// </summary>
        [ObservableProperty]
        private string standardContent = string.Empty;

        /// <summary>
        /// 当前项目的内容（可编辑）
        /// </summary>
        [ObservableProperty]
        private string projectContent = string.Empty;

        /// <summary>
        /// 备注信息
        /// </summary>
        [ObservableProperty]
        private string remark = string.Empty;

        /// <summary>
        /// 数据库主键：项目 ID
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 数据库主键：自定义项 ID
        /// </summary>
        public int CustomItemId { get; set; }

        /// <summary>
        /// 下拉菜单选项列表
        /// </summary>
        [ObservableProperty]
        private List<string> dropdownOptions = new List<string>();

        /// <summary>
        /// 是否启用下拉菜单模式
        /// </summary>
        [ObservableProperty]
        private bool isDropdownEnabled = false;


    }
}

