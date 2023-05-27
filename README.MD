# DB Version Manager（DBVM）数据库版本管理器

**DBVM:** [![NuGet Badge](https://buildstats.info/nuget/DBVM)](https://www.nuget.org/packages/DBVM)
**DBVM.MySql:** [![NuGet Badge](https://buildstats.info/nuget/DBVM.MySql)](https://www.nuget.org/packages/DBVM.MySql)
**DBVM.SqlServer:** [![NuGet Badge](https://buildstats.info/nuget/DBVM.SqlServer)](https://www.nuget.org/packages/DBVM.SqlServer)
**DBVM.Postgres:** [![NuGet Badge](https://buildstats.info/nuget/DBVM.Postgres)](https://www.nuget.org/packages/DBVM.Postgres)

![GitHub](https://img.shields.io/github/license/houlongchao/DBVM?style=social)

> 通过可追踪，可管理的方式对数据库DDL进行管理。

## Nuget安装

``` shell
dotnet add package DBVM.MySql
dotnet add package DBVM.SqlServer
dotnet add package DBVM.Postgres
```

详细说明见具体数据库包。

## 使用前提

无论是哪种使用方式，都必须包含数据库升级描述文件。

默认描述文件在`DBVM`目录中，以数据库类型命名的的`.xml`文件。如：

> `SqlServer`默认描述文件为`DBVM/SqlServer.xml`
>
> `Mysql`默认描述文件为`DBVM/Mysql.xml`

描述文件的格式相同，只是针对不同数据库所实现的数据库升级脚本有所不同而已。

> `versionitem`为对应的一条数据库升级。
> `type`属性说明是引用`sql`还是`sqlfile`(`sql`文件)，**必须**设置。
> `version`为当前升级项的版本，**必须**设置。
> `author`为当前升级项的编写者，**必须**设置。
> `date`为当前升级项的编写时间，**必须**设置。
> `desc`为当前升级项的描述。



如果`type`设置为`sqlfile`则在升级项中应该引入`sql`文件名，注意`sql`文件必须和程序集一起打包发布，`sql`文件相对于当前升级描述文件路径读取。

> 多人开发时，`version`建议每次多增加几个版本，有助于代码合并时有足够版本进行冲突处理。

``` XML
<?xml version="1.0" encoding="utf-8" ?>
<dbversions>
  <versionitem type="sqlfile" version="1" author="Hou Longchao" date="20220101" desc="说明,脚本中不能使用GO等非标准SQL" >
    <![CDATA[
    init.sql
    ]]>
  </versionitem>
  <versionitem type="sql" version="5" author="Hou Longchao" date="20220101">
    <![CDATA[
    select * from ___dbversions;
    ]]>
  </versionitem>
</dbversions>
```

## 使用方式

``` C#
// 方法一，直接检查更新
var dbvm = new MySqlVersionManager($"connectionStr");
dbvm.CheckAndUpdate();
```

``` C#
// 方法二，获取需要升级的版本，然后执行更新
var dbvm = new MySqlVersionManager($"connectionStr");
var versions = dbvm.GetNeedUpdateVersions();
foreach (var versionItem in versions)
{
    dbvm.UpdateVersion(versionItem);
}
```
