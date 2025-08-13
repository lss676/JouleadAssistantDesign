using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JouleadAssistantDesign.Models
{
    /// <summary>
    /// 用户模型，用于权限控制
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// 用户角色枚举
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// 普通用户 - 只能查看项目，进入P2/P3只读
        /// </summary>
        User = 0,
        
        /// <summary>
        /// 管理员 - 可以修改自定义项模板
        /// </summary>
        Admin = 1
    }
}

