using System;
using System.Collections.Generic;
using System.Text;
using Stylet;
using LiveCharts.Wpf;
using DrillTestCore.Models;
using LiveCharts;
using LiveCharts.Configurations;
using System.Threading;
using System.Threading.Tasks;

namespace DrillTestCore.Pages
{
   public  class TestViewModel: Screen
    {
        /// <summary>
        /// 缓存的测量数据
        /// </summary>
        public ChartValues<Point> ChartValues1 { get; set; }
        public ChartValues<Point> ChartValues2 { get; set; }
        /// <summary>
        /// 时间格式化器
        /// </summary>
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public bool IsReading { get; set; }
       
        public bool IsWorking1 { get; set; }
        // [DoNotNotify]
        public string show { get; set; }

        public string show1 { get; set; }
        bool Enable;  //测试发布消息到历史窗口控制按钮使能
        private IEventAggregator eventAggregator;
        public TestViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            //// 初始化创建一个 X Y 轴上数据显示的图表，并确定 X Y 轴的数据结构
            var mapper = Mappers.Xy<Point>().X(p => p.x).Y(p => p.y);
            

            // 配置这个图表，可以被其他地方（特定的方式）使用
            Charting.For<Point>(mapper);

            // 初始化测量的数据集
            ChartValues1 = new ChartValues<Point>();
            ChartValues2 = new ChartValues<Point>();
            // 设置 X ,Y轴显示的标签格式
            XFormatter = value => value+"mm";
            YFormatter = value => value + "MPa";
            // 默认情况下开始数据刷新
            IsReading = IsWorking1=true;
           
            Read();
        }
        public void PublishEvent()  //测试发布消息到历史窗口控制按钮使能
        {
            Enable = !Enable;           
            eventAggregator.Publish(new MyEvent(Enable));
        }

        protected override void OnViewLoaded()
        {
           
        }
        /// <summary>
        /// 模拟数据采集
        /// </summary>
        public async void Read()
        {

           await Task.Run(() => 
           {
               while (IsWorking1)
               {
                   ChartValues1.Clear();
                   ChartValues2.Clear();
                   int i = 0;
                   while (IsReading)
                   {                     
                       Thread.Sleep(50);
                       int value = (int)Math.Round(new Random().Next(10, 15) + (1 * new Random().NextDouble()), 2);
                       ChartValues1.Add(new Point
                       {
                           x = i,
                           y = value
                       });
                       i++;
                       if (ChartValues2.Count < ChartValues1.Count)
                       {
                           for (int k = ChartValues2.Count; k < ChartValues1.Count; k++)
                           {
                               ChartValues2.Add(ChartValues1[k]);
                           }
                       }
                       show1 = ChartValues1.Count.ToString();
                       if (ChartValues1.Count > 200) IsReading = false;
                   }
                   IsReading = true;
                   Thread.Sleep(1000);
               }
           });
           
        }
        public void test()
        {
            //ChartValues1.Clear();
            //IsWorking1 = !IsWorking1;
            //IsReading = !IsReading;
            //if (IsReading) Read();
        }
        public void test1()
        {
            //if (ChartUpdate == LiveCharts.UpdaterState.Running) ChartUpdate = LiveCharts.UpdaterState.Paused;
            //else ChartUpdate = LiveCharts.UpdaterState.Running;
            //IsWorking1 = !IsWorking1;
           // if (IsWorking1) Read();

        }
    }
}
