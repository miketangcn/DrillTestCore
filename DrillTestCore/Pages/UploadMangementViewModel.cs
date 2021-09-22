using DrillTestCore.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillTestCore.Pages
{
   public  class UploadMangementViewModel:Screen
    {
        public List<RadioButtonModel> RdoQueryType { get; set; }
        public RadioButtonModel RdoQuery { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string QuerySearialNO { get; set; }
        public List<Workrec> Workrecs { get; set; }
        public List<Holerec> Holerecs { get; set; }
        public Workrec Workrec { get; set; }
        public Holerec Holerec { get; set; }

        public  UploadMangementViewModel()
        {
            RdoQueryType = new List<RadioButtonModel>()
            {
               new RadioButtonModel{Content="时间方式",IsCheck=true},
               new RadioButtonModel{Content="工件号方式",IsCheck=false},
            };
            string year = (DateTime.Now.Year - 1).ToString() + "0101";
            StartTime = DateTime.ParseExact(year, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            EndTime = DateTime.Now.AddDays(1);
        }
        public  void Query()
        {
            RdoQuery = RdoQueryType.Where(q => q.IsCheck == true).FirstOrDefault();
            if (RdoQuery.Content == "时间方式")
                QueryWorkByDate();
            else QueryWorkBySerialNO();
        }
        public async void QueryWorkByDate()
        {
            await Task.Run(() =>
            {
                using (var db = new DrillContext())
                {
                    Workrecs = db.Workrec.Where(w => w.LastTime >= StartTime && w.LastTime <= EndTime).OrderByDescending(w => w.LastTime).ToList();
                    if (Workrecs != null)
                    {
                        Workrec = Workrecs.FirstOrDefault();
                    }
                    Holerecs = new List<Holerec>();
                    Holerecs.Clear();
                    foreach (var item in Workrecs)
                    {
                        var temp = db.Holerec.Where(h => h.SerialNo == item.SerialNo).ToList();
                        Holerecs.AddRange(temp);
                    }
                };

                int i = Holerecs.Count;
                // QueryHole();
            });

        }
        public async void QueryWorkBySerialNO()
        {
            await Task.Run(() =>
            {
                using var db = new DrillContext();
                if (QuerySearialNO != null)
                {                   
                    Workrecs = db.Workrec.Where(w => w.SerialNo.Contains(QuerySearialNO)).OrderByDescending(w => w.LastTime).ToList();
                }
                if (Workrecs != null)
                {
                    Workrec = Workrecs.FirstOrDefault();
                }
                Holerecs.Clear();
                foreach (var item in Workrecs)
                {
                    Holerecs.AddRange(db.Holerec.Where(h => h.SerialNo == item.SerialNo).ToList());
                }
                // QueryHole();
            });
        }
    }
}
