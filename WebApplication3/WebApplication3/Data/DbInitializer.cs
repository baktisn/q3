using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;

namespace WebApplication3.Data
{
    public class DbInitializer
    {
        public static void Initialize(MunicContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }


           
        var users = new Users[]
            {
            new Users{afm="443534",username="Alexander",password="Alexander"},
            new Users{afm="345345",username="Alonso",password="Alexander"},
            new Users{afm="Ar53425turo",username="Anand",password="Alexander"},
            new Users{afm="5234",username="Barzdukas",password="Alexander"},
            new Users{afm="53425",username="Li",password="Alexander"},
            new Users{afm="5234535",username="Justice",password="Alexander"},
            new Users{afm="L5324523aura",username="Norman",password="Alexander"},
            new Users{afm="52345324",username="Olivetto",password="Alexander"}
            };
            foreach (Users s in users)
            {
                context.Users.Add(s);
            }
            context.SaveChanges();


            if (context.Bills.Any())
            {
                return;   // DB has been seeded
            }



            var bills = new Bills[]
                {
            new Bills{ID=345345,UserId = "345345",Amount =10,Description="fdafghdav"},
             new Bills{ID=345346,UserId = "345345",Amount =20,Description="hjbibkn"}
                };
            foreach (Bills f in bills)
            {
                context.Bills.Add(f);
            }
            context.SaveChanges();

        }
    }




}

