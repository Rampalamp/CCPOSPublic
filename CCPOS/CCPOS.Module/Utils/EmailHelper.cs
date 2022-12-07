using CCPOS.Module.BusinessObjects.Customer;
using CCPOS.Module.BusinessObjects.Sales;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace CCPOS.Module.Utils
{
    public class EmailHelper
    {
        private static void SetupClient(SmtpClient client)
        {
            NetworkCredential netCred = new NetworkCredential(
                ConfigurationManager.AppSettings["InfoEmail"],
                ConfigurationManager.AppSettings["InfoEmailCredential"]
                );
            client.Host = "smtp.live.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.Credentials = netCred;
            client.EnableSsl = true;
        }

        public static bool SendInvoiceEmail(Sale sale)
        {
            try
            {
                using (MailMessage message = new MailMessage() { From = new MailAddress("info@thewinebaroakville.com", "TheWineBarOakville") })
                {
                    //create html message
                    message.IsBodyHtml = true;
                    //add email addresses

                    //create smpt client and authenticate
                    using (SmtpClient client = new SmtpClient())
                    {
                        SetupClient(client);

                        message.To.Add(new MailAddress(sale.Customer.Email));

                        message.Subject = "The Wine Bar Invoice - Thank you for your business.";

                        //add body
                        //if FullName exists on customer, use that, if not use generic intro.
                        if (!String.IsNullOrWhiteSpace(sale.Customer?.FullName))
                        {
                            message.Body = String.Format("Dear {0},", sale.Customer?.FullName);
                        }
                        else
                        {
                            message.Body = "Dear Valued Customer,";
                        }
                        message.Body += "<br>";
                        message.Body += "<br>";
                        message.Body += "Please see the attached invoice for your recent purchase at The Wine Bar.";
                        message.Body += "<br>";
                        message.Body += "<br>";
                        message.Body += "Thank you for your business,";
                        message.Body += "<br>";
                        message.Body += "The Wine Bar";

                        //Attach invoice report
                        message.Attachments.Add(new Attachment(CreateInvoiceAttachmentFromSale(sale)));

                        client.Send(message);

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);

                return false;
            }

        }

        public static bool SendSMSToCustomer(Customer customer, string subject, string body)
        {
            try
            {
                using (MailMessage message = new MailMessage() { From = new MailAddress("info@thewinebaroakville.com", "TheWineBarOakville") })
                {
                    //create html message
                    message.IsBodyHtml = true;
                    //add email addresses

                    //create smpt client and authenticate
                    using (SmtpClient client = new SmtpClient())
                    {
                        SetupClient(client);

                        message.To.Add(new MailAddress(
                            String.Format("{0}@txt.bell.ca",
                            Regex.Replace(customer.PhoneNumber, @"[^\d]", ""))));

                        message.Subject = subject;
                        message.Body = body;

                        client.Send(message);

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);

                return false;
            }
        }

        public static string CreateInvoiceAttachmentFromSale(Sale sale)
        {
            using (IObjectSpace space = AppHelper.Application.ObjectSpaceProvider.CreateObjectSpace())
            {
                IList<Sale> appdata = new List<Sale>() { sale };
                ReportDataV2 reportdata;
                //Once we have a hydro ottawa specific one in place, can check for LDC and use accordingly.

                reportdata = space.FindObject<ReportDataV2>(new BinaryOperator("DisplayName", "Sale Invoice"));


                string localDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                string savePath = localDocs + @"\TheWineBar\Invoices\" + DateTime.Now.ToString("yyyy-dd-M");
                //create local dir if dont exist
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                using (XtraReport report = GetNewFilteredReport(reportdata, appdata, space))
                {
                    report.ExportToPdf(String.Format("{0}\\{1}.pdf", savePath, "Sale Invoice # " + sale.SaleNumber.Number.ToString()));

                    return (String.Format("{0}\\{1}.pdf", savePath, "Sale Invoice # " + sale.SaleNumber.Number.ToString()));
                }
            }
        }

        public static XtraReport GetNewFilteredReport(ReportDataV2 reportdata, IList<Sale> datasource, IObjectSpace space)
        {
            IReportContainer reportContainer = ReportDataProvider.ReportsStorage.GetReportContainerByHandle(ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportdata));
            XtraReport rpt = reportContainer.Report;
            rpt.DataSource = datasource;

            rpt.CreateDocument();

            rpt.BeginInit();

            return rpt;

        }
    }
}
