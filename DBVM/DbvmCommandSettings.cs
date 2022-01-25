using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBVM
{
    internal class DbvmCommandSettings : CommandSettings
    {
        [Description("数据库连接字符串")]
        [CommandOption("-c | --connection")]
        public string Connection { get; set; }

        [Description("数据库地址")]
        [CommandOption("--host")]
        public string Host { get; set; }

        [Description("数据库端口")]
        [CommandOption("--port")]
        public int? Port { get; set; }

        [Description("数据库账户")]
        [CommandOption("--user")]
        public string User { get; set; }

        [Description("数据库密码")]
        [CommandOption("--pwd")]
        public string Password { get; set; }

        [Description("数据库名")]
        [CommandOption("--db")]
        public string DbName { get; set; }
    }
}
