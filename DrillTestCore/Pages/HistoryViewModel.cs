using System;
using System.Collections.Generic;
using System.Text;
using Stylet;
using DrillTestCore.Models;
using System.Threading.Tasks;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;

namespace DrillTestCore.Pages
{
    public class HistoryViewModel:Screen
    {
        public Subscriber _subscriber { get; set; }
        public SeriesCollection MutilSeriesCollection { get; set; }
        public SeriesCollection SingleSeriesCollection { get; set; }
        public PlotModel MyModel { get; private set; }
        public List<RadioButtonModel> RdoQueryType { get; set; }
        public RadioButtonModel RdoQuery { get; set; }
        public DateTime  StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string QuerySearialNO { get; set; }
        public List<Workrec> Workrecs { get; set; }
        public List<Holerec> Holerecs { get; set; }
        public Workrec Workrec { get; set; }
        public Holerec Holerec { get; set; }

        public ChartValues<ActualPoint> SingleChartValues { get; set; }
        /// <summary>
        /// 时间格式化器
        /// </summary>
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public HistoryViewModel(Subscriber subscriber)
        {
            _subscriber = subscriber;
            _subscriber._enable = true;
            MyModel = new PlotModel { Title = "Example 1" };

            MyModel.Axes.Add(new LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Minimum = 180, Maximum = 350,Title="位移mm",
                TextColor=OxyColor.FromRgb(47, 79 ,79), MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = OxyColor.FromRgb(245, 245, 245) });
            MyModel.Axes.Add(new LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Minimum = 0, Maximum = 10,
                Title = "压力MP" ,MajorGridlineStyle= LineStyle.Solid,MajorGridlineColor=OxyColor.FromRgb(245,245,245)  });
            //// 初始化创建一个 X Y 轴上数据显示的图表，并确定 X Y 轴的数据结构
            var mapper = Mappers.Xy<ActualPoint>().X(p => p.x).Y(p => p.y);
            // 配置这个图表，可以被其他地方（特定的方式）使用
            Charting.For<ActualPoint>(mapper);
            // 初始化测量的数据集
            SingleChartValues= new ChartValues<ActualPoint>();
            // 设置 X ,Y轴显示的标签格式
            XFormatter = value => value + "mm";
            YFormatter = value => value + "MPa";
            MutilSeriesCollection = new SeriesCollection();

            RdoQueryType = new List<RadioButtonModel>()
            {
               new RadioButtonModel{Content="时间方式",IsCheck=true},
               new RadioButtonModel{Content="工件号方式",IsCheck=false},
            };
            string year = (DateTime.Now.Year-1).ToString() + "0101";
            StartTime = DateTime.ParseExact(year, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            EndTime = DateTime.Now.AddDays(1);
            QueryWorkByDate();

        }
        public void Query()
        {
            RdoQuery = RdoQueryType.Where(q => q.IsCheck == true).FirstOrDefault();
            if (RdoQuery.Content == "时间方式")
                QueryWorkByDate();
            else QueryWorkBySerialNO();

        }
        public  async void  QueryWorkByDate()
        {
            await Task.Run(() =>
            {
                using var db = new DrillContext();
                Workrecs = db.Workrec.Where(w => w.LastTime >=StartTime && w.LastTime<= EndTime).OrderByDescending(w=>w.LastTime).ToList();
                if (Workrecs!=null)
                {
                    Workrec=Workrecs.FirstOrDefault();
                    QueryHole();
                }
              
            });
           
        }
        public async void QueryWorkBySerialNO()
        {
            await Task.Run(() =>
            {
                if (QuerySearialNO!=null)
                {
                    using var db = new DrillContext();
                    Workrecs = db.Workrec.Where(w => w.SerialNo.Contains(QuerySearialNO)).OrderByDescending(w => w.LastTime).ToList();
                    if (Workrecs != null)
                    {
                        Workrec = Workrecs.FirstOrDefault();
                        QueryHole();
                    }
                }
            });
        }
        public async void QueryHole()
        {
            if (Workrec!=null)
            {
                await Task.Run(() =>
                {
                    using (var db = new DrillContext())
                    {
                        Holerecs = db.Holerec.Where(h => h.SerialNo == Workrec.SerialNo).ToList();
                    }
                    if (Holerecs != null)
                    {
                        Holerec = Holerecs.FirstOrDefault();
                    }
                    MyModel.Series.Clear();
                    for (int i = 0; i < Holerecs.Count; i++)
                    {
                        List<DataPoint> DataPoints = new List<DataPoint>();
                        List<Point> points = new List<Point>();
                        points = CommonMethods.StringToList(Holerecs[i].Data);
                        for (int j = 0; j < points.Count; j++)
                        {
                            var temppoint = new DataPoint(points[j].x * Global.SystemPara.con_factor_x, points[j].y * Global.SystemPara.con_factor_y);
                            DataPoints.Add(temppoint);
                        }
                        OxyPlot.Series.LineSeries lineSeries = new OxyPlot.Series.LineSeries();
                        lineSeries.Points.AddRange(DataPoints);
                        lineSeries.Color = OxyColor.FromRgb(255, 0, 0);
                        lineSeries.StrokeThickness = 1;
                        lineSeries.TrackerFormatString = "{0}\n{1}: {2:0.#}mm\n{3}: {4:0.##}Mpa";
                        MyModel.Series.Add(lineSeries);
                    }
                    MyModel.Title = "工件" + Workrec.SerialNo + "曲线";
                    MyModel.InvalidatePlot(true);
                });
            }
            


            //await Task.Run(() =>
            //{
            //    using (var db=new DrillContext())
            //    {
            //        Holerecs = db.Holerec.Where(h => h.SerialNo == Workrec.SerialNo).ToList();
            //    }
            //});
            //if (Holerecs!=null)
            //{
            //    Holerec = Holerecs.FirstOrDefault();
            //}
            //if (MutilSeriesCollection!=null)
            //{
            //    MutilSeriesCollection.Clear();
            //}
            //for (int i = 0; i < Holerecs.Count; i++)
            //{
            //    ChartValues<ActualPoint> ChartValues = new ChartValues<ActualPoint>();
            //    List<Point> points = new List<Point>();
            //    LineSeries tempLineSeries = new LineSeries();
            //    points = CommonMethods.StringToList(Holerecs[i].Data);
            //    for (int j = 0; j < points.Count; j++)
            //    {
            //        ActualPoint actualPoint = new ActualPoint();
            //        actualPoint.x = points[j].x * Global.SystemPara.con_factor_x;
            //        actualPoint.y = points[j].y * Global.SystemPara.con_factor_y;
            //        //actualPoint.x = (float)Math.Round((double)points[j].x * Global.SystemPara.con_factor_x,2);
            //        //actualPoint.y = (float)Math.Round((double)points[j].y * Global.SystemPara.con_factor_y,2);
            //        ChartValues.Add(actualPoint);
            //    }
            //    tempLineSeries.Values = ChartValues;
            //    tempLineSeries.StrokeThickness = 1;
            //    tempLineSeries.Stroke = System.Windows.Media.Brushes.Red;
            //    tempLineSeries.Fill = System.Windows.Media.Brushes.LightBlue;
            //    tempLineSeries.PointGeometry = null;
            //    if (i<=20)
            //    {
            //        MutilSeriesCollection.Add(tempLineSeries);
            //    }
            //}
        }
      public void  WorkGridSelectChange()
      {
        if (Workrec!=null) QueryHole();
      }      
      public void HoleGridSelectChange()
      {
            SingleChartValues.Clear();
            if (Holerec!=null)
            {
                //List<Point> points = new List<Point>();
                List<Point> points = CommonMethods.StringToList(Holerec.Data);
                for (int j = 0; j < points.Count; j++)
                {
                    ActualPoint actualPoint = new ActualPoint
                    {
                        x = points[j].x * Global.SystemPara.con_factor_x,
                        y = points[j].y * Global.SystemPara.con_factor_y
                    };
                    SingleChartValues.Add(actualPoint);
                }
            }                 
        }
    }
}
