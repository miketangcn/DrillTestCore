using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DrillTestCore.Models
{
    [Serializable]
  public class Point
    {
        public int x { get; set; }
        public int y { get; set; }
    }
    public class ActualPoint : INotifyPropertyChanged
    {
        public float x { get; set; }
        public float y { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
