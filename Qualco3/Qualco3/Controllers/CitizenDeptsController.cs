using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Db.Models;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MailKit.Net.Smtp;




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
using Microsoft.AspNetCore.Authorization;
/// 

namespace Qualco3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CitizenDeptsController : Controller
    {

        private readonly Db.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CitizenDeptsController(UserManager<ApplicationUser> userManager, Db.Data.ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
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

                //  while (true)
                //{
                //  if (DateTime.Now.AddHours(3).ToShortTimeString() == "09:58:00 aM")
                // {


                //List<string> fileNames = new List<string>();
                //List<string[]> lstCloudFilesdata = new List<string[]>();


                CloudStorageAccount storageAccount = CloudStorageAccount.Parse($"{configuration["ConnectionString1"]}");
                CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
                CloudFileShare fileShare = fileClient.GetShareReference("import");
                //looks for a file share in the cloud
                bool fileShareExists = await fileShare.ExistsAsync();
                if (fileShareExists)
                {

                    List<CloudFile> lstCloudFiles = new List<CloudFile>();
                    CloudFileDirectory rootDir = fileShare.GetRootDirectoryReference();

                    List<string> sl = new List<string>();

                    CloudFile file = rootDir.GetFileReference("CitizenDebts_1M_3.txt");
                    //if the file exists
                    bool asd = await file.ExistsAsync();
                    if (asd)
                    {
                        //adds new datasting array
                        sl = await ReadDataAsync(file);
                    }

                    //foreach (string y in sl)
                    //{ Console.WriteLine("From list new : " + y); };

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
                                                      //Amount = Decimal.Parse(eachLine[9]),
                                                      Amount = Decimal.Parse(eachLine[9], System.Globalization.CultureInfo.GetCultureInfo("el-GR")),
                                                     DueDate = DateTime.ParseExact(eachLine[10],
                                                    "yyyyMMdd", CultureInfo.InvariantCulture)

                                                  };
                    //foreach (var p in o)
                    //{
                    //    Console.WriteLine(p.FirstName + " - " + p.UserId + ", - " + p.DueDate);
                    //};
                  //  var dd = decimal.Parse("10,33", CultureInfo.GetCultureInfo("el-GR"));
                    //clear citizen depts table
                    var all = from c in _context.CitizenDepts select c;
                    _context.CitizenDepts.RemoveRange(all);
                    await _context.SaveChangesAsync();

                    // add new citizendepts
                    foreach (var p in o)
                    {
                        _context.Add(p);
                    }
                    await _context.SaveChangesAsync();

                    //filter citizens
                    IEnumerable<CitizenDepts> NewCitizens = o.Where(x => !((_context.ApplicationUser.Any(y => y.VAT == x.VAT)) || (_context.ApplicationUser.Any(y => y.Email == x.Email))))
                        .Select(x => x).AsEnumerable();


                    foreach (var p in NewCitizens)
                    {
                        Console.WriteLine(p.VAT + " , " + p.Email + " , " + p.LastName + " , " + p.Bill_description);

                    }

                    //only new citizens
                    foreach (var p in NewCitizens.Distinct())
                    {
                        var user = new ApplicationUser { UserName = p.Email, Email = p.Email };

                        var TempPass = GeneratePassword(3, 3, 3, 3);
                        // int lowercase, int uppercase, int numerics, int symbols
                        Console.WriteLine(TempPass);
                        var result = await _userManager.CreateAsync(user, TempPass);
                        if (result.Succeeded)
                        {
                            SendMail(p.LastName, p.Email, TempPass);
                            var query =
                              from UserUpd in _context.ApplicationUser
                              where UserUpd.Email == p.Email
                              select UserUpd;

                            foreach (ApplicationUser UserUpd in query)
                            {
                                UserUpd.VAT = p.VAT;
                                UserUpd.IsFirst = true;
                                UserUpd.LastName = p.LastName;
                                UserUpd.FirstName = p.FirstName;
                                UserUpd.Phone = p.Phone;
                                UserUpd.County = p.County;
                                UserUpd.Address = p.Address;

                            }
                        }
                        else
                        { Console.WriteLine("#############################ALREADY REGISTERED " + p.VAT + " , " + p.Email + " , " + p.LastName); }

                    };
                    await _context.SaveChangesAsync();


                    // IEnumerable<Bills> NewBills = o.Where(x => o.Any())
                    //.Select(x => new
                    //{
                    //    NewBills.GuId = x.UserId,
                    //    NewBills.Amount = x.Amount,
                    //    NewBills.DueDate = x.DueDate,
                    //    NewBills.Bill_description = x.Bill_description,
                    //    NewBills.Status = 0,
                    //    NewBills.User_id = x.VAT,
                    //    NewBills.PaymentMethodId = 1,
                    //    NewBills.SettlementId = 0
                    //});

                   
                    foreach (var p in _context.ApplicationUser)
                    {
                        var query2 =
                              from UIdUpd in _context.CitizenDepts
                              where UIdUpd.VAT == p.VAT
                              select UIdUpd;

                        foreach (CitizenDepts UIdUpd in query2)
                        {
                            UIdUpd.UserGUId = p.Id;
                        }
                    };

                    await _context.SaveChangesAsync();

                    //foreach (var p in o)
                    //{
                    //    var query3 =
                    //            from NewBills in _context.Bills
                    //            select NewBills;

                    //    foreach (Bills NewBills in query3)
                    //    {
                    //        NewBills.GuId = p.BillId;
                    //        NewBills.Amount = p.Amount;
                    //        NewBills.DueDate = p.DueDate;
                    //        NewBills.Bill_description = p.Bill_description;
                    //        NewBills.Status = 0;
                    //        NewBills.UserId = p.UserGUId;
                    //        NewBills.PaymentMethodId = 1;
                    //        NewBills.SettlementId = 1;

                    //    }
                    //};


                    //IEnumerable<Bills> NewBillsls = new List<Bills>();
                    List<Bills> NewBillsls = new List<Bills>();
                    Bills NewBill = new Bills();
                    //etc for your other entities

                    List<CitizenDepts> UpdCit = new List<CitizenDepts>();

                    foreach (var p in _context.CitizenDepts)
                    {
                        UpdCit.Add(p);
                    }
                    foreach (var p in UpdCit)
                    {
                       
                            Console.WriteLine("#############################CITIZEN_DEPTS " + p.VAT + " , " + p.Email + " , " + p.UserGUId);
                            NewBill.GuId = p.BillId;
                            NewBill.Amount = p.Amount;
                            NewBill.DueDate = p.DueDate;
                            NewBill.Bill_description = p.Bill_description;
                            NewBill.Status = 0;
                            NewBill.UserId = p.UserGUId;
                            NewBill.PaymentMethodId = 1;
                            NewBill.SettlementId = 1;    
                            NewBillsls.Add(NewBill);
                            NewBill = new Bills();
                        //_context.Bills.Add(NewBill);
                    }

                    //....  delete bills
                    var allBills = from c in _context.Bills select c;
                    _context.Bills.RemoveRange(allBills);
                    await _context.SaveChangesAsync();

                    //....  delete settlements
                    _context.Settlements.RemoveRange(
                                        _context.Settlements
                                         .Where(s => s.ID !=1 )
                                                             );
                    await _context.SaveChangesAsync();



                    foreach (var c in NewBillsls)
                    {
                        Console.WriteLine("############################BILLS " + c.GuId + " , " + c.UserId);
                        _context.Bills.Add(c);
                    }
                    await _context.SaveChangesAsync();


                    return View(o);
                }
                    // }
                
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


        public static string GeneratePassword(int lowercase, int uppercase, int numerics, int symbols)
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";
            string symbol = "~!@#$%^&*()_+=-";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            for (int i = 1; i <= symbols; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    symbol[random.Next(number.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);
        }

        public static void SendMail(string lastName, string userMail,string TempPass)
        {

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Municipality Portal", "qualco3team@gmail.com"));
            message.To.Add(new MailboxAddress(lastName, userMail));
            message.Subject = "Your Temporary Password";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = @"<b>Dear " + lastName + ",</b></br> This is your temporary random generated password:  <b><i>" + TempPass + "</i></b>  .</br>Please login with that and change it immediatelly ";
            message.Body = bodyBuilder.ToMessageBody();

            //message.Body = new TextPart("plain")
            //{
            //    Text = @"<b>Dear " + lastName + ",<b></br> This is your temporary random generated password --<< <i>" + TempPass + "<i>  >>-- .</br>Please login with that and change it immediatelly "
            //};

            using (var client = new SmtpClient())
            {

                // Establishes connection to the specified SMTP
                client.Connect("smtp.gmail.com", 587, false);
               // client.AuthenticationMechanisms.Remove("XOAUTH2");
                //client.AuthenticationMechanisms.Remove("XOAUTH");
                // Authenticate connection. Only needed if the SMTP server requires authentication.
                client.Authenticate("qualco3team@gmail.com","Qualco123!");

                client.Send(message);
                client.Disconnect(true);
                
            }

            return;
        }

      

        private static async Task<List<string>> ReadDataAsync(CloudFile file)
        {
            try
            {
                List<string> sl = new List<string>();
                using (var stream = await file.OpenReadAsync())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string s = String.Empty;
                        int x = 0;
                        while ((s = reader.ReadLine()) != null)
                        {
                           if (s.Contains("VAT") == false)
                            {
                                sl.Add(s);
                            //Console.WriteLine(s);
                            }
                        }
                        return sl;
                    }
                }
               
            }
            catch (Exception ex) { return null; }
        }

        
    }
}