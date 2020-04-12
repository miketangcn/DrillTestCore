using System;
using System.Collections.Generic;
using System.Text;

namespace DrillTestCore.Models
{
   public  class SystemPara
    {
        #region 系统设置参数
        public  string IP1 { get; set; }
        public  string IP2 { get; set; }
        public  int con_chek_x { get; set; }
        public  int con_chek_y { get; set; }
        public  int min_x { get; set; }
        public  int min_y { get; set; }
        public  int max_x { get; set; }
        public  int max_y { get; set; }
        public  float con_factor_x { get; set; }
        public  float con_factor_y { get; set; }
        public  string SavePath { get; set; }
        public  string OutputPath { get; set; }
        public  bool AutoOut { get; set; }
        #endregion
    }
}
