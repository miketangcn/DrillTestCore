using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DrillTestCore.Models
{
    public partial class Workrec :INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string SerialNo { get; set; }
        public short Layer { get; set; }
        public short HoleCount { get; set; }
        public DateTime LastTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
