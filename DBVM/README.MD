# DBVM CLI

独立可执行程序管理数据库DDL

``` shell
 ____    ____   __     __  __  __
 |  _ \  | __ )  \ \   / / |  \/  |
 | | | | |  _ \   \ \ / /  | |\/| |
 | |_| | | |_) |   \ V /   | |  | |
 |____/  |____/     \_/    |_|  |_|

USAGE:
    DBVM.exe [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    MySql
    SqlServer
    Postgres
```

## 通过dotnet tool安装

``` shell
# 全局安装dbvm
dotnet tool install -g dbvm 


# 使用dbvm
dbvm 
dbvm MySql
```

