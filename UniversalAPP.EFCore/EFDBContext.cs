using Microsoft.EntityFrameworkCore;

namespace UniversalAPP.EFCore
{
    /// <summary>
    /// 数据库操作上下文
    /// </summary>
    public class EFDBContext:DbContext
    {
        private static string ConnectionString;

        public EFDBContext(string connectionString) {
            ConnectionString = connectionString;
        }

        public EFDBContext(DbContextOptions<EFDBContext> options):base(options)
        {
           
        }  
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(ConnectionString, p => p.UseRowNumberForPaging());
        }
        

        public DbSet<Entity.Demo> Demos { get; set; }

        public DbSet<Entity.DemoAlbum> DemoAlbums { get; set; }

        public DbSet<Entity.DemoDept> DemoDepts { get; set; }

        public DbSet<Entity.SysLogException> SysLogExceptions { get; set; }

        public DbSet<Entity.SysLogMethod> SysLogMethods { get; set; }

        public DbSet<Entity.AppVersion> AppVersons { get; set; }

        public DbSet<Entity.CusCategory> CusCategorys { get; set; }

        public DbSet<Entity.SysRole> SysRoles { get; set; }

        public DbSet<Entity.SysRoleRoute> SysRoleRoutes { get; set; }

        public DbSet<Entity.SysRoute> SysRoutes { get; set; }

        public DbSet<Entity.SysUser> SysUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //指定表名
            //modelBuilder.Entity<Entity.AppVersion>().ToTable("AppVersion");
            modelBuilder.Entity<Entity.SysUser>().ToTable("SysUser");
        }

    }
}
