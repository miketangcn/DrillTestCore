using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Stylet;

namespace DrillTestCore.Pages
{
    public class Subscriber : IHandle<MyEvent>, INotifyPropertyChanged
    {
        public  bool _enable { get; set; }
        public Subscriber(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Handle(MyEvent Event)
        {
            _enable = Event._enable;
        }
    }
}
