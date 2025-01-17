using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Projet2Groupe1.Models;
using Projet2Groupe1.ViewModels;

namespace Projet2Groupe1.Controllers
{
    public class RegistrationController : Controller
    {

        [HttpGet]
        public IActionResult MemberRegistration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MemberRegistration(User user, Member member)
        {
                     
            using (IUserService ius = new UserService(new DataBaseContext()))
            {
                Console.WriteLine("ModelState de User " + ModelState.IsValid);
                if (ModelState.IsValid && ius.searchUser(user.Id) == null)
                {
                    UserRole role = member.IsPremium ? UserRole.PREMIUM : UserRole.MEMBER;
                    statusRegistration statusAccepted = statusRegistration.ACCEPTED;
                    user.Id = ius.CreateUser(user.FirstName, user.LastName, user.Phone, user.Mail, user.Password, user.Newsletter, statusAccepted, user.Role = role);
                    Console.WriteLine(user.ToString());
                }

                using (IMemberService ims = new MemberService(new DataBaseContext()))
                {
                    Console.WriteLine("ModelState de member " + ModelState.IsValid);
                    if (ModelState.IsValid)
                    {
                        member.Id = ims.CreateMember(member.Age, member.City, member.ZipCode, member.IsPremium, member.IsPayed, user.Id);

                        Console.WriteLine("Member id est : " + member.Id + "Ispremium est  " + member.IsPremium + " et IsPayed est a " + member.IsPayed);

                        if (member.IsPremium)
                        {
                            if (!member.IsPayed)
                            {
                                TempData["MemberId"] = member.Id; 
                                TempData["userId"] = user.Id;  // j'enregistre le userId dans le tempData pour le recupérer dans le controlleur de paiement
                                Console.WriteLine("Apres l avoir mis dans le view bag member.Id vaut : " + member.Id);
                                TempData["Message"] = "Veuillez effectuer le paiement pour finaliser votre inscription.";
                                return RedirectToAction("Payment", "Payment");
                            }
                        }

                        UserRole dashboardRole = member.IsPremium ? UserRole.PREMIUM : UserRole.MEMBER;
                        return RedirectToAction("Connection", "Login");
                    }
                }
                return View();
            }
        }

        public IActionResult OrganizerRegistration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterOrganizer(User user,Organizer organizer)
        {
            Console.WriteLine("je suis entre dans RegisterOrganizer");
            using (IUserService ius = new UserService(new DataBaseContext()))
            {
                if (!ModelState.IsValid)
                {
                    foreach (var field in ModelState)
                    {
                        string fieldName = field.Key; // Nom du champ
                        var errors = field.Value.Errors; // Liste des erreurs associées

                        foreach (var error in errors)
                        {
                            Console.WriteLine($"Champ : {fieldName}, Erreur : {error.ErrorMessage}");
                        }
                    }
                }
                Console.WriteLine("vérification du model state " + ModelState.IsValid);
                if (ModelState.IsValid && ius.searchUser(user.Id) == null)
                {
                    statusRegistration StatusPending = statusRegistration.PENDING;
                    user.Id = ius.CreateUser(user.FirstName, user.LastName, user.Phone, user.Mail, user.Password,user.Newsletter, user.StatusRegistration = StatusPending, user.Role = UserRole.ORGANIZER);
                }
                Console.WriteLine("Apres le if de createuser");


                using (IOrganizerService ios = new OrganizerService(new DataBaseContext()))
                {
                    if (ModelState.IsValid)
                    {
                        Console.WriteLine("dans le if ");
                        organizer.Id = ios.CreateOrganizer(organizer.Function, organizer.Denomination, organizer.RIB, organizer.Adress,user.Id);
                      
                        ius.UpdateUser(user);
                       
                        return RedirectToAction("Connection", "Login");
                    }
                    Console.WriteLine("Apresle if de createOrganizer");


                }
                return View();
            }


        }


        [HttpGet]
        public IActionResult ProviderRegistration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProviderRegistration(User user, Provider provider)
        {
            using (IUserService ius = new UserService(new DataBaseContext()))
            {
                if (ModelState.IsValid && ius.searchUser(user.Id) == null)
                {
                    statusRegistration StatusPending = statusRegistration.PENDING;
                    user.Id = ius.CreateUser(user.FirstName, user.LastName, user.Phone, user.Mail, user.Password, user.Newsletter, user.StatusRegistration = StatusPending, user.Role = UserRole.PROVIDER);
                }
            }

            using (IProviderService ips = new ProviderService(new DataBaseContext()))
            {
                provider.UserId = user.Id;
                if (ModelState.IsValid)
                {
                    ips.CreateProvider(provider.Function, provider.ServiceType, provider.Adress, provider.UserId);

                    return RedirectToAction("Connection", "Login"); //new { dashboardType = UserRole.PROVIDER }
                }
            }

            return View();
        }

        public IActionResult IndividualRegistration()
        {
            return View();
        }

        public IActionResult ProfessionalRegistration()
        {
            return View();
        }
    }
}
