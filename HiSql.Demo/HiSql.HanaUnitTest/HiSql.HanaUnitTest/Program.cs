﻿using System;

namespace HiSql.HanaUnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            Console.WriteLine($"数据库连接id" + sqlClient.Context.ConnectedId);
            //Demo_Insert.Init(sqlClient);
            //DemoCodeFirst.Init(sqlClient);
            //Demo_Query.Init(sqlClient);

            Demo_Upgrade.Init(sqlClient);

            //Demo_Update.Init(sqlClient);
            //Demo_Delete.Init(sqlClient);
            //Demo_DbCode.Init(sqlClient);
        }
    }
}
