using System;
using System.Collections.Generic;
using System.Text;

namespace DrillTestCore.Models
{
   public class Global
    {
        public static SystemPara SystemPara = new SystemPara();// 系统设置参数
        public static Int16 MaxPressure1 = 0, MaxPressure2 = 0;//最大压力
        public static Point Point1 = new Point();//#1当前数据
        public static Point Point2 = new Point();//#2当前数据
        public static bool ConnectStatus1;
        public static bool ConnectStatus2;
        public static List<Point> lstPoint1 = new List<Point>();//一个洞的测试数据
        public static List<Point> lstPoint2 = new List<Point>();
        public static Holerec HoleRecod1 = new Holerec();
        public static Holerec HoleRecod2 = new Holerec();
        public static Int16 i;
        public static int ViewNubmer { get; set; }
    }
}
