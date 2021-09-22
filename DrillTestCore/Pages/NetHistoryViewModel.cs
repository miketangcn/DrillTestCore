using System;
using System.Collections.Generic;
using Stylet;
using DrillTestCore.Models;
using DrillTestCore.Lib;
using System.Threading.Tasks;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using System.Windows;
using OxyPlot.Wpf;
using System.Windows.Forms;
namespace DrillTestCore.Pages
{
     public class NetHistoryViewModel : Stylet.Screen
    {
        private readonly IWindowManager _windowManager;
        public PlotModel MyModel { get; private set; }
        public PlotModel SingleModel { get; private set; }
        public List<RadioButtonModel> RdoQueryType { get; set; }
        public RadioButtonModel RdoQuery { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string QuerySearialNO { get; set; }
        public List<Workrec> Workrecs { get; set; }
        public List<Holerec> Holerecs { get; set; }
        public Workrec Workrec { get; set; }  
        public Holerec Holerec { get; set; }
        public bool CanExportExcel { get; set; }

        public NetHistoryViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            MyModel = new PlotModel { Title = "工件曲线" };

            MyModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Minimum = 180,
                Maximum = 350,
                Title = "位移mm",
                TextColor = OxyColor.FromRgb(47, 79, 79),
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245),
                IsZoomEnabled = false
            });
            MyModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Minimum = 0,
                Maximum = 10,
                Title = "压力MP",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245),
                IsZoomEnabled = false
            });

            SingleModel = new PlotModel { Title = "单洞曲线" };

            SingleModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Minimum = 180,
                Maximum = 350,
                Title = "位移mm",
                TextColor = OxyColor.FromRgb(47, 79, 79),
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245),
                IsZoomEnabled = false
            });
            SingleModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Minimum = 0,
                Maximum = 10,
                Title = "压力MP",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245),
                IsZoomEnabled = false
            });

            RdoQueryType = new List<RadioButtonModel>()
            {
                new RadioButtonModel{Content="工件号方式",IsCheck=false},
                new RadioButtonModel{Content="时间方式",IsCheck=true},
            };
            string year = (DateTime.Now.Year).ToString() + "0101";
            StartTime = DateTime.ParseExact(year, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            EndTime = DateTime.Now.AddDays(1);
            CanExportExcel = true;
            //QueryWorkByDate();

        }
        public void Query()
        {
            RdoQuery = RdoQueryType.Where(q => q.IsCheck == true).FirstOrDefault();
            if (RdoQuery.Content == "时间方式")
                QueryWorkByDate();
            else QueryWorkBySerialNO();

        }
        public async void QueryWorkByDate()
        {
            try
            {
                await Task.Run(() =>
                {
                    using var db = new AliDbContext();
                    Workrecs = db.Workrecs.Where(w => w.LastTime >= StartTime && w.LastTime <= EndTime).OrderByDescending(w => w.LastTime).ToList();

                    //if (Workrec != null) QueryHole();  //Workrecs变化后会触发dategrid的selectchange 事件 会触发 queryhole 过程
                });
            }
            catch (Exception)
            {

                _windowManager.ShowMessageBox("网络数据库连接失败！", "警告");
            }

            //Workrec与dategrid 的selectitem绑定 如果将下面的Workrec查询放在异步方法中，
            //dategrid 的workrecs查询后会导致dategrid 的selectitem变化-》Workrec的变化，导致下面的Workrec查询没起到作用
            if (Workrecs != null)
            {
                Workrec = Workrecs.FirstOrDefault();
            }
        }
        public async void QueryWorkBySerialNO()
        {
            await Task.Run(() =>
            {
                if (QuerySearialNO != null)
                {
                    using var db = new AliDbContext();
                    Workrecs = db.Workrecs.Where(w => w.SerialNo.Contains(QuerySearialNO)).OrderByDescending(w => w.LastTime).ToList();
                }
            });
            if (Workrecs != null)
            {
                Workrec = Workrecs.FirstOrDefault();
            }

        }
        protected override void OnViewLoaded()
        {           
                QueryWorkByDate();            
        }
        public async void QueryHole()
        {

            await Task.Run(() =>
            {
                using var db = new AliDbContext();
                Holerecs = db.Holerecs.Where(h => h.SerialNo == Workrec.SerialNo).ToList();
            });
            MyModel.Title = "工件" + Workrec.SerialNo + "曲线";
            MyModel.Series.Clear();
            for (int i = 0; i < Holerecs.Count; i++)
            {
                List<DataPoint> DataPoints = new List<DataPoint>();
                List<Models.Point> points = new List<Models.Point>();
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
            if (Holerecs != null)
            {
                Holerec = Holerecs.FirstOrDefault();
                if (Workrec != null) HoleGridSelectChange();
            }
            MyModel.InvalidatePlot(true);
        }
        public void WorkGridSelectChange()
        {
            if (Workrec != null) QueryHole();
        }
        public void HoleGridSelectChange()
        {
            if (Holerec != null)
            {
                List<DataPoint> DataPoints = new List<DataPoint>();
                List<Models.Point> points = new List<Models.Point>();
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
                SingleModel.Title = "工件" + Workrec.SerialNo + "洞" + Holerec.HoleNumber.ToString() + "曲线";
                SingleModel.InvalidatePlot(true);
            }
        }
        public async void DeleteWorkRec()
        {

            if (Workrec != null && _windowManager.ShowMessageBox("确认要删除工件号为" + Workrec.SerialNo +
                "的记录?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await Task.Run(() =>
                {
                    var db = new AliDbContext();
                    var temph = db.Holerecs.Where(h => h.SerialNo == Workrec.SerialNo).ToList();
                    db.Workrecs.Remove(Workrec);
                    db.Holerecs.RemoveRange(temph);
                    db.SaveChanges();
                });
                Query();
            }
        }
        public async void DeleteHoleRec()
        {
            if (Holerec != null && _windowManager.ShowMessageBox("确认要删除工件" + Workrec.SerialNo + "洞号" + Holerec.HoleNumber +
                "的记录?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await Task.Run(() =>
                {
                    var db = new AliDbContext();
                    db.Holerecs.Remove(Holerec);
                    db.SaveChanges();
                });
                QueryHole();
            }
        }

        public async void ExportExcel()
        {
            //导出曲线为图片
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "File (*.xls)|*.xls",
                Title = $"导出工件{Workrec.SerialNo}记录",
                InitialDirectory = Global.SystemPara.SavePath,
                FileName = Workrec.SerialNo
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Global.SystemPara.SavePath = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("\\") + 1);
                var fileName = Global.SystemPara.SavePath + Workrec.SerialNo + ".xls";
                var pictureName = Global.SystemPara.SavePath + Workrec.SerialNo + "hole" + Holerec.HoleNumber + ".png";
                var pngExporter = new PngExporter { Width = 600, Height = 800, Background = OxyColors.White };
                pngExporter.ExportToFile(SingleModel, pictureName);
                CommonMethods.WriteConfig();
                CanExportExcel = false;
                await Task.Run(() =>
                {
                    if (Workrec != null && Holerecs.Count > 0 && CommonMethods.GetFilePath(dialog.FileName))
                    {
                        for (int i = 1; i < Holerecs.Count + 1; i++)
                        {
                            var points = new List<Models.Point>();
                            points = CommonMethods.StringToList(Holerecs[i - 1].Data);
                            NopiExcelHelper<Models.Point>.AddExcel(points, fileName, "hole" + i.ToString(),
                                Holerecs[i - 1].MaxPressure);
                        }
                    }
                });
                CanExportExcel = true;
            }

        }
    }
}
