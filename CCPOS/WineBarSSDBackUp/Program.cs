using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WineBarSSDBackUp
{
    class Program
    {
        const string ssdPath = @"E:\TheWineBarSSD";
        const string wbFiles = @"C:\Users\TheWineBar\Documents\TheWineBar";
        const string ssdPathSQLBackUp = ssdPath + @"\DataBackUp";
        const string wbSQLBackUp = @"C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\Backup\TheWineBarLive.bak";
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        static void Main(string[] args)
        {
            try
            {
                var handle = GetConsoleWindow();
                // Hide
                ShowWindow(handle, SW_HIDE);
                if (Directory.Exists(wbFiles))
                {
                    if (Directory.Exists(ssdPath))
                    {
                        #region CopyLocalDocumentsToSSD
                        //Now Create all of the directories
                        foreach (string dirPath in Directory.GetDirectories(wbFiles, "*", SearchOption.AllDirectories))
                        {
                            Directory.CreateDirectory(dirPath.Replace(wbFiles, ssdPath));
                        }

                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(wbFiles, "*.*", SearchOption.AllDirectories))
                        {
                            File.Copy(newPath, newPath.Replace(wbFiles, ssdPath), true);
                        }
                        #endregion
                        #region CopyBakFile
                        if (!Directory.Exists(ssdPathSQLBackUp))
                        {
                            Directory.CreateDirectory(ssdPathSQLBackUp);
                        }
                        File.Copy(wbSQLBackUp, ssdPathSQLBackUp + @"\TheWineBarLive.bak", true);
                        #endregion

                        MessageBox.Show(@"Files Successfully Back Up To E:\TheWineBarSSD");
                    }
                    else
                    {
                        MessageBox.Show("SSD WineBar File Path Missing.");
                    }
                }
                else
                {
                    MessageBox.Show("Local WineBar File Path Missing.");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
    }
}
