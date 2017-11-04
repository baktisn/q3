using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Db.Models;

///
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.Azure;
using System.IO;
using System.Text;
//using Microsoft.Azure.WebJobs;
using Db.Models.AccountViewModels;
using System.Globalization;
/// 

namespace Qualco3.Controllers
{
    public class CitizenDeptsController : Controller
    {

        private readonly Db.Data.ApplicationDbContext _context;

        public CitizenDeptsController(Db.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetFile()
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
                //  while (true)
                //{
                //  if (DateTime.Now.AddHours(3).ToShortTimeString() == "09:58:00 aM")
                // {
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
                    FileContinuationToken token = null;
                    FileResultSegment k = await rootDir.ListFilesAndDirectoriesSegmentedAsync(token);
                    token = k.ContinuationToken;

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
                                    UserId = eachLine[7],
                                    Bill_description = eachLine[8],
                                    Amount = Decimal.Parse(eachLine[9]),
                                    DueDate =DateTime.ParseExact(eachLine[10],
                                  "yyyyMMdd",CultureInfo.InvariantCulture)

                    };
                        foreach (var p in o)
                        {
                            Console.WriteLine(p.FirstName + " - " + p.UserId + ", - " + p.DueDate);
                        };

                        //Console.WriteLine(o.ElementAt(0));

                       
                            foreach (var p in o)
                            {
                               //Add Student object into Students DBset
                               _context.Add(p);
                           }
                        //// call SaveChanges method to save student into database
                        _context.SaveChanges();
                        
                        // List<CitizenDepts> o = new List<CitizenDepts>()
                        //{
                        //    new CitizenDepts { VAT = "getgt", FirstName = "fghfghd",  LastName = "sdfsddsfv",Email = "gwtgtew@wef.gr",Phone = "45324532",Address = "fghdgfh",County = "jhgjgjgf",ID = 4,Bill_description = "cgfbxfhgf", Amount = 14.45M,DueDate = new DateTime(2017, 1, 18)},
                        //    new CitizenDepts { VAT = "getgt", FirstName = "fghfghd",  LastName = "sdfsddsfv",Email = "gwtgtew@wef.gr",Phone = "45324532",Address = "fghdgfh",County = "jhgjgjgf",ID = 4,Bill_description = "cgfbxfhgf", Amount = 14.45M,DueDate =new DateTime(2017, 1, 18)},
                        //    new CitizenDepts { VAT = "getgt", FirstName = "fghfghd",  LastName = "sdfsddsfv",Email = "gwtgtew@wef.gr",Phone = "45324532",Address = "fghdgfh",County = "jhgjgjgf",ID = 4,Bill_description = "cgfbxfhgf", Amount = 14.45M,DueDate = new DateTime(2017, 1, 18)}

                        //};


                        // foreach (var p in o)
                        // {
                        //     Console.WriteLine(p.FirstName + " - " + p.ID + ", - " + p.DueDate);
                        // };

                        //ViewData["MyList"]= sl;
                        return View(o);
                    }
                    // }
                    if (lstCloudFilesdata != null && fileNames != null)
                    {
                        ProccessData(lstCloudFilesdata, fileNames);
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

            }

            return View();

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
                    Db.Models.ApplicationUser user = new Db.Models.ApplicationUser();

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