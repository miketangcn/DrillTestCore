using Stylet;
using DrillTestCore.Models;
using System.Linq;
using StyletIoC;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using DrillTestCore.Lib;

namespace DrillTestCore.Pages
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive
    {
        private readonly IContainer _container;
        private readonly IWindowManager _windowManager;
        private readonly TestViewModel _testView;
        private readonly HistoryViewModel _historyView;
        public string testttt { get; set; }
        public ShellViewModel(IContainer container, IWindowManager windowManager, TestViewModel TestView, HistoryViewModel HistoryView)
        {
            _testView = TestView;
            _historyView = HistoryView;
            _container = container;
            _windowManager = windowManager;
            this.Items.Add(TestView);
            this.Items.Add(HistoryView);
            this.ActiveItem = _testView;
            CommonMethods.ReadConfig();
            testttt = "sdfasdfasdfas";           
        }
        
        protected override void OnViewLoaded()
        {
            //query();
            //ShowTestView();
        }

        public void ShowHistoryView()
        {
            //var HistoryViewModel = _container.Get<HistoryViewModel>();
            ActivateItem(_historyView);
        }
        public void ShowTestView()
        {
            //var TestViewModel = _container.Get<TestViewModel>();
            ActivateItem(_testView);
        }
        public void ShowReportView()
        {

        }

        public void query()
        {
            using (var db = new DrillContext())
            {
                var works = db.Workrec.ToList();
            }
        }
    }

}
