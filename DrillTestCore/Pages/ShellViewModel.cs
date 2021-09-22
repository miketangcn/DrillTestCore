using Stylet;
using DrillTestCore.Models;
using System.Linq;
using StyletIoC;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using DrillTestCore.Lib;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace DrillTestCore.Pages
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive
    {
        private readonly IContainer _container;
        private readonly IWindowManager _windowManager;
        private readonly TestViewModel _testView;
        private readonly HistoryViewModel _historyView;
        private readonly NetHistoryViewModel _netHistoryView;
        public string Status { get; set; }          
       

        public bool CanUploadData { get; set; }//上载数据到服务器使能
        public bool CanNetBatchExport { get; set; }
        public bool CanLocalBatchExport { get; set; }
        public bool CanAbout { get; set; }
        public Subscriber _subscriber { get; set; }//模块连接状态消息订阅
        public bool AutoOut { get; set; }

        public ShellViewModel(IContainer container, IWindowManager windowManager,  IEventAggregator eventAggregator)
        {
            _container = container;
            _windowManager = windowManager;
            _historyView = _container.Get<HistoryViewModel>(); ;
            _netHistoryView = _container.Get<NetHistoryViewModel>(); ;
            _subscriber = new Subscriber(eventAggregator);
            _testView = _container.Get<TestViewModel>();
            CanAbout=CanUploadData = true;
            CanLocalBatchExport = false;
            CanNetBatchExport = false;
            this.Items.Add(_testView);
            this.Items.Add(_historyView);
            this.Items.Add(_netHistoryView);
            this.ActiveItem = _testView;
            Global.ViewNubmer = 1;
            CommonMethods.ReadConfig();
            AutoOut = Global.SystemPara.AutoOut;
            Global.SystemPara.OutputPath = Directory.GetCurrentDirectory() + @"\data\";
            CommonMethods.WriteConfig();
        }

        protected override void OnViewLoaded()
        {
            if (!HslCommunication.Authorization.SetAuthorizationCode("6803a1a2-d614-40b4-b85b-52aee4dce075"))
            {
                _windowManager.ShowMessageBox("授权失败！当前程序只能使用8小时！");
                
            }
        }

        public void ShowHistoryView()
        {
            //var HistoryViewModel = _container.Get<HistoryViewModel>();
            ActivateItem(_historyView);
            CanLocalBatchExport = true;
            CanNetBatchExport = false;
            Global.ViewNubmer = 2;
        }
        public void ShowTestView()
        {
            //var TestViewModel = _container.Get<TestViewModel>();
            ActivateItem(_testView);
            CanLocalBatchExport = false;
            CanNetBatchExport = false;
            Global.ViewNubmer = 1;
        }
        public void ShowUploadMangementView()
        {
           // ActivateItem(_uploadMangementView);
        }
        public void ShowNetHistoryView()
        {
            ActivateItem(_netHistoryView);
            CanLocalBatchExport = false;
            CanNetBatchExport = true;
            Global.ViewNubmer = 3;
        }
        //上传数据窗口
        public void UploadData()
        {
            CanUploadData = false;
            var uploadSelectViewModel = _container.Get<UploadSelectViewModel>();//IOC容器绑定model，新建窗口
            _windowManager.ShowDialog(uploadSelectViewModel);
            CanUploadData = true;
        }

        public void ChangeAutoOut()
        {
            Global.SystemPara.AutoOut = !Global.SystemPara.AutoOut;
            AutoOut = Global.SystemPara.AutoOut;
            CommonMethods.WriteConfig();
        }

        public async Task NetBatchExport()
        {
            if (_windowManager.ShowMessageBox("批量导出当前工件数据到Excel文件", "导出数据", MessageBoxButton.OKCancel,
                MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                CanNetBatchExport = false;
                await Task.Run(() =>
                {
                    if (_netHistoryView.Workrecs.Count > 0)
                    {
                        foreach (var workerec in _netHistoryView.Workrecs)
                        {
                           var fileName = Global.SystemPara.OutputPath +"Net"+ workerec.SerialNo + ".xls";
                            using var db = new AliDbContext();
                            var holerecs = db.Holerecs.Where(h => h.SerialNo == workerec.SerialNo).ToList();
                            if (holerecs.Count > 0 && CommonMethods.GetFilePath(fileName))
                            {
                                Status = "正在导出工件" + workerec.SerialNo + "数据";
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
                CanNetBatchExport = true;
                Status = "";
            }

        }
        public async Task LocalBatchExport()
        {
            if (_windowManager.ShowMessageBox("批量导出当前工件数据到Excel文件", "导出数据", MessageBoxButton.OKCancel,
                    MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    CanLocalBatchExport = false;
                    await Task.Run(() =>
                    {
                        if (_historyView.Workrecs.Count > 0)
                        {
                            foreach (var workerec in _historyView.Workrecs)
                            {
                                var fileName = Global.SystemPara.OutputPath + "Net" + workerec.SerialNo + ".xls";
                                using var db = new DrillContext();
                                var holerecs = db.Holerec.Where(h => h.SerialNo == workerec.SerialNo).ToList();
                                if (holerecs.Count > 0 && CommonMethods.GetFilePath(fileName))
                                {
                                    Status = "正在导出工件" + workerec.SerialNo + "数据";
                                    for (int i = 1; i < _historyView.Holerecs.Count + 1; i++)
                                    {
                                        var points = new List<Models.Point>();
                                        points = CommonMethods.StringToList(_historyView.Holerecs[i - 1].Data);
                                        NopiExcelHelper<Models.Point>.AddExcel(points, fileName, "hole" + i.ToString(),
                                            _historyView.Holerecs[i - 1].MaxPressure);
                                    }
                                }
                            }
                        }
                    });
                    CanLocalBatchExport = true; 
                    Status = "";
                }
        }
        public void About()
        {
            var aboutView = _container.Get<AboutViewModel>();
            CanAbout = false;
            _windowManager.ShowDialog(aboutView);
            CanAbout = true;
        }
    }
}
