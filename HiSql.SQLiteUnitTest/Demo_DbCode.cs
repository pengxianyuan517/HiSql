﻿using HiSql.UnitTest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HiSql.Demo_Insert;

namespace HiSql
{
    class Demo_DbCode
    {
        public static void Init(HiSqlClient sqlClient)
        {

            Console.WriteLine("表操作测试");
            //Demo_AddColumn(sqlClient);//OK

            //Demo_ModiColumn(sqlClient);//OK---
            // Demo_ReColumn(sqlClient);//OK---
            // Demo_ModiTable(sqlClient);//OK---
            // Demo_ReTable(sqlClient);//OK
            // Demo_DelColumn(sqlClient);//OK
            // Demo_Tables(sqlClient);//OK
            //Demo_CreateView(sqlClient);//OK
            //Demo_View(sqlClient);//OK
            // Demo_ViewsPaging(sqlClient);//OK
            // Demo_AllTables(sqlClient);//OK
            //Demo_TablesPaging(sqlClient);//OK
            //Demo_TableDataCount(sqlClient);//OK
            // Demo_GlobalTables(sqlClient);//没有全局临时表
            // Demo_DropView(sqlClient);//OK

            //Demo_ModiView(sqlClient);//OK
            // Demo_IndexList(sqlClient);//OK
            // Demo_Index_Create(sqlClient);//OK
            // Demo_Primary_Create(sqlClient);//OK
            // Demo_AllTablesPaging(sqlClient);//OK
            // Dome_GetTableStruct();//ok
            //Dome_TestCone();

        }
        static void Dome_TestCone()
        {
            HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            var tabinfo = sqlClient.Context.DMInitalize.GetTabStruct("H_Test");

            int cnt = 1000000;
            TabInfo _tabcopy = ClassExtensions.DeepCopy<TabInfo>(tabinfo);

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < cnt; i++)
            {
                TabInfo tabcopy = ClassExtensions.DeepCopy<TabInfo>(tabinfo);
            }


            Console.WriteLine($"ClassExtensions.DeepCopy 执行{cnt}次耗时 {stopwatch.ElapsedMilliseconds}");

             stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < cnt; i++)
            {
                TabInfo tabcopy = DataConvert.CloneTabInfo (tabinfo);
            }


            Console.WriteLine($"DataConvert.CloneTabInfo 执行{cnt}次耗时 {stopwatch.ElapsedMilliseconds}");

        }

            static void Dome_GetTableStruct()
        {
            //Global.RedisOn = true;
            //Global.RedisOptions = new RedisOptions() { Host = "192.168.10.130", Port = 8379, PassWord = "", Database = 1 }; //rCache = new RCache(new RedisOptions { Host = "192.168.10.130", Port=8379, PassWord = "" , Database = 1});
            //Global.RedisOptions = new RedisOptions() { Host = "172.16.80.178", PassWord = "pwd123", Database = 1 };

            HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("Hi_FieldModel");
            tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("Hi_TabModel");

            TabInfo tableInfo = sqlClient.Context.DMInitalize.GetTabStruct("H_Test");
            
            {
                string _key = "Lock.LockOn4900001223";
                //LckInfo 是指加锁时需要指定的信息  UName 表示加锁人，ip表示在哪一个地址加的锁，可以通过 HiSql.Lock.GetCurrLockInfo  获取所有的详细加锁信息便于后台管理
                var rtn = HiSql.Lock.LockOn(_key, new LckInfo { UName = "登陆名", Ip = "127.0.0.1" });

                HiSql.Lock.LockOn("Lock.LockOn4test", new LckInfo { UName = "登陆名", Ip = "127.0.0.1" });
                if (rtn.Item1)
                {
                    Console.WriteLine($"针对于采购订单[{_key}] 加锁成功");
                    //执行采购订单处理业务

                    //解锁
                    //HiSql.Lock.UnLock(_key);
                }

                //同时加锁多个key 如果有一个key被其它任务加锁那么 锁定失败
                var rtn2 = HiSql.Lock.LockOn(new string[] { "4900001223", "4900001224" }, new LckInfo { UName = "登陆名", Ip = "127.0.0.1" });

            }

            //Parallel.For(0, 10, (x, y) => {
            //    HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            //    TabInfo tableInfo = sqlClient.Context.DMInitalize.GetTabStruct("Hi_Test23");
            //    Console.WriteLine($"创建成功：字段数："+ tableInfo.Columns.Count);
            //});

        }


        static void Demo_AllTablesPaging(HiSqlClient sqlClient)
        {
            int total = 0;
            List<TableInfo> lsttales = sqlClient.DbFirst.GetAllTables("Hi", 11, 1, out total);
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }
        static void Demo_ViewsPaging(HiSqlClient sqlClient)
        {
            int total = 0;
            List<TableInfo> lsttales = sqlClient.DbFirst.GetViews("vw_FModel", 11, 1, out total);
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }
        static void Demo_ReTable(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var rtn = sqlClient.DbFirst.ReTable("HTest04", "HTest05", OpLevel.Execute);
            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出重命名表 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出重命名失败原因

        }


        static void Demo_Index_Create(HiSqlClient sqlClient)
        {
            //CREATE INDEX index_name ON Hi_DataElement('CreateTime','ModiTime')	3
            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("Hi_DataElement");
            List<HiColumn> hiColumns = tabInfo.Columns.Where(c => c.FieldName == "CreateTime").ToList();
            var rtn = sqlClient.DbFirst.CreateIndex("Hi_DataElement", "Hi_DataElement_Hi_DataElement6", hiColumns, OpLevel.Execute);
            if (rtn.Item1)
                Console.WriteLine(rtn.Item3);
            else
                Console.WriteLine(rtn.Item2);


            rtn = sqlClient.DbFirst.DelIndex("Hi_DataElement", "index_name", OpLevel.Execute);

            if (rtn.Item1)
                Console.WriteLine(rtn.Item3);
            else
                Console.WriteLine(rtn.Item2);
        }
        static void Demo_Primary_Create(HiSqlClient sqlClient)
        {
            //删除主键
            List<TabIndex> lstindex = sqlClient.DbFirst.GetTabIndexs("Hi_FieldModel").Where(t => t.IndexType == "Key_Index").ToList();
            foreach (var item in lstindex)
            {
                var rtndel = sqlClient.DbFirst.DelPrimaryKey(item.TabName, OpLevel.Execute);
                if (rtndel.Item1)
                    Console.WriteLine(rtndel.Item3);
                else
                    Console.WriteLine(rtndel.Item2);
            }

            //创建主键
            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("Hi_FieldModel");
            List<HiColumn> hiColumns = tabInfo.Columns.Where(c => c.FieldName == "TabName" || c.FieldName == "FieldName" ).ToList();//|| c.FieldName == "SortNum"

            hiColumns.ForEach((c) => { 
                c.IsPrimary = true;
            });
            var rtn = sqlClient.DbFirst.CreatePrimaryKey("Hi_FieldModel", hiColumns, OpLevel.Execute);
            if (rtn.Item1)
                Console.WriteLine(rtn.Item3);
            else
                Console.WriteLine(rtn.Item2);

        }
        static void Demo_IndexList(HiSqlClient sqlClient)
        {
            List<TabIndex> lstindex = sqlClient.DbFirst.GetTabIndexs("Hi_FieldModel");

            foreach (TabIndex tabIndex in lstindex)
            {
                Console.WriteLine($"TabName:{tabIndex.TabName} IndexName:{tabIndex.IndexName} IndexType:{tabIndex.IndexType}");
            }

            List<TabIndexDetail> lstindexdetails = sqlClient.DbFirst.GetTabIndexDetail("Hi_FieldModel", "sqlite_autoindex_Hi_FieldModel_1");
            foreach (TabIndexDetail tabIndexDetail in lstindexdetails)
            {
                Console.WriteLine($"TabName:{tabIndexDetail.TabName} IndexName:{tabIndexDetail.IndexName} IndexType:{tabIndexDetail.IndexType} ColumnName:{tabIndexDetail.ColumnName}");

            }
        }

        static void Demo_Tables(HiSqlClient sqlClient)
        {
            List<TableInfo> lsttales = sqlClient.DbFirst.GetTables();
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }

        static void Demo_View(HiSqlClient sqlClient)
        {
            List<TableInfo> lsttales = sqlClient.DbFirst.GetViews();
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }



        static void Demo_GlobalTables(HiSqlClient sqlClient)
        {
            List<TableInfo> lsttales = sqlClient.DbFirst.GetGlobalTempTables();
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }

        static void Demo_AllTables(HiSqlClient sqlClient)
        {
            List<TableInfo> lsttales = sqlClient.DbFirst.GetAllTables();
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }
        static void Demo_TablesPaging(HiSqlClient sqlClient)
        {
            int total = 0;
            List<TableInfo> lsttales = sqlClient.DbFirst.GetTables("HI", 11, 1, out total);
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }
        static void Demo_TableDataCount(HiSqlClient sqlClient)
        {
            int total = 0;
            int lsttales = sqlClient.DbFirst.GetTableDataCount("Hi_FieldModel");
            Console.WriteLine($" {lsttales} ");
        }




        static void Demo_CreateView(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var rtn = sqlClient.DbFirst.CreateView("vw_FModel",
                sqlClient.HiSql("select a.TabName,b.TabReName,b.TabDescript,a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName").ToSql(),
                OpLevel.Execute);

            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }

        static void Demo_ModiView(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var rtn = sqlClient.DbFirst.ModiView("vw_FModel",
                sqlClient.HiSql("select a.TabName,b.TabReName,b.TabDescript,a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName where b.TabType in (0,1)").ToSql(),
                OpLevel.Execute);

            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }

        static void Demo_DropView(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var rtn = sqlClient.DbFirst.DropView("vw_FModel",

                OpLevel.Execute);

            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }

        static void Demo_AddColumn(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            HiColumn column = new HiColumn()
            {
                TabName = "H_Test",
                FieldName = "TestAdd",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.EMPTY,
                DefaultValue = "",
                FieldDesc = "测试字段添加"

            };

            var rtn = sqlClient.DbFirst.AddColumn("H_Test", column, OpLevel.Execute);

            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }

        static void Demo_DelColumn(HiSqlClient sqlClient)
        {
            HiColumn column = new HiColumn()
            {
                TabName = "H_Test",
                FieldName = "TestAdd3",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.EMPTY,
                DefaultValue = "",
                FieldDesc = "测试字段添加"

            };

            var rtn = sqlClient.DbFirst.DelColumn("H_Test", column, OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
        }


        static void Demo_ReColumn(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            HiColumn column = new HiColumn()
            {
                TabName = "H_Test",
                FieldName = "TestAdd",
                ReFieldName = "TestAdd2",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "TGM",
                FieldDesc = "测试字段变更"

            };

            var rtn = sqlClient.DbFirst.ReColumn("H_Test", column, OpLevel.Execute);
            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }

        static void Demo_ModiTable(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var tabinfo = sqlClient.Context.DMInitalize.GetTabStruct("H_Test");

            TabInfo _tabcopy = ClassExtensions.DeepCopy<TabInfo>(tabinfo);
            //_tabcopy.Columns.RemoveAt(5);

            HiColumn newcol = ClassExtensions.DeepCopy<HiColumn>(_tabcopy.Columns[3]);
            newcol.FieldName = "TestAdd3";
            newcol.ReFieldName = "TestAdd3";
            newcol.IsNull = true;
            _tabcopy.Columns.Add(newcol);

            _tabcopy.Columns[4].ReFieldName = _tabcopy.Columns[4].FieldName + "test";
            _tabcopy.Columns[4].IsRequire = true;

            _tabcopy.PrimaryKey.ForEach(x => {
                x.IsPrimary = false;
            });
           
            _tabcopy.Columns.ForEach(t => {
                if (t.FieldName == "Hid" || t.FieldName == "TestAdd2")
                {
                    t.IsPrimary = true;
                }
            });

            var rtn = sqlClient.DbFirst.ModiTable(_tabcopy, OpLevel.Execute);
            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因

        }

        static void Demo_ModiColumn(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            HiColumn column = new HiColumn()
            {
                TabName = "H_Test",
                FieldName = "TestAdd",
                FieldType = HiType.VARCHAR,
                FieldLen = 511,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "TGasdfsdM",
                FieldDesc = "测试字888888段变更"

            };

            var rtn = sqlClient.DbFirst.ModiColumn("H_Test", column, OpLevel.Execute);
            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }
    }
}
