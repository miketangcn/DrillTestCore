using System;
using System.Collections.Generic;
using System.Text;

namespace DrillTestCore.Models
{
   public class Global
    {
        public static SystemPara SystemPara = new SystemPara();// 系统设置参数
        public static string DateFilePath;
        public static string DateFileName1;
        public static string DateFileName2;
        public static bool IsShow1, IsShow2;
        public static Int16 MaxPressure1 = 0, MaxPressure2 = 0;//最大压力
        public static ActualPoint CurrentPoint1 = new ActualPoint();//转换后的显示值
        public static ActualPoint CurrentPoint2 = new ActualPoint();//转换后的显示值
        public static int CurrentHoleCount1 = 0;//显示用的当前洞的编号
        public static int CurrentHoleCount2 = 0;//显示用的当前洞的编号
        public static Point Point1 = new Point();//#1当前数据
        public static Point Point2 = new Point();//#2当前数据
        public static bool ConnectStatus1;
        public static bool ConnectStatus2;
        public static bool Working1;
        public static bool Working2;
        public static bool SubWorking1;
        public static bool SubWorking2;
        public static bool LastSubWorking1;
        public static bool LastSubWorking2;
        public static List<Point> lstPoint1 = new List<Point>();//一个洞的测试数据
        public static List<Point> lstPoint2 = new List<Point>();
        public static Int16 HoleNumber1;
        public static Int16 HoleNumber2;
        public static Workrec WorkRecord1 = new Workrec();
        public static Workrec WorkWorkRecord2 = new Workrec();
        public static Holerec HoleRecod1 = new Holerec();
        public static Holerec HoleRecod2 = new Holerec();
        public static Int16 i, j;
        public static bool flag = false;
        public static bool LastQueryMode;
        public static DateTime LastQueryTFrom, LastQueryTTo;
        public static string LastQueryId;
        public static int LastDataRow;
    }
}
