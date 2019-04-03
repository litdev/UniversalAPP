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


###发布项目到Windows Server IIS
1. 下载并安装[最新版托管捆绑包](https://www.microsoft.com/net/permalink/dotnetcore-current-windows-runtime-bundle-installer)，无需再安装dotnet sdk
    >安装完毕务必重启，确保模块列表中有```AspNetCoreModule```和```AspNetCoreModuleV2```
2. 发布项目   
    ![image](https://s2.ax1x.com/2019/04/02/A6eStU.jpg)
    > 如果使用Swagger生成API文档，则别忘了把Swagger.xml也复制进来

3. IIS上正常建个网站，然后设置网站使用的应用程序池
    ![image](https://s2.ax1x.com/2019/04/02/A6eg4U.jpg)
    >程序池高级设置里有个```启用32位应用程序```，默认为False，如果网站有问题，试试改为True   

#### 所需环境和补丁

 - [VC++2015 Update 3 RC](https://www.microsoft.com/zh-CN/download/details.aspx?id=52685)
 - Windows Server2008 R2  [KB2533623](https://support.microsoft.com/en-us/help/2533623/microsoft-security-advisory-insecure-library-loading-could-allow-remot)
 - Windows Server2012 R2   [KB2999226](https://support.microsoft.com/en-us/help/2999226/update-for-universal-c-runtime-in-windows)

 ### 参考资料
 - [博客园问答](https://q.cnblogs.com/q/111731/)
 - [IIS上托管官方文档](https://docs.microsoft.com/zh-cn/aspnet/core/host-and-deploy/iis/index?view=aspnetcore-2.2)
 - [设置环境变量和日志等](https://docs.microsoft.com/zh-cn/aspnet/core/host-and-deploy/aspnet-core-module?tabs=aspnetcore2x&view=aspnetcore-2.2)

 ### 其他
 1. 设置服务器环境变量，编辑根目录的```web.config```
    ```
    <aspNetCore processPath="dotnet" arguments=".\UniversalAPP.Web.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" >
	    <environmentVariables>
		    <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Development" />
	    </environmentVariables>
	</aspNetCore>
    ```

2. 如果服务器装了安全狗，要把```响应内容防护```关闭，不然异常处理页面(/Home/Error)会乱码

3. 有意思的一点是，如果网站需要大规模维护而需要中断访问一段时间时，可以在网站根目录增加一个```app_offline.htm```文件,这样网站更新中对外显示就比较友好了，[参见](https://docs.microsoft.com/zh-cn/aspnet/core/host-and-deploy/aspnet-core-module?tabs=aspnetcore2x&view=aspnetcore-2.2#appofflinehtm)