using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using JouleadAssistantDesign.Models;

namespace JouleadAssistantDesign.Data
{
    public class JouleadDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<CustomItem> CustomItems { get; set; }
        public DbSet<ElementConfig> ElementConfigs { get; set; }
        public DbSet<TemplateItem> TemplateItems { get; set; }
        public DbSet<TemplateElement> TemplateElements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // 在应用根目录生成本地 SQLite 数据库文件 joulead.db
            options.UseSqlite("Data Source=joulead.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 复合主键：ProjectId + CustomItemId + Id（模板行号）
            modelBuilder.Entity<ElementConfig>(b =>
            {
                b.HasKey(ec => new { ec.ProjectId, ec.CustomItemId, ec.Id });
                b.Property(ec => ec.Id)
                 .ValueGeneratedNever(); // 不自增，用来对齐模板行号
            });

            // 多对多：Project.SelectedItems <-> TemplateItem.Projects
            modelBuilder.Entity<Project>()
                .HasMany(p => p.SelectedItems)
                .WithMany(ti => ti.Projects)
                .UsingEntity(j => j.ToTable("ProjectTemplateItems"));

            // 一对多：Project -> ElementConfigs
            modelBuilder.Entity<Project>()
                .HasMany(p => p.ElementConfigs)
                .WithOne(ec => ec.Project)
                .HasForeignKey(ec => ec.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // 模板项 → 元素模板（1:N）
            modelBuilder.Entity<TemplateItem>()
                .HasMany(t => t.Elements)
                .WithOne(e => e.TemplateItem)
                .HasForeignKey(e => e.TemplateItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}

