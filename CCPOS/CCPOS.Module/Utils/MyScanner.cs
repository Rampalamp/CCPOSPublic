using HidLibrary;
using System;
using System.Text;
using System.Windows.Forms;

namespace CCPOS.Module.Utils
{
    public class MyScanner
    {

        public static HidDevice Scanner { get; set; }

        public static string ScanTextValue { get; set; }

        public static void SetScanTextValue(byte[] data)
        {
            //Data received from scanner device is messy, need to split into another array using \, should always be the same index I believe.
            try
            {
                var rawData = Encoding.Default.GetString(data);

                if (rawData == null) return;
                //loop through and remove any Control Characters. The hand scanner returns a load of em.
                StringBuilder cleanData = new StringBuilder();
                char ch;
                for (int i = 0; i < rawData.Length; i++)
                {
                    ch = rawData[i];
                    if (!char.IsControl(ch))
                    {
                        cleanData.Append(ch);
                    }
                }

                ScanTextValue = cleanData.ToString();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }


    }
}
