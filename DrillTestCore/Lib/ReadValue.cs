using HslCommunication.ModBus;
using System;
using DrillTestCore.Models;
using System.Collections.Generic;
using System.Text;
using HslCommunication.LogNet;
using System.Threading.Tasks;
using HslCommunication;
using System.Threading;
using Stylet;
using DrillTestCore.Pages;

namespace DrillTestCore.Lib
{

    public static class ReadValue
    {
        private static IEventAggregator eventAggregator = new EventAggregator();
        private static ModbusTcpNet ModbusTcpNet1 = new ModbusTcpNet(Global.SystemPara.IP1);
        private static ModbusTcpNet ModbusTcpNet2 = new ModbusTcpNet(Global.SystemPara.IP2);
        public static short Distance, Pressure;
        public static bool IsMax;
        public static ILogNet logNet = new LogNetFileSize(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Logs", 2 * 1024 * 1024);
        private static System.Threading.Thread thread2 = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadBackgroundRead2));

        public static void PublishEvent()  //测试发布消息到历史窗口控制按钮使能
        {
            var c1 = Global.ConnectStatus1 == true ? "在线" : "离线！";
            var c2 = Global.ConnectStatus2 == true ? "在线1" : "离线1！";
            eventAggregator.Publish(new ConnectStatusEvent(c1, c2));
        }
        public static async void ConnnectPlc1()
        {
            Task task = new Task(() =>
            {
                try
                {
                    OperateResult connect1 = ModbusTcpNet1.ConnectServer();
                    if (connect1.IsSuccess)
                    {
                        Global.ConnectStatus1 = true;
                    }
                    else Global.ConnectStatus1 = false;
                }
                catch
                {
                    Global.ConnectStatus1 = false;
                }
            });
            task.Start();
            await task;
        }
        public static async void ConnnectPlc2()
        {
            Task task = new Task(() =>
            {
                try
                {
                    OperateResult connect2 = ModbusTcpNet2.ConnectServer();
                    if (connect2.IsSuccess)
                    {
                        Global.ConnectStatus2 = true;
                    }
                    else Global.ConnectStatus2 = false;
                }
                catch
                {
                    Global.ConnectStatus2 = false;
                }
            });
            task.Start();
            await task;


        }
        public static void DisConnnectPlc1()
        {
            ModbusTcpNet1.ConnectClose();
        }
        public static void DisConnnectPlc2()
        {
            ModbusTcpNet2.ConnectClose();
        }

        public static void StartRead2()
        {
            thread2.IsBackground = true;
            thread2.Start();

        }
        public static bool Read1()
        {
            string address = "x=4;72";
            bool flag = false;
                if (!Global.ConnectStatus1)
                {
                    logNet.WriteWarn("#1压机通讯故障");
                    ModbusTcpNet1.ConnectServer();
                }
                try
                {
                    #region 写AO1,AO2 这一段要删掉
                    //if (Global.Working1)
                    //{
                    //    Distance = (short)(Distance + 2);
                    //    if (Pressure < 650 && !IsMax)
                    //    {
                    //        Random random = new Random();
                    //        Pressure = (short)(Pressure +random.Next(3));
                    //    }
                    //    else
                    //    {
                    //        Random random = new Random();
                    //        IsMax = true;
                    //        Pressure = (short)(Pressure -random.Next(2,8));
                    //    }
                    //    byte[] buffer = new byte[4];
                    //    ModbusTcpNet1.ByteTransform.TransByte(Distance).CopyTo(buffer, 0);//
                    //    ModbusTcpNet1.ByteTransform.TransByte(Pressure).CopyTo(buffer, 2);
                    //    OperateResult write = ModbusTcpNet1.Write(addresswrite, buffer);//要删掉
                    //    if (write.IsSuccess)
                    //    {
                    //        OperateResult<byte[]> result = ModbusTcpNet1.Read(address, ushort.Parse("2"));
                    //        if (result.IsSuccess)
                    //        {
                    //            Global.ConnectStatus1 = true;
                    //            Global.Point1.x = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 0);
                    //            Global.Point1.y = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 2);
                    //            Point point = new Point();
                    //            point.x = Global.Point1.x;
                    //            point.y = (short)(Global.Point1.y - 1000);
                    //            CommonMethods.DateTreating1(point);
                    //        }
                    //        else Global.ConnectStatus1 = false;
                    //    }
                    //    else
                    //    {
                    //        // failed
                    //    }
                    //}
                    #endregion
                    OperateResult<byte[]> result = ModbusTcpNet1.Read(address, ushort.Parse("2"));
                    if (result.IsSuccess)
                    {
                        Global.ConnectStatus1 = true;
                        Global.Point1.x = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 0);
                        Global.Point1.y = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 2);
                        flag= true;

                    }
                    else Global.ConnectStatus1 = false; //这段后面要恢复
                }
                catch
                {
                    //设置读写标志为false   
                    Global.ConnectStatus1 = false;
                }
            return flag;
        }
        private static void ThreadBackgroundRead2()
        {
            string address = "x=4;72";
            while (true)
            {
                if (!Global.ConnectStatus2)
                {
                    logNet.WriteWarn("#2压机通讯故障");
                    ModbusTcpNet2.ConnectServer();
                }
                try
                {
                    OperateResult<Byte[]> result = ModbusTcpNet2.Read(address, ushort.Parse("2"));
                    if (result.IsSuccess)
                    {
                        Global.Point2.x = ModbusTcpNet2.ByteTransform.TransInt16(result.Content, 0);
                        Global.Point2.y = ModbusTcpNet2.ByteTransform.TransInt16(result.Content, 2);
                        Global.ConnectStatus2 = true;
                        Point point = new Point();
                        point.x = Global.Point2.x;
                        point.y = (short)(Global.Point2.y - 1000);
                        //CommonMethods.DateTreating2(point);
                    }
                    else Global.ConnectStatus2 = false;
                }
                catch
                {
                    //设置读写标志为false  
                    Global.ConnectStatus2 = false;
                }
                Thread.Sleep(5);
            }
        }
    }
}
