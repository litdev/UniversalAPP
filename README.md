#### 此分支数据库为MySql版
1. 安装MySql，[Win10Linux子系统](https://blog.csdn.net/weixin_39345384/article/details/80855359),[安装后有个问题要解决](https://www.cnblogs.com/cpl9412290130/p/9583868.html)，安装的版本是```5.7.25-0ubuntu0.18.04.2```,安装完毕后需要[更改编码为utf8mb4](https://www.cnblogs.com/rchao/p/9492167.html) ([Windows版本](https://blog.csdn.net/NikoZhao/article/details/79521812))，不然插入中文报错
2. 一开始使用的是```MySql.Data.EntityFrameworkCore```V8.0.16这个Oracle官方版本，但是如果属性为Bool，则会报错```Unable to cast object of type 'System.Boolean' to type 'System.SByte'.```，据说是还未修复的bug，没办法改用```Pomelo.EntityFrameworkCore.MySql```V2.2.0,注意连接字符串中要有```TreatTinyAsBoolean=true``` [参考文章](https://blog.csdn.net/sxy_student/article/details/88171823)

#### 项目初始化
1. 当```Migrations```目录下```init```存在时，项目启动后会自动创建数据库并插入基础数据，如果不存在，则进行```add-migration init```后再运行项目(无需进行update)
2. 当数据库不存在时，会自动进行迁移并插入一些基础数据(如添加一个后台管理账户)，所以下周源码后直接运行项目就行了
3. 往后当有新的迁移(`add-migration xxx`)时，如果不进行手动迁移数据库(`update-database`)则访问网页会收到`HTTP Error 502.5 - ANCM Out-Of-Process Startup Failure`这个错误，如果项目本地直接连接线上服务器，则在`程序包管理器控制台`(默认项目选择：UniversalAPP.EFCore)进行`update-database`就行了。否则将`UniversalAPP.EFCoreMigrator`项目发布后放到服务器cmd中运行`dotnet UniversalAPP.EFCoreMigrator.dll`按照提示进行迁移(数据库连接字符串写死了，注意更改)。
4. 附程序包管理器控制台中的迁移命令

命令|描述|-
-:|:-:|:-:
Add-Migration xxx|创建迁移|
Update-Database|更新数据库|
Script-Migration|生成SQL迁移脚本|
Script-Migration -From xxx -To xxx|生成指定版本的SQL迁移脚本(xxx为Migrations文件中的cs文件名)|
Remove-Migration|删除迁移|
 
#### 项目迁移到NetCore中遇到的一些差异

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


#### 发布项目到Windows Server IIS
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
