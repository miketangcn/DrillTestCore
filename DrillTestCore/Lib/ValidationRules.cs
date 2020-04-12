using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DrillTestCore.Lib
{
    public class SerialRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex SerialReg = new Regex(@"^[A-Za-z0-9-]{4,16}$");
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "工件号不能为空");
            }
            else if (!SerialReg.IsMatch(value.ToString()))
            {
                return new ValidationResult(false, "工件号格式为3位以上字母和数字及符号-的组合");
            }
            return new ValidationResult(true, null);
        }
    }
    public class LayerRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex LayerReg = new Regex(@"^([1-9][0-9]{1,2})$");
            if (!LayerReg.IsMatch(value.ToString()))
            {
                return new ValidationResult(false, "请输入100以内数字");
            }
            return new ValidationResult(true, null);
        }
    }

}

