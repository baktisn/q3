using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace ImportExportWebJobCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => mainAsync(args)).GetAwaiter().GetResult();
        }

        private static async Task mainAsync(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();
                var allCitDebtsLines = new string[10000000];
                var allSettlementsLines = new string[10000000];
                var allPaymentLines = new string[10000000];
                while (true)
                {
                    if (DateTime.Now.AddHours(3).ToShortTimeString() == "12:00 PM")
                    {
                        List<string> fileNames = new List<string>();
                        List<string[]> lstCloudFilesdata = new List<string[]>();
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse($"{configuration["ConnectionString1"]}");
                        CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
                        CloudFileShare fileShare = fileClient.GetShareReference("import");
                        //looks for a file share in the cloud
                        bool fileShareExists = await fileShare.ExistsAsync();
                        if (fileShareExists)
                        {
                           
                            List<CloudFile> lstCloudFiles = new List<CloudFile>();
                            CloudFileDirectory rootDir = fileShare.GetRootDirectoryReference();
                            //for each file in my fileshare
                            FileContinuationToken token= null;
                            FileResultSegment k = await rootDir.ListFilesAndDirectoriesSegmentedAsync(token);
                            token = k.ContinuationToken;

                            foreach (IListFileItem fiile in k.Results)
                            {
                                //if the file exists
                                CloudFile file = (CloudFile)fiile;
                                bool asd = await file.ExistsAsync();
                                if (asd)
                                {
                                    //adds new datasting array
                                  await ReadDataAsync(lstCloudFilesdata, file, fileNames);
                                }
                            }
                        }
                        if (lstCloudFilesdata != null && fileNames != null)
                        {
                            ProccessData(lstCloudFilesdata, fileNames);
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
               
            }
        }

        private static void ProccessData(List<string[]> lstCloudFilesdata, List<string> fileNames)
        {
            int i = 0;
            foreach (string[] str in lstCloudFilesdata)
            {
                ReadingAndProcessingData(str, fileNames[i]);
                i++;
            }
        }

        private static async Task<List<string>> ReadDataAsync(List<string[]> lstCloudFilesData, CloudFile fiile, List<string> fileNames)
        {
            try
            {
                lstCloudFilesData.Add(new string[1000000]);
                fileNames.Add(fiile.Name);
                using (var stream = await fiile.OpenReadAsync())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string s = String.Empty;
                        int x = 0;
                        while ((s = reader.ReadLine()) != null)
                        {
                            lstCloudFilesData.Last()[x] = reader.ReadLine();
                            x++;
                        }
                    }
                }
                return fileNames;
            }
            catch (Exception ex) { return null; }
        }
        private static void ReadingAndProcessingData(string[] data, string objectToUse)
        {
            switch (objectToUse)
            {
                case "CitizenDebts_1M_3.txt":
                    //TODO:insertCitizenDebts();
                    break;
                case "PAYMENTS_20171003.txt":
                    //TODO: insertPayments()
                    break;
                case "SETTLEMENTS_20171003.txt":
                    //TODO: insertSettlements()
                    break;
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
}
