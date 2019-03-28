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
