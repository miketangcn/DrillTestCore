using System;
using System.Collections.Generic;
using System.Text;
using Stylet;
using System.ComponentModel;

namespace DrillTestBrowser.Models
{
    public class RadioButtonModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RadioButtonModel()
        {

        }
        public string Content { get; set; }
        public bool IsCheck{ get; set; }
    }
}
