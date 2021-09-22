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
using System.Windows.Documents;

namespace DrillTestCore.Lib
{

    public static class ReadValue
    {
        private static ModbusTcpNet ModbusTcpNet1 = new ModbusTcpNet(Global.SystemPara.IP1);
        private static ModbusTcpNet ModbusTcpNet2 = new ModbusTcpNet(Global.SystemPara.IP2);
        public static short Distance=0, Pressure=320;
        public static  string address = "x=4;72";
        public static  string addresswrite = "96";//要删掉
        public static bool IsMax;
        public static ILogNet logNet = new LogNetFileSize(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Logs", 2 * 1024 * 1024);

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


        public static bool Read1()
        {
            bool flag = false;
                try
                {
                    #region 写AO1,AO2 这一段要删掉
                    if (TestViewModel.Working1)
                    {
                        Distance = (short)(Distance + 2);
                        if (Pressure < 650 && !IsMax)
                        {
                            Random random = new Random();
                            Pressure = (short)(Pressure + random.Next(3));
                        }
                        else
                        {
                            Random random = new Random();
                            IsMax = true;
                            Pressure = (short)(Pressure - random.Next(2, 8));
                        }
                        byte[] buffer = new byte[4];
                        ModbusTcpNet1.ByteTransform.TransByte(Distance).CopyTo(buffer, 0);//
                        ModbusTcpNet1.ByteTransform.TransByte(Pressure).CopyTo(buffer, 2);
                        OperateResult write = ModbusTcpNet1.Write(addresswrite, buffer);//要删掉
                        if (write.IsSuccess)
                        {
                            OperateResult<byte[]> result = ModbusTcpNet1.Read(address, ushort.Parse("2"));
                            if (result.IsSuccess)
                            {
                                Global.ConnectStatus1 = true;
                                Global.Point1.x = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 0);
                                Global.Point1.y = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 2);
                                flag = true;
                            }
                        }
                        else
                        {
                            // failed
                        }
                    }
                    #endregion
                    //OperateResult<byte[]> result = ModbusTcpNet1.Read(address, ushort.Parse("2"));
                    //if (result.IsSuccess)
                    //{
                    //    Global.ConnectStatus1 = true;
                    //    Global.Point1.x = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 0);
                    //    Global.Point1.y = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 2);
                    //    flag= true;
                    //}
                }
                catch
                {
                    //设置读写标志为false   
                    Global.ConnectStatus1 = false;
                }
            Global.ConnectStatus1 = flag;
            if (!Global.ConnectStatus1)
            {

                logNet.WriteWarn("#1压机通讯故障");
                ModbusTcpNet1.ConnectServer();
            }
            Thread.Sleep(2);
            return flag;
        }

        public static bool Read2()
        {
            bool flag = false;
            try
            {
                OperateResult<byte[]> result = ModbusTcpNet2.Read(address, ushort.Parse("2"));
                if (result.IsSuccess)
                {
                    Global.ConnectStatus2 = true;
                    Global.Point2.x = ModbusTcpNet2.ByteTransform.TransInt16(result.Content, 0);
                    Global.Point2.y = ModbusTcpNet2.ByteTransform.TransInt16(result.Content, 2);
                    flag = true;
                }
            }
            catch
            {
                //设置读写标志为false   
                Global.ConnectStatus2 = false;
            }
            Global.ConnectStatus2 = flag;
            if (!Global.ConnectStatus2)
            {

                logNet.WriteWarn("#2压机通讯故障");
                ModbusTcpNet2.ConnectServer();
            }
            Thread.Sleep(2);
            return flag;
        }


    }
}
