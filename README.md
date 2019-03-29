###项目迁移到NetCore中遇到的一些差异

1. JQuery的版本是v3.2.1，这个版本取消了`size()`这个函数，改用`length`   
2. EFCore中将对象添加到DBContext上下文中有些区别   
    旧
    ```
    System.Data.Entity.Infrastructure.DbEntityEntry entry = db.Entry<T>(entity);
    entry.State = EntityState.Modified;
    ```
    新
    ```
    Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry = db.Entry<T>(entity);
    entry.State = EntityState.Modified;
    ```
3. 用户机密，复制```appsettings.Development.json```里的配置到```secrets.json```中，正常获取配置文件的KEY，如果该KEY在```secrets.json```中存在，则优先读取```secrets.json```中的值，否则使用```appsettings.Development.json```的值    
    >注意，如果是使用了```secrets.json```的值，网站运行中如果更改了机密中的值，系统是不会```reloadOnChange```的，需要项目重新编译运行