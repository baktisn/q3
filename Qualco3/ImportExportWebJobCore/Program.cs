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
using Db.Models;
using Db.Data;
using System.Globalization;

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

                            //var context_ = new Db.Data.();
                            List<string> sl = new List<string>();

                            foreach (IListFileItem fiile in k.Results)
                            {
                                //if the file exists
                                CloudFile file = (CloudFile)fiile;
                                bool asd = await file.ExistsAsync();
                                if (asd)
                                {
                                    //adds new datasting array
                                    sl = await ReadDataAsync(lstCloudFilesdata, file, fileNames);
                                }

                                foreach (string y in sl)
                                { Console.WriteLine("From list new : " + y); };

                                IEnumerable<CitizenDepts> o = from eachLine in (
                                 from inner in sl
                                 select inner.Split(';')
                                 )
                                                              select new CitizenDepts
                                                              {
                                                                  VAT = eachLine[0],
                                                                  FirstName = eachLine[1],
                                                                  LastName = eachLine[2],
                                                                  Email = eachLine[3],
                                                                  Phone = eachLine[4],
                                                                  Address = eachLine[5],
                                                                  County = eachLine[6],
                                                                  BillId = eachLine[7],
                                                                  Bill_description = eachLine[8],
                                                                  Amount = Decimal.Parse(eachLine[9]),
                                                                  DueDate = DateTime.ParseExact(eachLine[10],
                                                               "yyyyMMdd", CultureInfo.InvariantCulture)

                                                              };
                                foreach (var p in o)
                                {
                                    Console.WriteLine(p.FirstName + " - " + p.BillId + ", - " + p.DueDate);
                                };



                                //string s = context_.Database.ProviderName;
                         
                               // Console.WriteLine(s);
                               /// var all = from c in context_.CitizenDepts select c;
                                //context_.CitizenDepts.RemoveRange(all);
                               /// context_.SaveChanges();

                                foreach (var p in o)
                                {
                                    //Add Student object into Students DBset
                                    //if (p.VAT!=null)
                                    //context_.Add(p);
                                }
                                //// call SaveChanges method to save student into database
                                //context_.SaveChanges();


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
                Console.WriteLine(ex.Message);


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
                List<string> sl = new List<string>();
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

                            if (s.Contains("VAT") == false)
                            { sl.Add(s); }
                        }
                        return sl;
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

                 //   var context_ = new ApplicationDbContext();

                  
                   // CitizenDepts o = 
                   //     new CitizenDepts { VAT = "getgt", FirstName = "fghfghd",  LastName = "sdfsddsfv",Email = "gwtgtew@wef.gr",Phone = "45324532",Address = "fghdgfh",County = "jhgjgjgf",UserId = "gfjdfhgcmvm",Bill_description = "cgfbxfhgf", Amount = 14.45M,DueDate = new DateTime(2017, 1, 18)
                   // };

                   //context_.CitizenDepts.Add(o);
                   // context_.SaveChanges();



                    //TODO:insertCitizenDebts();
                    break;
                case "PAYMENTS_20171003.txt":
                    //TODO: insertPayments()
                    break;
                case "SETTLEMENTS_20171003.txt":
                    //TODO: insertSettlements()
                    break;

            }
        }
    }
}
