using Stylet;
using System;
using System.Collections.Generic;
using System.Text;
using DrillTestBrowser.Pages.NetHistoryView;
using StyletIoC;
using DrillTestBrowser.Lib;
using DrillTestBrowser.Pages.AboutView;
using NPOI.XSSF.Extractor;
using DrillTestBrowser.Models;
using System.IO;

namespace DrillTestBrowser.Pages.ShellView
{
    public class ShellViewModel:Conductor<Screen>.Collection.OneActive
    {
        private readonly IWindowManager _windowManager;
        private readonly IContainer _container;
        private readonly NetHistoryViewModel _historyViewModel;

        public ShellViewModel(IWindowManager windowManager,IContainer container)
        {
            _windowManager = windowManager;
            _container = container;
            _historyViewModel= container.Get<NetHistoryViewModel>();
            CommonMethods.ReadConfig();
            Global.SystemPara.OutputPath = Directory.GetCurrentDirectory() + @"\data\";
            CommonMethods.WriteConfig();
            this.Items.Add(_historyViewModel);
            this.ActiveItem = _historyViewModel;
        }



    }
}
