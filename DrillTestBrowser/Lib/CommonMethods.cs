using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using DrillTestBrowser.Models;
using Microsoft.Extensions.Configuration;

namespace DrillTestBrowser.Lib
{
    public static class CommonMethods
    {
        //模板文件全路径
        private static string ModelFileName = Directory.GetCurrentDirectory()+ @"\Template\Template.xls";

        #region 读写配置文件
        public static void ReadConfig()
        {
            var Path = System.IO.Directory.GetCurrentDirectory();
            var JsonName = Path + @"\appsettings.json";
            var JsonHelper = new JsonFileHelper(JsonName);
            Global.ConnectionStrings = JsonHelper.GetConnectionStrings();
            Global.SystemPara = JsonHelper.Read<SystemPara>("AppSettings");

        }
        public static void WriteConfig()
        {
            var Path = Directory.GetCurrentDirectory();
            var JsonName = Path + @"\appsettings.json";
            var JsonHelper = new JsonFileHelper(JsonName);
            JsonHelper.Write<SystemPara>("AppSettings", Global.SystemPara);

        }
        #endregion

        #region 新建文件夹及新建文件
        public static bool GetFilePath(string FileName)
        {
            if (!Directory.Exists(Path.GetDirectoryName(FileName)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FileName));
                    if (CopyExcelModel(FileName))
                        return true;
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                if (CopyExcelModel(FileName))
                    return true;
                else return false;

            }
        }
        /// <summary>
        /// 拷贝模板到目标文件
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool CopyExcelModel(string FileName)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(ModelFileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ModelFileName));
                }
                if (!File.Exists(FileName))
                {
                    if (File.Exists(ModelFileName))
                    {
                        File.Copy(ModelFileName, FileName);
                        return true;
                    }
                    else return false;
                }
                else return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion


        #region 数据库及数据文件处理

        public static void AddWorkrec(Workrec workRecord)
        {
            var _workRecord = new Workrec()
            {
                SerialNo = workRecord.SerialNo,
                Layer = workRecord.Layer,
                HoleCount = workRecord.HoleCount,
                LastTime = workRecord.LastTime
            };


            using (var db = new DrillContext())
            {
                //int i = db.Workrec.Where(w => w.SerialNo == workRecord.SerialNo).Count();
                var temp = db.Workrec.Where(w => w.SerialNo == workRecord.SerialNo).FirstOrDefault();
                if (temp != null)
                {
                    temp.LastTime = _workRecord.LastTime;
                    temp.HoleCount = (short)(_workRecord.HoleCount);
                    db.Workrec.Update(temp);
                    db.SaveChanges();
                }
                else
                {
                    db.Workrec.Add(_workRecord);
                    db.SaveChanges();
                }
                    
            }

        }
        public static void AddHolerec(Holerec holeRecod)
        {
            var _holeRecord = new Holerec()
            {
                Data = holeRecod.Data,
                TestTime = holeRecod.TestTime,
                MaxPressure = holeRecod.MaxPressure,
                MacId = holeRecod.MacId,
                SerialNo = holeRecod.SerialNo,
                HoleNumber = holeRecod.HoleNumber,
                LayerNo = holeRecod.LayerNo
            };

            using var db = new DrillContext();
            var temp = db.Holerec.Where(h => h.SerialNo == holeRecod.SerialNo && h.HoleNumber == holeRecod.HoleNumber).FirstOrDefault();
            if (temp != null)
            {
                temp.Data = holeRecod.Data;
                temp.TestTime = holeRecod.TestTime;
                temp.MaxPressure = holeRecod.MaxPressure;
                temp.MacId = holeRecod.MacId;
                db.Holerec.Update(temp);
            }
            else db.Holerec.Add(_holeRecord);
            db.SaveChanges();
        }
        #endregion

        #region list对象序列化到字符串和字符串反序列化
        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Compress(byte[] inputBytes)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(inputBytes, 0, inputBytes.Length);
                    zipStream.Close(); //很重要，必须关闭，否则无法正确解压
                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 解压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Decompress(byte[] inputBytes)
        {

            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Close();
                        return outStream.ToArray();
                    }
                }

            }
        }
        public static string ListToString(List<Point> list)
        {

            byte[] vs = new byte[list.Count * 4];
            for (int i = 0; i < list.Count; i++)
            {
                vs[4 * i] = Convert.ToByte(list[i].x >> 8);
                vs[4 * i + 1] = Convert.ToByte(list[i].x & 0x00FF);
                vs[4 * i + 2] = Convert.ToByte(list[i].y >> 8);
                vs[4 * i + 3] = Convert.ToByte(list[i].y & 0x00FF);
            }
            byte[] compressAfterByte = Compress(vs);
            return Convert.ToBase64String(compressAfterByte);

        }
        public static List<Point> StringToList(string Data)
        {
            if (string.IsNullOrEmpty(Data))
                return null;
            byte[] bytes = Convert.FromBase64String(Data);
            byte[] vs = Decompress(bytes);
            List<Point> points = new List<Point>();
            for (int i = 0; i < vs.Length / 4; i++)
            {
                Point point = new Point();
                point.x = Convert.ToInt16((vs[4 * i] << 8) | vs[4 * i + 1]);
                point.y = Convert.ToInt16((vs[4 * i + 2] << 8) | vs[4 * i + 3]);
                points.Add(point);
            }
            return points;
        }

        #endregion
    }
}