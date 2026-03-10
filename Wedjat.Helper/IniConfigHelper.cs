using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Helper
{

        public class IniConfigHelper
        {
            #region API函数声明
            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
            [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
            private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
            [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
            private static extern uint GetPrivateProfileStringA(string section, string key, string def, Byte[] retVal, int size, string filePath);
            #endregion
            [Description("读取所有的Section")]
            public static List<string> ReadSections(string iniFilename)
            {
                List<string> result = new List<string>();
                Byte[] buf = new Byte[65536];
                uint len = GetPrivateProfileStringA(null, null, null, buf, buf.Length, iniFilename);
                int j = 0;
                for (int i = 0; i < len; i++)
                {
                    if (buf[i] == 0)
                    {
                        result.Add(Encoding.Default.GetString(buf, j, i - j));
                        j = i + 1;
                    }
                }
                return result;
            }
            [Description("读取某个Section下所有的key")]
            public static List<string> Readkeys(string SectionName, string iniFilename)
            {
                List<string> result = new List<string>();
                Byte[] buf = new Byte[65536];
                uint len = GetPrivateProfileStringA(SectionName, null, null, buf, buf.Length, iniFilename);
                int j = 0;
                for (int i = 0; i < len; i++)
                {
                    if (buf[i] == 0)
                    {
                        result.Add(Encoding.Default.GetString(buf, j, i - j));
                        j = i + 1;
                    }
                }
                return result;
            }

            [Description("读取某个Section下某个key对应的Value")]
            public static string ReadIniData(string Section, string Key, string DefaultValue, string iniFilePath)
            {
                if (File.Exists(iniFilePath))
                {
                    StringBuilder temp = new StringBuilder(1024);
                    GetPrivateProfileString(Section, Key, DefaultValue, temp, 1024, iniFilePath);
                    return temp.ToString();
                }
                return string.Empty;
            }

            [Description("写入某个Section下某个key对应的Value")]
            public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }

