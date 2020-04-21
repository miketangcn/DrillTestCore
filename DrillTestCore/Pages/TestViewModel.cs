using System;
using Stylet;
using DrillTestCore.Lib;
using DrillTestCore.Models;
using System.Threading;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using PropertyChanged;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using StyletIoC;

namespace DrillTestCore.Pages
{
    //public class Publisher
    //{
    //    private IEventAggregator eventAggregator;
    //    public Publisher(IEventAggregator eventAggregator)
    //    {
    //        this.eventAggregator = eventAggregator;
    //    }

    //    public void PublishEvent(string c1, string c2)
    //    {
    //        this.eventAggregator.Publish(new ConnectStatusEvent(c1,c2));
    //    }
    //}
    public  class TestViewModel: Screen
    {
        private readonly IWindowManager _windowManager;
        //   private readonly IContainer _container;
        //public Publisher publisher { get; set; }
        public PlotModel MyModel1 { get; set; }//工作台1曲线的model
        public PlotModel MyModel2 { get; set; }//工作台1曲线的mode2
        public OxyPlot.Series.LineSeries LineSeries1 { get; set; }
        public OxyPlot.Series.LineSeries LineSeries2 { get; set; }
        public Workrec Workrec1 { get; set; }//#1台当前测试工件信息
        public Workrec Workrec2 { get; set; }//#2台当前测试工件信息
        public Holerec Holerec1 { get; set; }
        public Holerec Holerec2 { get; set; }
        public ActualPoint ActualPoint1 { get; set; } //#1工作台当前读取工程值
        public ActualPoint ActualPoint2 { get; set; } //#2工作台当前读取工程值
        public bool Working1 { get; set; }      //#1工作台工作状态
        public bool SubWorking1 { get; set; }    //#1工作台压洞状态
        public bool Working2 { get; set; }      //#2工作台工作状态
        public bool SubWorking2 { get; set; }    //#2工作台压洞状态
        [DoNotNotify]
        public bool LastSubWorking1 { get; set; }    //#1工作台上次压洞状态
        [DoNotNotify]
        public bool LastSubWorking2 { get; set; }    //#2工作台上次压洞状态
        public bool CanStart1 { get; set; }//#1开始按钮使能
        public bool CanStop1 { get; set; }//#1停止按钮使能
        public bool CanRedo1 { get; set; }//#1重压按钮使能
        public bool CanStart2 { get; set; }//#2开始按钮使能
        public bool CanStop2 { get; set; }//#2停止按钮使能
        public bool CanRedo2 { get; set; }//#2重压按钮使能
        public bool Workrec1IsFocus { get; set; }//设置工作台1工件序列号textbox焦点 详见印象笔记
        public bool Workrec2IsFocus { get; set; }//设置工作台2工件序列号textbox焦点 详见印象笔记
        public bool IsReading { get; set; }
       
        public bool IsWorking1 { get; set; }

        private IEventAggregator eventAggregator;

        public TestViewModel(IEventAggregator eventAggregator,IWindowManager windowManager)
        {
            //初始化
            _windowManager = windowManager;
            this.eventAggregator = eventAggregator;
            //publisher = new Publisher(eventAggregator);
          //  _container = container;
            Workrec1 = new Workrec();
            Workrec2 = new Workrec();
            Holerec1 = new Holerec();
            Holerec2 = new Holerec();
            CanRedo2 = CanStop1 = CanRedo1 = CanStop2 =Working1=Working2=SubWorking1=SubWorking2= false;
            CanStart1 = CanStart2 = true;
            InitalChart();
            // 默认情况下开始数据刷新
            IsReading = IsWorking1=true;
           
            Read();
        }
        public void InitalChart()
        {
            MyModel1 = new PlotModel { Title = "工件台1测试曲线" };
            MyModel2 = new PlotModel { Title = "工件台2测试曲线" };
            MyModel1.Axes.Add(new LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Minimum = 180,
                Maximum = 350,
                Title = "位移mm",
                TextColor = OxyColor.FromRgb(47, 79, 79),
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245)
            });
            MyModel1.Axes.Add(new LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Minimum = 0,
                Maximum = 10,
                Title = "压力MP",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245)
            });
            MyModel2.Axes.Add(new LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Minimum = 180,
                Maximum = 350,
                Title = "位移mm",
                TextColor = OxyColor.FromRgb(47, 79, 79),
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245)
            });
            MyModel2.Axes.Add(new LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Minimum = 0,
                Maximum = 10,
                Title = "压力MP",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245)
            });
            LineSeries1 = new OxyPlot.Series.LineSeries
            {
                Color = OxyColor.FromRgb(255, 0, 0),
                StrokeThickness = 1,
                TrackerFormatString = "{0}\n{1}: {2:0.#}mm\n{3}: {4:0.##}Mpa"
            };
            MyModel1.Series.Clear();
            MyModel1.Series.Add(LineSeries1);
            LineSeries2 = new OxyPlot.Series.LineSeries
            {
                Color = OxyColor.FromRgb(255, 0, 0),
                StrokeThickness = 1,
                TrackerFormatString = "{0}\n{1}: {2:0.#}mm\n{3}: {4:0.##}Mpa"
            };
            MyModel2.Series.Clear();
            MyModel2.Series.Add(LineSeries2);
        }

        public void PublishEvent()  //测试发布消息到历史窗口控制按钮使能
        {
            var c1 = Global.ConnectStatus1 == true ? "在线" : "离线！";
            var c2 = Global.ConnectStatus2 == true ? "在线" : "离线！";
            eventAggregator.Publish(new ConnectStatusEvent(c1, c2));
            //publisher.PublishEvent(c1, c2);
        }

        protected override void OnViewLoaded()
        {
           
        }
        /// <summary>
        /// 模拟数据采集
        /// </summary>
        public async void Read()
        {
            OxyPlot.Series.LineSeries lineSeries = new OxyPlot.Series.LineSeries();
            lineSeries.Color = OxyColor.FromRgb(255, 0, 0);
            lineSeries.StrokeThickness = 1;
            lineSeries.TrackerFormatString = "{0}\n{1}: {2:0.#}mm\n{3}: {4:0.##}Mpa";
            MyModel2.Series.Clear();
            MyModel2.Series.Add(lineSeries);
            await Task.Run(() => 
            {           
               while (IsWorking1)
               {

                   lineSeries.Points.Clear();       
                   int i = 180;
                   while (IsReading)
                   {
                        PublishEvent();//联机状态消息发布
                        Thread.Sleep(50);
                       int value = (int)Math.Round(new Random().Next(10, 15) + (1 * new Random().NextDouble()), 2);
                       float value2 = new Random().Next(10);
                       i++;
                        var temppoint = new DataPoint(i, value2);
                       lineSeries.Points.Add(temppoint);
                       MyModel2.InvalidatePlot(true);
                       if (i > 350) IsReading = false;
                   }
                   IsReading = true;
                   Thread.Sleep(1000);
               }
           });    
        }
        /// <summary>
        /// 工作台1工作
        /// </summary>
        public async void Read1()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    //此处后续添加按钮使能判断
                    if (ReadValue.Read1())//读成功
                    {
                        DataTreating1();
                        if (Working1)
                        {
                          MyModel2.InvalidatePlot(true);//曲线刷新
                        }                  
                    };
                }              
            });
        }
        /// <summary>
        /// 读到一个值后的处理（判断，保存数据，刷新画面曲线）
        /// </summary>
        public async void DataTreating1()
        {
            string FullFileName = Global.DateFilePath + Global.DateFileName1;
            Models.Point point = new Models.Point
            { 
                x = Global.Point1.x,
                y = (short)(Global.Point1.y - 1000) 
            };
            ActualPoint1.x = (float)Math.Round(point.x * Global.SystemPara.con_factor_x,1);
            ActualPoint1.y = (float)Math.Round(point.x * Global.SystemPara.con_factor_y,2);
            //测试状态
            if (Global.Working1)
            {
                if (Global.Point1.x >= Global.SystemPara.con_chek_x && Global.Point1.y >= Global.SystemPara.con_chek_y)//进入压洞状态
                {
                    Global.SubWorking1 = true;
                    CanRedo1 = false;
                    if (Global.Point1.y > Global.MaxPressure1)//设置最大压力
                    {
                        Global.MaxPressure1 = (short)Global.Point1.y; //其实没太大意义
                    }
                    var temppoint = new DataPoint(ActualPoint1.x, ActualPoint1.y);
                    LineSeries1.Points.Add(temppoint);
                    Global.lstPoint1.Add(point);
                }
                else if (Global.Point1.x < Global.SystemPara.con_chek_x || Global.Point1.y < Global.SystemPara.con_chek_y)//不在压洞状态
                {
                    Global.SubWorking1 = false;
                    CanRedo1 = true;
                }

                if (Global.LastSubWorking1 && !Global.SubWorking1)//一个洞压完
                {
                    //调用异步写数据库和写数据文件代码
                    Workrec1.LastTime = DateTime.Now;
                    Holerec1.MaxPressure = (float)Math.Round((Global.MaxPressure1 * Global.SystemPara.con_factor_y - 10), 2);
                    Holerec1.TestTime = DateTime.Now;
                    Holerec1.SerialNo = Workrec1.SerialNo;
                    Workrec1.HoleCount=Holerec1.HoleNumber;
                    Holerec1.Data =CommonMethods.ListToString(Global.lstPoint1);
                    Holerec1.MacId = 1;
                    await Task.Run(() =>
                    {
                        CommonMethods.AddHolerec(Holerec1); //向数据库写记录
                        CommonMethods.AddWorkrec(Workrec1); //向数据库写记录
                        if (Global.SystemPara.AutoOut)//根据需要确定是否要写到excel文件中
                        {
                            if (CommonMethods.GetFilePath(FullFileName))
                            {
                                NopiExcelHelper<Models.Point>.AddExcel(Global.lstPoint1, FullFileName, "hole" + Global.HoleRecod1.HoleNumber.ToString(),
                                (float)Global.HoleRecod1.MaxPressure);
                            }
                        }
                    });
                  
                    //这个地方要增加设置窗口重新测试按钮为不可用?
                    Workrec1.HoleCount++;

                }
                if (!SubWorking1)
                {
                    LineSeries1.Points.Clear();
                    Global.lstPoint1.Clear();
                }
                LastSubWorking1 = SubWorking1;
            }
            //非测试状态
            else
            {
                Global.lstPoint1.Clear();
            }
        }    
        public void Redo1()
        {
            if (Holerec1.HoleNumber>1)
            {
                var dialogResult = _windowManager.ShowMessageBox($"确定要重压当前{Holerec1.HoleNumber - 1}号钻头","警告",MessageBoxButton.YesNo,
                                      MessageBoxImage.Warning);
                if (dialogResult==MessageBoxResult.Yes)
                {
                    Holerec1.HoleNumber--;
                }

            }
         
        }
        public async void Start1()
        {
            Workrec1.HoleCount = 0;
            if (Workrec1.Layer == 0) Workrec1.Layer = 1;
            if (Holerec1.LayerNo == null || Holerec1.LayerNo == 0) Holerec1.LayerNo = 1;
            if (Workrec1.SerialNo == null) Workrec1.SerialNo = DateTime.Now.Date.ToString("yyyMMdd") + DateTime.Now.TimeOfDay.ToString("hhmm");
            //判断当前测试工件输入正确与否 重复与否
            //进入测试状态后按钮使能
            //判断#2台是否有相同号工作
            if (Working2 && Workrec1.SerialNo==Workrec2.SerialNo)
            {
                _windowManager.ShowMessageBox("测试工件号已在#2台测试，请变更工件号", "警告", MessageBoxButton.OK);
                return;
            }
            CanStart1 = false;
            //查询数据中是否相同的工件
            Workrec temp=new Workrec();
            Holerec temph = new Holerec();
            await Task.Run(() =>
            {
                using var db = new DrillContext();
                temp = db.Workrec.Where(w => w.SerialNo == Workrec1.SerialNo).FirstOrDefault();
                if (temp != null)
                {
                    temph = db.Holerec.LastOrDefault();
                }
            });
            if (temp != null)
            {
                var dialogResult=_windowManager.ShowMessageBox("测试工件号已存在，继续以此工件号测试请按'Yes'变更请按'No'", "警告",
                                                   MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (dialogResult == MessageBoxResult.No)
                {
                    //此处为增加textbox焦点控制代码
                    Workrec1IsFocus = CanStart1 = true;
                    return;
                }
                else
                {
                    if (temph != null)
                    {
                        Holerec1.HoleNumber = temph.HoleNumber++;
                    }
                    else Holerec1.HoleNumber = 1;
                }
            }
            else Holerec1.HoleNumber = 1;
            Working1 = true;
            CanRedo1 = CanStop1 = true;
            Working1 = true;
        }
        public void Stop1()
        {
            Working1 = false;
            CanRedo1 = CanStop1 = false;
            CanStart1 = true;
        }
   }
}
