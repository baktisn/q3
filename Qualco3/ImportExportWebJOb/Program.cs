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
                while (true)
                {
                    if (DateTime.Now.AddHours(3).ToShortTimeString() == "12:00 PM")
                    {
                        List<string> fileNames = new List<string>();
                        List<string[]> lstCloudFilesdata = new List<string[]>();
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage1"));
                        CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
                        CloudFileShare fileShare = fileClient.GetShareReference("import");
                        //looks for a file share in the cloud
                        if (fileShare.Exists())
                        {
                            List<CloudFile> lstCloudFiles = new List<CloudFile>();
                            CloudFileDirectory rootDir = fileShare.GetRootDirectoryReference();
                            //for each file in my fileshare
                            foreach (CloudFile fiile in rootDir.ListFilesAndDirectories())
                            {
                                //if the file exists
                                if (fiile.Exists())
                                {
                                    //adds new datasting array
                                    ReadData(lstCloudFilesdata,fiile,fileNames);
                                }
                            }
                        }
                        if (lstCloudFilesdata != null && fileNames!=null)
                        {
                            ProccessData(lstCloudFilesdata,fileNames);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            finally
            {
                Main();
            }

        }

        private static void ProccessData(List<string[]> lstCloudFilesdata,List<string> fileNames)
        {
            int i = 0;
            foreach (string[] str in lstCloudFilesdata)
            {
                ReadingAndProcessingData(str, fileNames[i]);
                i++;
            }
            
        }
        /// <summary>
        /// Reads data and populates a list of string arrays
        /// </summary>
        /// <param name="lstFileData">my Initialized list</param>
        /// <param name="fiile">file data</param>
        private static List<string> ReadData(List<string[]> lstFileData,CloudFile fiile,List<string> filNames)
        {
            lstFileData.Add(new string[1000000]);
            filNames.Add(fiile.Name);
            using (var stream = fiile.OpenRead())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string s = String.Empty;
                    int x = 0;
                    while ((s = reader.ReadLine()) != null)
                    {
                       lstFileData.Last()[x] = reader.ReadLine();
                        x++;
                    }
                }
            }
            return filNames;
        }
        private static void ReadingAndProcessingData(string[] data,string objectToUse)
        {
            switch (objectToUse)
            {
                case "CitizenDebts_1M_3.txt":
                    //TODO: insertCitizenDebts();
                    break;
                case "PAYMENTS_20171003.txt":
                    //TODO: insertPayments()
                    break;
                case "SETTLEMENTS_20171003.txt":
                    //TODO: insertSettlements()
                    break;

            }
            //foreach (string str in data)
            //{
            //    int count = 0;
            //    if (!string.IsNullOrEmpty(str))
            //    {
            //        count = str.Length - str.Replace(";", "").Length;
            //    }
            //    for (int i = 0; i < count; i++)
            //    {

            //    }
            //}
        }
    } 
}