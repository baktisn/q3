using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Db.Models;


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.Azure;
using System.IO;
using System.Text;
using Db.Models.AccountViewModels;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace Qualco3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PostFileController : Controller
    {
        private readonly Db.Data.ApplicationDbContext _context;
       
        public PostFileController( Db.Data.ApplicationDbContext context)
        {
            _context = context;
            
        }

        public async Task<IActionResult> PostFile()
        {
            PostFile model = new PostFile();
            try
            {

                var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();

                var allSettlementsLines = new string[10000000];
                var allPaymentLines = new string[10000000];

                List<string> fileNames = new List<string>();
                List<string[]> lstCloudFilesdata = new List<string[]>();
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse($"{configuration["ConnectionString1"]}");
                CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
                CloudFileShare fileShare = fileClient.GetShareReference("export");
                //looks for a file share in the cloud
                bool fileShareExists = await fileShare.ExistsAsync();



                if (fileShareExists)
                {
                    
                    CloudFileDirectory rootDir = fileShare.GetRootDirectoryReference();

                    var payments = _context.Bills
                      .Where(s => s.Status == 1).ToList();
                    List<string> PaymentsList = new List<string>();
                    model.PaymentsCount= 0;

                    foreach (var x in payments)
                    {
                        PaymentsList.Add(x.GuId + ";" + x.DueDate.ToUniversalTime().ToString("o") + ";" + x.Amount +";"+"CREDIT");
                        model.PaymentsCount++ ;
                    }
                    Console.WriteLine(model.PaymentsCount);
                    string fileName = "PAYMENTS_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                   await Export(PaymentsList, rootDir, fileName);

                    var List = _context.ApplicationUser    
                       .Join(_context.Bills, 
                          a => a.Id,      
                          b => b.UserId,
                          (a, b) => new { a, b })
                           .Where(w => w.b.Status==2)
                       .Join(_context.Settlements, 
                          bb => bb.b.SettlementId,     
                          c => c.ID,   
                          (bb, c) => new { bb, c })
                       .Where(w => w.bb.b.Status == 2 )  
                      .Select(m => new
                      {
                          VAT = m.bb.a.VAT, 
                          SettlReq = m.c.RequestDate,
                          Bills = m.bb.a.Bills,
                          Downpayment=m.c.DownPayment,
                          Installments=m.c.Installments,
                          Interest=m.c.Interest,
                          SettlementId=m.c.ID
                      }).ToList();

                    List<string> SettlementsList = new List<string>();
                    model.SettlementsCount = 0;

                    foreach (var x in List.Distinct())
                    {
                        //Console.WriteLine(string.Join(",", x.Bills.Where(w=>w.Status==2).Select(n=>n.GuId)));
                        SettlementsList.Add(x.VAT + ";" + x.SettlReq.ToUniversalTime().ToString("o") + ";" + string.Join(",", x.Bills.Where(w => w.Status == 2 && w.SettlementId==x.SettlementId).Select(n => n.GuId).Distinct()) + ";" + x.Downpayment + ";" + x.Installments + ";" + x.Interest);
                        model.SettlementsCount++;
                    }
                    Console.WriteLine(model.SettlementsCount);
                    fileName = "SETTLEMENTS" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    await Export(SettlementsList, rootDir, fileName);

                }


                }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

            }

            return View(model);

        }


        public async Task Export(List<string> List, CloudFileDirectory rootDir,string filename)
        {

            string text = string.Join("\n", List.ToArray());
            byte[] byteArray = Encoding.UTF8.GetBytes(text);
            MemoryStream stream = new MemoryStream(byteArray);
            CloudFile file = rootDir.GetFileReference(filename);
            await file.UploadFromStreamAsync(stream);

        }


    }
}