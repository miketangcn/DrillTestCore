using System;
using System.Collections.Generic;
using Stylet;
using DrillTestCore.Models;
using System.Threading.Tasks;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;

namespace DrillTestCore.Pages
{
    public class HistoryViewModel:Screen
    {
        public PlotModel MyModel { get; private set; }
        public PlotModel SingleModel { get; private set; }
        public List<RadioButtonModel> RdoQueryType { get; set; }
        public RadioButtonModel RdoQuery { get; set; }
        public DateTime  StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string QuerySearialNO { get; set; }
        public List<Workrec> Workrecs { get; set; }
        public List<Holerec> Holerecs { get; set; }
        public Workrec Workrec { get; set; }
        public Holerec Holerec { get; set; }

        public HistoryViewModel()
        {
            MyModel = new PlotModel { Title = "工件曲线" };
            MyModel.Axes.Add(new LinearAxis 
            { 
                Position = OxyPlot.Axes.AxisPosition.Bottom, 
                Minimum = 180, Maximum = 350,Title="位移mm",
                TextColor=OxyColor.FromRgb(47, 79 ,79),
                MajorGridlineStyle = LineStyle.Solid, 
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245)
            });
            MyModel.Axes.Add(new LinearAxis 
            { 
                Position = OxyPlot.Axes.AxisPosition.Left,
                Minimum = 0, 
                Maximum = 10,
                Title = "压力MP" ,
                MajorGridlineStyle= LineStyle.Solid,
                MajorGridlineColor=OxyColor.FromRgb(245,245,245) 
            });

            SingleModel = new PlotModel { Title = "单洞曲线" };

            SingleModel.Axes.Add(new LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Minimum = 180,
                Maximum = 350,
                Title = "位移mm",
                TextColor = OxyColor.FromRgb(47, 79, 79),
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245)
            });
            SingleModel.Axes.Add(new LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Minimum = 0,
                Maximum = 10,
                Title = "压力MP",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245)
            });

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
            });
            if (Workrecs != null)
            {
                Workrec = Workrecs.FirstOrDefault();
            }
            QueryHole();
        }
        public async void QueryWorkBySerialNO()
        {
            await Task.Run(() =>
            {
                if (QuerySearialNO!=null)
                {
                    using var db = new DrillContext();
                    Workrecs = db.Workrec.Where(w => w.SerialNo.Contains(QuerySearialNO)).OrderByDescending(w => w.LastTime).ToList();                   
                }
            });
            if (Workrecs != null)
            {
                Workrec = Workrecs.FirstOrDefault();
            }
            QueryHole();
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
                if (Holerecs != null)
                {
                    Holerec = Holerecs.FirstOrDefault();
                }
            }
        }
      public void  WorkGridSelectChange()
      {
        if (Workrec!=null) QueryHole();
      }      
      public void HoleGridSelectChange()
      {
            if (Holerec!=null )
            {
                List<DataPoint> DataPoints = new List<DataPoint>();
                List<Point> points = new List<Point>();
                points = CommonMethods.StringToList(Holerec.Data);
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
                SingleModel.Series.Clear();
                SingleModel.Series.Add(lineSeries);
                SingleModel.Title = "工件" + Workrec.SerialNo +"洞"+Holerec.HoleNumber.ToString()+ "曲线";
                SingleModel.InvalidatePlot(true);
            }                 
        }
    }
}
