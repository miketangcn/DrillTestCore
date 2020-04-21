using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Stylet;

namespace DrillTestCore.Pages
{
    public class Subscriber : IHandle<ConnectStatusEvent>, INotifyPropertyChanged
    {
        public  string _connectStatus1 { get; set; }
        public string _connectStatus2 { get; set; }
        public Subscriber(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Handle(ConnectStatusEvent Event)
        {
            _connectStatus1 = Event._connectStatus1;
            _connectStatus2 = Event._connectStatus2;
        }
    }
}
