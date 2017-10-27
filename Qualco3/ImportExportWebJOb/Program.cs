using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.File;
using System.IO;

namespace ImportExportWebJOb
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            try
            {
                var allCitDebtsLines = new string[10000000];
                var allSettlementsLines = new string[10000000];
                var allPaymentLines = new string[10000000];
                //while (true)
                //{
                //    if (DateTime.Now.AddHours(3).ToShortTimeString() == "12:00 PM")
                //    {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
    CloudConfigurationManager.GetSetting("AzureWebJobsStorage1"));

                CloudFile fileCitDebts = null,fileSetllements=null,filePayments=null;
                CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
                CloudFileShare fileShare = fileClient.GetShareReference("import");
                if (fileShare.Exists())
                {
                    CloudFileDirectory rootDir = fileShare.GetRootDirectoryReference();
                    fileCitDebts = rootDir.GetFileReference("CitizenDebts_1M.txt");
                    fileSetllements = rootDir.GetFileReference("SETTLEMENTS_20171003.txt");
                    filePayments = rootDir.GetFileReference("PAYMENTS_20171003.txt");


                    //TODO: in development use file = rootDir.GetFileReference($"PAYMENTS_{DateTime.Now.ToString("yyyyMMdd")}.txt");
                    //      or file = rootDir.GetFileReference($"PAYMENTS_{DateTime.Now.AddDays(-1).ToString("yyyyMMdd")}.txt");
                    if (fileCitDebts.Exists() && filePayments.Exists() && fileSetllements.Exists())
                    {
                        
                        using (var stream = fileCitDebts.OpenRead())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string s = String.Empty;
                                int x = 0;
                                while ((s = reader.ReadLine()) != null)
                                {
                                    allPaymentLines[x] = reader.ReadLine();
                                    x++;
                                }
                            }

                        }
                        if (allPaymentLines.Length <= 10000000)
                        {
                            doStuffLowerThanMillion(allPaymentLines);
                        }

                        //for (int i = 1; i < allPaymentLines.Length; i++)
                        //{
                        //    ReadingAndProcessingLinesFromFile_DoStuff(allPaymentLines[i]);
                        //}

                    }
                }
                //}
                //    allLines = new string[10000000];
                //}
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Main();
            }

        }

        private static void doStuffLowerThanMillion(string[] allPaymentLines)
        {
            int end=0, start = 0;
            
        }

        private static void ReadingAndProcessingLinesFromFile_DoStuff(string v)
        {
            try
            {
                if (!string.IsNullOrEmpty(v)) { Console.WriteLine(v); }
                else return;
            }
            catch (Exception ex)
            {


            }
            finally
            {
                Main();
            }
        }
    }
}