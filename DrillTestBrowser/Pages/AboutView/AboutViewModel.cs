using System;
using System.Collections.Generic;
using System.Text;
using Stylet;

namespace DrillTestBrowser.Pages.AboutView
{
    public  class AboutViewModel:Screen
    {
        public void Close()
        {
          this.RequestClose();
        }
    }
}
