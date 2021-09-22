using DrillTestCore.Models;
using Microsoft.EntityFrameworkCore;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrillTestCore.Pages
{
   public class UploadSelectViewModel:Screen
    {

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Indicate  { get; set; }
        private List<Workrec> Workrecs { get; set; }
        private List<Holerec> Holerecs { get; set; }

        private readonly IWindowManager _windowManager;
        public UploadSelectViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            StartTime = DateTime.Now.AddDays(-30);
            EndTime = DateTime.Now.AddDays(1);
            Indicate = "Hidden";
        }

        public async void Confirm()
        {
            try
            {
                await Task.Run(() =>
                {
                    Indicate = "Visible";
                    Workrecs = new List<Workrec>();
                    Holerecs = new List<Holerec>();
                    using (var db = new DrillContext())
                    {
                        Workrecs = db.Workrec.Where(w => w.LastTime >= StartTime && w.LastTime <= EndTime).AsNoTracking().ToList();
                        Holerecs = db.Holerec.Where(h => h.TestTime >= StartTime && h.TestTime <= EndTime).AsNoTracking().ToList();
                    }
                    using (var alidb = new AliDbContext())
                    {
                        var aliworkrecs = new List<Workrec>();
                        var aliholerecs = new List<Holerec>();
                        aliworkrecs = alidb.Workrecs.AsNoTracking().ToList();//一次性缓存
                        aliholerecs = alidb.Holerecs.AsNoTracking().ToList();//
                        foreach (var item in Workrecs)
                        {
                            //if (alidb.Workrecs.Any(w => w.Id == item.Id)==false)
                            if (aliworkrecs.Any(w => w.Id == item.Id) == false)
                            {
                                alidb.Workrecs.Add(item);
                            }
                            else
                            {
                                alidb.Workrecs.Update(item);
                            }
                        }
                        foreach (var item in Holerecs)
                        {
                            //if(alidb.Holerecs.Any(h => h.Id == item.Id) == false)
                            if (aliholerecs.Any(h => h.Id == item.Id) == false)
                            {
                                alidb.Holerecs.Add(item);
                            }
                            else
                            {
                                alidb.Holerecs.Update(item);
                            }
                        }
                        alidb.SaveChanges();
                    }
                });
                Indicate = "Hidden";
                _windowManager.ShowMessageBox("上传完成", "信息", MessageBoxButton.OK);
            }
            catch (Exception)
            {
                Indicate = "Hidden";
                _windowManager.ShowMessageBox("网络数据库连接失败！", "警告");
                
            }
           
        }
        public void Cancel()
        {
            this.RequestClose(false);
        }
    }
}
