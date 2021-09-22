using System;
using System.Collections.Generic;
using System.Text;
using Stylet;

namespace DrillTestCore.Pages
{
    public  class AboutViewModel:Screen
    {
        public void Close()
        {
          this.RequestClose();
        }
    }
}
