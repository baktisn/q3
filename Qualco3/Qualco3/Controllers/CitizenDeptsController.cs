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
using System.Net;
using System.Transactions;
using System.Text.RegularExpressions;
/// 

namespace Qualco3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CitizenDeptsController : Controller
    {

        private readonly Db.Data.ApplicationDbContext _context;
        private readonly Db.Data.ApplicationDbContext _context2;
        private readonly UserManager<ApplicationUser> _userManager;
        private int NewUsers;
        private IEnumerable<CitizenDepts> NewCitizens;

        public CitizenDeptsController(UserManager<ApplicationUser> userManager, Db.Data.ApplicationDbContext context)
        {
            _context = context;
            _context2 = context;
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

                    DeptResults DeptResults = new DeptResults();
                    List<ErrorLines> ErrorLines= new List<ErrorLines>();
                    ErrorLines ErrorLine = new ErrorLines();



                    string bigfilename = "CitizenDebts_1M_3Big.txt";
                    string fileName = "CitizenDebts_1M_3.txt";
                        //"DEBTS_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    CloudFile file = rootDir.GetFileReference(fileName);
                    string checkfile = bigfilename;
                    //if the file exists
                    
                    bool asd = await file.ExistsAsync();
                    if (asd)
                    {
                        //adds new datasting array
                        sl = await ReadDataAsync(file);
                        if (sl is null)
                        { 
                            //return NotFound(HttpStatusCode.NotFound + "\n" + "\nΣφάλμα\nΤο αρχείο δεν περιέχει σωστό αριθμό στηλών</b>");
                            Redirect(DeptResults.BillsCount=0,DeptResults.NewUsers=0, HttpStatusCode.NotFound.ToString(),null, "\n" + "\nΣφάλμα\nΤο αρχείο δεν περιέχει σωστό αριθμό στηλών</b>");
                        }
                    }
                    else
                    {
                        Redirect(DeptResults.BillsCount = 0, DeptResults.NewUsers = 0, HttpStatusCode.NotFound.ToString(), null, "\n" + "\nΣφάλμα\nΔεν βρέθηκε το αρχείο</b>");

                       // return NotFound(HttpStatusCode.NotFound+ "\n" + "\nΣφάλμα\nΔεν βρέθηκε το αρχείο</b>");
                    }

                    Console.WriteLine("File into List "+DateTime.Now.ToString());
                    //foreach (string y in sl)
                    //{ Console.WriteLine("From list new : " + y); };
                    string[] cols;
                    
                        for (int i = sl.Count - 1; i >= 0; i--)
                        {

                            cols = sl.ElementAt(i).Split(';');
                        if (cols[0].Trim().Length != 10 || !cols[0].All(char.IsDigit))
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος ΑΦΜ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                            continue;
                            //return NotFound(HttpStatusCode.NotFound + "\n" + "\nΣφάλμα Λάθος ΑΦΜ");}
                        }
                            if (cols[1].Trim().Length ==0 )
                        {
                                ErrorLine.line = i;
                                ErrorLine.ErrorMessage = "Σφάλμα Λάθος Όνομα ";
                                ErrorLine.LineString = sl[i];
                                ErrorLines.Add(ErrorLine);
                                sl.RemoveAt(i); //return NotFound(HttpStatusCode.NotFound + "\n" + "\nΣφάλμα Λάθος Όνομα ");
                        }

                        if (cols[2].Trim().Length == 0)
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Επώνυμο ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }

                            if (cols[3].Trim().Length == 0 || !Regex.IsMatch(cols[3], @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Email   ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                            }

                        if (cols[4].Trim().Length == 0 || !cols[4].All(char.IsDigit))
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Τηλέφωνο  ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                            }

                        if (cols[5].Trim().Length == 0)
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Διεύθυσνη ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }

                        if (cols[6].Trim().Length == 0)
                        { sl.RemoveAt(i); return NotFound(HttpStatusCode.NotFound + "\n" + "\nΣφάλμα Λάθος Περιοχή "); }
                        //!Regex.IsMatch(cols[7], @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$")
                        if (cols[7].Trim().Length == 0 )
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Αρ.Λογαριασμού ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }
                  
                        if (cols[8].Trim().Length == 0)
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Περιγραφή Λογαριασμού";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                         }

                        decimal number;
                        if (cols[9].Trim().Length == 0 || !Decimal.TryParse(cols[9], out number) || cols[9].Contains('.'))

                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Ποσό";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                         }

                        DateTime d;
                        if (cols[10].Trim().Length == 0 || !DateTime.TryParseExact(cols[10], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None,out d))
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Ημερομηνία";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }
                    }



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
                    Console.WriteLine("File splitted " + DateTime.Now.ToString());


          
                        var all = from c in _context.CitizenDepts select c;
                    _context.CitizenDepts.RemoveRange(all);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("CitizenDept table deleted " + DateTime.Now.ToString());

                            foreach (var p in o)
                            {
                                _context.Add(p);
                            }
                            Console.WriteLine("Context filledup " + DateTime.Now.ToString());
                            await _context.SaveChangesAsync();
        


                     Console.WriteLine("Context saved to DB " + DateTime.Now.ToString());

                    //filter citizens
                    NewCitizens = o.
                        Where(x => !((_context.ApplicationUser.Any(y => y.VAT == x.VAT)) || (_context.ApplicationUser.Any(y => y.Email == x.Email))))
                        .Select(x => x).AsEnumerable();
                    Console.WriteLine("Citizens filterd " + DateTime.Now.ToString());


                    foreach (var p in NewCitizens)
                    {
                        Console.WriteLine(p.VAT + " , " + p.Email + " , " + p.LastName + " , " + p.Bill_description);

                    }
                    Console.WriteLine("Citizens log " + DateTime.Now.ToString());
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
                            if  (checkfile!= "CitizenDebts_1M_3.txt")
                            {
                                Console.WriteLine("Sending Emails");
                                SendMail(p.LastName, p.Email, TempPass);
                            }
                            

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
                        NewUsers++;
                    };
                   
                     await _context.SaveChangesAsync();
                    Console.WriteLine("New Users Registered and Emailed " + DateTime.Now.ToString());
                    

                    foreach (var a in _context.ApplicationUser)
                    {
                        var query2 =
                              from UIdUpd in _context.CitizenDepts
                              where UIdUpd.VAT == a.VAT
                              select UIdUpd;

                        foreach (CitizenDepts UIdUpd in query2)
                        {
                            UIdUpd.UserGUId = a.Id;
                        }
                    };

                    await _context.SaveChangesAsync();




                    List<Bills> NewBillsls = new List<Bills>();
                    Bills NewBill = new Bills();

                    List<CitizenDepts> UpdCit = new List<CitizenDepts>();


                        UpdCit.AddRange(_context.CitizenDepts);

                    foreach (var e in UpdCit)
                    {
                       
                            Console.WriteLine("#############################CITIZEN_DEPTS " + e.VAT + " , " + e.Email + " , " + e.UserGUId);
                            NewBill.GuId = e.BillId;
                            NewBill.Amount = e.Amount;
                            NewBill.DueDate = e.DueDate;
                            NewBill.Bill_description = e.Bill_description;
                            NewBill.Status = 0;
                            NewBill.UserId = e.UserGUId;
                            NewBill.PaymentMethodId = 1;
                            NewBill.SettlementId = 1;    
                            NewBillsls.Add(NewBill);
                            NewBill = new Bills();
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

                        _context.Bills.AddRange(NewBillsls);
  
                    await _context.SaveChangesAsync();

                    string Mes;
                    if (ErrorLines.Count()>0)
                    { Mes = "Ύπήρχαν "+ ErrorLines.Count() + " σφάλματα στην εισαγωγή του αρχείου"; }
                    else
                    { Mes = "Δεν Υπήρχαν Σφάλματα στην εισαγωγή του αρχείου"; }
                 

                    //  return View(NewCitizens);
                        return RedirectToAction("DeptResults", "DeptResults", new DeptResults
                    {
                        BillsCount = NewBillsls.Count(),
                        NewUsers = NewUsers,
                        HttpStatus = "",
                        ErrorLines = ErrorLines,
                        Message = Mes
                    });
                      
                }
                else
                {
                    return NotFound(HttpStatusCode.NotFound + "\n" + "\nΔεν βρέθηκε ο κοινός φάκελος!");
                }
    
            }
            catch (Exception ex)
            {
                return NotFound(HttpStatusCode.ExpectationFailed + "\n" + ex.Message+ "\nΣφάλμα\nΑπροσδιόριστο σφάλμα Στην Εισαγωγή του αρχείου</b>");
            }
            finally
            {
                Console.WriteLine("Import Finished Successfully " + DateTime.Now.ToString());
            }

            return View();

        }




        public async Task<IActionResult> GetFile2()
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

                    DeptResults DeptResults = new DeptResults();
                    List<ErrorLines> ErrorLines = new List<ErrorLines>();
                    ErrorLines ErrorLine = new ErrorLines();



                    string bigfilename = "CitizenDebts_1M_3Big.txt";
                   // string fileName = "CitizenDebts_1M_3.txt";
                    //"DEBTS_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    CloudFile file = rootDir.GetFileReference(bigfilename);
                    string checkfile = bigfilename;
                    //if the file exists

                    bool asd = await file.ExistsAsync();
                    if (asd)
                    {
                        //adds new datasting array
                        sl = await ReadDataAsync(file);
                        if (sl is null)
                        {
                            //return NotFound(HttpStatusCode.NotFound + "\n" + "\nΣφάλμα\nΤο αρχείο δεν περιέχει σωστό αριθμό στηλών</b>");
                            Redirect(DeptResults.BillsCount = 0, DeptResults.NewUsers = 0, HttpStatusCode.NotFound.ToString(), null, "\n" + "\nΣφάλμα\nΤο αρχείο δεν περιέχει σωστό αριθμό στηλών</b>");
                        }
                    }
                    else
                    {
                        Redirect(DeptResults.BillsCount = 0, DeptResults.NewUsers = 0, HttpStatusCode.NotFound.ToString(), null, "\n" + "\nΣφάλμα\nΔεν βρέθηκε το αρχείο</b>");

                        // return NotFound(HttpStatusCode.NotFound+ "\n" + "\nΣφάλμα\nΔεν βρέθηκε το αρχείο</b>");
                    }

                    Console.WriteLine("File into List " + DateTime.Now.ToString());
                    //foreach (string y in sl)
                    //{ Console.WriteLine("From list new : " + y); };
                    string[] cols;

                    for (int i = sl.Count - 1; i >= 0; i--)
                    {

                        cols = sl.ElementAt(i).Split(';');
                        if (cols[0].Trim().Length != 10 || !cols[0].All(char.IsDigit))
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος ΑΦΜ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                            continue;
                            //return NotFound(HttpStatusCode.NotFound + "\n" + "\nΣφάλμα Λάθος ΑΦΜ");}
                        }
                        if (cols[1].Trim().Length == 0)
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Όνομα ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i); //return NotFound(HttpStatusCode.NotFound + "\n" + "\nΣφάλμα Λάθος Όνομα ");
                        }

                        if (cols[2].Trim().Length == 0)
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Επώνυμο ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }

                        if (cols[3].Trim().Length == 0 || !Regex.IsMatch(cols[3], @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Email   ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }

                        if (cols[4].Trim().Length == 0 || !cols[4].All(char.IsDigit))
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Τηλέφωνο  ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }

                        if (cols[5].Trim().Length == 0)
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Διεύθυσνη ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }

                        if (cols[6].Trim().Length == 0)
                        { sl.RemoveAt(i); return NotFound(HttpStatusCode.NotFound + "\n" + "\nΣφάλμα Λάθος Περιοχή "); }
                        //!Regex.IsMatch(cols[7], @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$")
                        if (cols[7].Trim().Length == 0)
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Αρ.Λογαριασμού ";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }

                        if (cols[8].Trim().Length == 0)
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Περιγραφή Λογαριασμού";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }

                        decimal number;
                        if (cols[9].Trim().Length == 0 || !Decimal.TryParse(cols[9], out number) || cols[9].Contains('.'))

                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Ποσό";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }

                        DateTime d;
                        if (cols[10].Trim().Length == 0 || !DateTime.TryParseExact(cols[10], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                        {
                            ErrorLine.line = i;
                            ErrorLine.ErrorMessage = "Σφάλμα Λάθος Ημερομηνία";
                            ErrorLine.LineString = sl[i];
                            ErrorLines.Add(ErrorLine);
                            sl.RemoveAt(i);
                        }
                    }



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
                    Console.WriteLine("File splitted " + DateTime.Now.ToString());



                    var all = from c in _context.CitizenDepts select c;
                    _context.CitizenDepts.RemoveRange(all);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("CitizenDept table deleted " + DateTime.Now.ToString());

                    foreach (var p in o)
                    {
                        _context.Add(p);
                    }
                    Console.WriteLine("Context filledup " + DateTime.Now.ToString());
                    await _context.SaveChangesAsync();



                    Console.WriteLine("Context saved to DB " + DateTime.Now.ToString());

                    //filter citizens
                    NewCitizens = o.
                        Where(x => !((_context.ApplicationUser.Any(y => y.VAT == x.VAT)) || (_context.ApplicationUser.Any(y => y.Email == x.Email))))
                        .Select(x => x).AsEnumerable();
                    Console.WriteLine("Citizens filterd " + DateTime.Now.ToString());


                    foreach (var p in NewCitizens)
                    {
                        Console.WriteLine(p.VAT + " , " + p.Email + " , " + p.LastName + " , " + p.Bill_description);

                    }
                    Console.WriteLine("Citizens log " + DateTime.Now.ToString());
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
                            if (checkfile != "CitizenDebts_1M_3.txt")
                            {
                                Console.WriteLine("Sending Emails");
                                SendMail(p.LastName, p.Email, TempPass);
                            }


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
                        NewUsers++;
                    };

                    await _context.SaveChangesAsync();
                    Console.WriteLine("New Users Registered and Emailed " + DateTime.Now.ToString());


                    foreach (var a in _context.ApplicationUser)
                    {
                        var query2 =
                              from UIdUpd in _context.CitizenDepts
                              where UIdUpd.VAT == a.VAT
                              select UIdUpd;

                        foreach (CitizenDepts UIdUpd in query2)
                        {
                            UIdUpd.UserGUId = a.Id;
                        }
                    };

                    await _context.SaveChangesAsync();




                    List<Bills> NewBillsls = new List<Bills>();
                    Bills NewBill = new Bills();

                    List<CitizenDepts> UpdCit = new List<CitizenDepts>();


                    UpdCit.AddRange(_context.CitizenDepts);

                    foreach (var e in UpdCit)
                    {

                        Console.WriteLine("#############################CITIZEN_DEPTS " + e.VAT + " , " + e.Email + " , " + e.UserGUId);
                        NewBill.GuId = e.BillId;
                        NewBill.Amount = e.Amount;
                        NewBill.DueDate = e.DueDate;
                        NewBill.Bill_description = e.Bill_description;
                        NewBill.Status = 0;
                        NewBill.UserId = e.UserGUId;
                        NewBill.PaymentMethodId = 1;
                        NewBill.SettlementId = 1;
                        NewBillsls.Add(NewBill);
                        NewBill = new Bills();
                    }

                    //....  delete bills
                    var allBills = from c in _context.Bills select c;
                    _context.Bills.RemoveRange(allBills);
                    await _context.SaveChangesAsync();

                    //....  delete settlements
                    _context.Settlements.RemoveRange(
                                        _context.Settlements
                                         .Where(s => s.ID != 1)
                                                             );
                    await _context.SaveChangesAsync();

                    _context.Bills.AddRange(NewBillsls);

                    await _context.SaveChangesAsync();

                    string Mes;
                    if (ErrorLines.Count() > 0)
                    { Mes = "Ύπήρχαν " + ErrorLines.Count() + " σφάλματα στην εισαγωγή του αρχείου"; }
                    else
                    { Mes = "Δεν Υπήρχαν Σφάλματα στην εισαγωγή του αρχείου"; }


                    //  return View(NewCitizens);
                    return RedirectToAction("DeptResults", "DeptResults", new DeptResults
                    {
                        BillsCount = NewBillsls.Count(),
                        NewUsers = NewUsers,
                        HttpStatus = "",
                        ErrorLines = ErrorLines,
                        Message = Mes
                    });

                }
                else
                {
                    return NotFound(HttpStatusCode.NotFound + "\n" + "\nΔεν βρέθηκε ο κοινός φάκελος!");
                }

            }
            catch (Exception ex)
            {
                return NotFound(HttpStatusCode.ExpectationFailed + "\n" + ex.Message + "\nΣφάλμα\nΑπροσδιόριστο σφάλμα Στην Εισαγωγή του αρχείου</b>");
            }
            finally
            {
                Console.WriteLine("Import Finished Successfully " + DateTime.Now.ToString());
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

                            if (s.Count(f => f == ';') != 10)
                            {
                              return null;
                            }

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

        public  ActionResult Redirect(int Bills,int Users,string status,List<ErrorLines> lines,string Message)
        {
            TempData["list"] = lines;
            return RedirectToAction("DeptResults", "DeptResults", new DeptResults
            {
                BillsCount = Bills,
                NewUsers = Users,
                HttpStatus = status,
                ErrorLines = lines,
                Message = Message
            }
                              );
        }



        //public static Db.Data.ApplicationDbContext BulkInsert<CitizenDepts>(this Db.Data.ApplicationDbContext _context, CitizenDepts entity, int count, int batchSize) 
        //{
        //    _context.Set<CitizenDepts>.Add(entity);

        //    if (count % batchSize == 0)
        //    {
        //        _context.SaveChanges();
        //        _context.Dispose();
        //        _context = context;

        //        // This is optional
        //        _context.ChangeTracker.AutoDetectChangesEnabled = false;
        //    }
        //    return _context;
        //}





    }
}