using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace DrillTestCore.Models
{
    public partial class Holerec : INotifyPropertyChanged
    {
        public long Id { get; set; }
        public string SerialNo { get; set; }
        public short HoleNumber { get; set; }
        public DateTime TestTime { get; set; }
        public string Data { get; set; }
        public float MaxPressure { get; set; }
        public short MacId { get; set; }
        public short LayerNo { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
