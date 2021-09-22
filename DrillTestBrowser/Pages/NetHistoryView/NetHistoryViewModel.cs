using System;
using System.Collections.Generic;
using DrillTestBrowser.Models;
using OxyPlot;
using Stylet;
using System.Threading.Tasks;
using System.Linq;
using OxyPlot.Axes;
using System.Windows;
using DrillTestBrowser.Lib;
using Microsoft.Win32;
using StyletIoC;
using DrillTestBrowser.Pages.AboutView;

namespace DrillTestBrowser.Pages.NetHistoryView
{
    public class NetHistoryViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly IContainer _container;
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
        public string Status { get; set; }
        public string ConnectStatus { get; set; }
        public int RecordsCount { get; set; }
        public bool CanQuery { get; set; }
        public bool CanSingleExport { get; set; }
        public bool CanBatchExport { get; set; }

        public NetHistoryViewModel(IWindowManager windowManager, IContainer container)
        { 
            _windowManager = windowManager;
            _container = container;
            MyModel = new PlotModel {Title = "Work Charts"};

            MyModel.Axes.Add(new LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Minimum = 180,
                Maximum = 350,
                Title = "Distance(mm)",
                TextColor = OxyColor.FromRgb(47, 79, 79),
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245),
                IsZoomEnabled = false
            });
            MyModel.Axes.Add(new LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Minimum = 0,
                Maximum = 10,
                Title = "Pressure(MP)",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245),
                IsZoomEnabled = false
            });

            SingleModel = new PlotModel {Title = "Single Hole Chart"};

            SingleModel.Axes.Add(new LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Minimum = 180,
                Maximum = 350,
                Title = "Distance(mm)",
                TextColor = OxyColor.FromRgb(47, 79, 79),
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245),
                IsZoomEnabled = false
            });
            SingleModel.Axes.Add(new LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Minimum = 0,
                Maximum = 10,
                Title = "Pressure(MP)",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(245, 245, 245),
                IsZoomEnabled = false
            });

            RdoQueryType = new List<RadioButtonModel>()
            {
                new RadioButtonModel {Content = "Date Mode", IsCheck = true},
                new RadioButtonModel {Content = "Work Mode", IsCheck = false},
            };
            string year = (DateTime.Now.Year).ToString() + "0101";
            StartTime = DateTime.ParseExact(year, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            EndTime = DateTime.Now.AddDays(1);
            CanSingleExport =CanBatchExport= CanQuery=true;
            //QueryWorkByDate();

        }

        public void Query()
        {
            RdoQuery = RdoQueryType.Where(q => q.IsCheck == true).FirstOrDefault();
            if (RdoQuery.Content == "Date Mode")
                QueryWorkByDate();
            else QueryWorkBySerialNo();

        }

        public async void QueryWorkByDate()
        {
            try
            {
                await Task.Run(() =>
                {
                    using var db = new DrillContext();
                    Workrecs = db.Workrec.Where(w => w.LastTime >= StartTime && w.LastTime <= EndTime)
                        .OrderByDescending(w => w.LastTime).ToList();
                    RecordsCount = Workrecs.Count;
                    //if (Workrec != null) QueryHole();  //Workrecs变化后会触发dategrid的selectchange 事件 会触发 queryhole 过程
                });
                ConnectStatus = "Success!";
            }
            catch (Exception)
            {
                ConnectStatus = "failure!";
                _windowManager.ShowMessageBox("Connect Net DataBase Failure！", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            //Workrec与dategrid 的selectitem绑定 如果将下面的Workrec查询放在异步方法中，
            //dategrid 的workrecs查询后会导致dategrid 的selectitem变化-》Workrec的变化，导致下面的Workrec查询没起到作用
            if (Workrecs != null)
            {
                Workrec = Workrecs.FirstOrDefault();
            }
        }

        public async void QueryWorkBySerialNo()
        {
            try
            {
                await Task.Run(() =>
                {
                    if (QuerySearialNO != null)
                    {
                        using var db = new DrillContext();
                        Workrecs = db.Workrec.Where(w => w.SerialNo.Contains(QuerySearialNO))
                            .OrderByDescending(w => w.LastTime).ToList();
                        RecordsCount = Workrecs.Count;
                    }
                });
                ConnectStatus = "Success!";
            }
            catch (Exception e)
            {
                ConnectStatus = "failure!";
                _windowManager.ShowMessageBox("Connect Net DataBase Failure！", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
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
                using var db = new DrillContext();
                Holerecs = db.Holerec.Where(h => h.SerialNo == Workrec.SerialNo).ToList();
            });
            MyModel.Title = "Work" + Workrec.SerialNo + "'s Chart";
            MyModel.Series.Clear();
            for (int i = 0; i < Holerecs.Count; i++)
            {
                List<DataPoint> DataPoints = new List<DataPoint>();
                List<Models.Point> points = new List<Models.Point>();
                points = CommonMethods.StringToList(Holerecs[i].Data);
                for (int j = 0; j < points.Count; j++)
                {
                    var temppoint = new DataPoint(points[j].x * Global.SystemPara.con_factor_x,
                        points[j].y * Global.SystemPara.con_factor_y);
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
                    var temppoint = new DataPoint(points[j].x * Global.SystemPara.con_factor_x,
                        points[j].y * Global.SystemPara.con_factor_y);
                    DataPoints.Add(temppoint);
                }

                OxyPlot.Series.LineSeries lineSeries = new OxyPlot.Series.LineSeries();
                lineSeries.Points.AddRange(DataPoints);
                lineSeries.Color = OxyColor.FromRgb(255, 0, 0);
                lineSeries.StrokeThickness = 1;
                lineSeries.TrackerFormatString = "{0}\n{1}: {2:0.#}mm\n{3}: {4:0.##}Mpa";
                SingleModel.Series.Clear();
                SingleModel.Series.Add(lineSeries);
                SingleModel.Title = "Work" + Workrec.SerialNo + " Hole" + Holerec.HoleNumber.ToString() + "'s Chart";
                SingleModel.InvalidatePlot(true);
            }
        }

        public async void DeleteWorkRec()
        {

            if (Workrec != null && _windowManager.ShowMessageBox("Confirm to delete the record of the work" + Workrec.SerialNo ,
                "Warning", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await Task.Run(() =>
                {
                    var db = new DrillContext();
                    var temph = db.Holerec.Where(h => h.SerialNo == Workrec.SerialNo).ToList();
                    db.Workrec.Remove(Workrec);
                    db.Holerec.RemoveRange(temph);
                    db.SaveChanges();
                });
                Query();
            }
        }

        public async void DeleteHoleRec()
        {
            if (Holerec != null && _windowManager.ShowMessageBox(
                "Confirm to Delete the record of the work" + Workrec.SerialNo + "Hole" + Holerec.HoleNumber ,
                "Waring", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await Task.Run(() =>
                {
                    var db = new DrillContext();
                    db.Holerec.Remove(Holerec);
                    db.SaveChanges();
                });
                QueryHole();
            }
        }
        public async void SingleExport()
        {
            
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "File (*.xls)|*.xls",
                Title = $"导出工件{Workrec.SerialNo}记录",
                InitialDirectory = Global.SystemPara.SavePath,
                FileName = Workrec.SerialNo
            };

            if (dialog.ShowDialog() == true)
            {
                Global.SystemPara.SavePath = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("\\") + 1);
                var fileName = Global.SystemPara.SavePath + Workrec.SerialNo + ".xls";
                CommonMethods.WriteConfig();
                CanSingleExport = false;
                await Task.Run(() =>
                {
                    if (Workrec != null && Holerecs.Count > 0 && CommonMethods.GetFilePath(dialog.FileName))
                    {
                        for (var i = 1; i < Holerecs.Count + 1; i++)
                        {
                            var points = new List<Models.Point>();
                            points = CommonMethods.StringToList(Holerecs[i - 1].Data);
                            NopiExcelHelper<Models.Point>.AddExcel(points, fileName, "hole" + i.ToString(),
                                Holerecs[i - 1].MaxPressure);
                        }
                    }
                });
                CanSingleExport = true;
            }

        }

        public async void BatchExport()
        {
            if (_windowManager.ShowMessageBox("Batch Export works' data to Excel", "Export data", MessageBoxButton.OKCancel,
                MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                CanBatchExport = CanQuery= false;
                await Task.Run(() =>
                {
                    if (Workrecs.Count > 0)
                    {
                        foreach (var workerec in Workrecs)
                        {
                            var fileName = Global.SystemPara.OutputPath + workerec.SerialNo + ".xls";
                            using var db = new DrillContext();
                            var holerecs = db.Holerec.Where(h => h.SerialNo == workerec.SerialNo).ToList();
                            if (holerecs.Count > 0 && CommonMethods.GetFilePath(fileName))
                            {
                                Status = "Exporting work" + workerec.SerialNo + "'s data";
                                for (int i = 1; i < holerecs.Count + 1; i++)
                                {
                                    var points = new List<Models.Point>();
                                    points = CommonMethods.StringToList(holerecs[i - 1].Data);
                                    NopiExcelHelper<Models.Point>.AddExcel(points, fileName, "hole" + i.ToString(),
                                        holerecs[i - 1].MaxPressure);
                                }
                            }
                        }
                    }
                });
                CanBatchExport = CanQuery= true;
                Status = "";
            }
        }
        public void About()
        {
            var aboutView = _container.Get<AboutViewModel>();
            _windowManager.ShowDialog(aboutView);
        }

    }
}
