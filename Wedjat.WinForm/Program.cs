using AntdUI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wedjat.BLL;
using Wedjat.DAL;
using Wedjat.Model;



namespace Wedjat.WinForm
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                AppDbContext.SyncAllTables(); 
                Debug.WriteLine("数据库初始化成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } 
            Application.Run(new FormLogin());
        }
    }
}
