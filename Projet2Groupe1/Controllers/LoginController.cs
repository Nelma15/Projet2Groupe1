﻿using System.Data;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Projet2Groupe1.Models;
using Projet2Groupe1.ViewModels;

namespace Projet2Groupe1.Controllers
{
    public class LoginController : Controller
    {
        //[HttpGet]
        //public IActionResult CreateAccount()
        //{
        //    return View();
        //}
        //[HttpPost]
        ////public IActionResult CreateAccount(User user)
        ////{
        ////    using (IUserService ius = new UserService(new DataBaseContext()))
        ////    {
        ////        Console.WriteLine("vérification du model state " + ModelState.IsValid);
        ////        if (ModelState.IsValid && ius.searchUser(user.Id) == null)
        ////        {
        ////            ius.CreateUser(user.FirstName, user.LastName, user.Phone, user.Mail, user.Password, user.Newsletter, user.Role);
        ////            Console.WriteLine("Création" + user.ToString());
        ////        }
        ////        return View();
        ////    }
        ////}
      
        public IActionResult Connection()
        {
            using (IUserService ius = new UserService(new DataBaseContext()))
            {
                Console.WriteLine("IsAuthenticated est a " + HttpContext.User.Identity.IsAuthenticated);
                UserViewModel uvm = new UserViewModel
                {
                    Authenticate = HttpContext.User.Identity.IsAuthenticated
                };
                              
            if (uvm.Authenticate)
                {
                    Console.WriteLine("je suis deja authentifie");
                    uvm.User = ius.GetUser(HttpContext.User.Identity.Name);
                }
                
                return View(uvm);
            }

        }
        [HttpPost]
        public IActionResult Connection(UserViewModel userViewModel, string returnUrl)
        {

            using (IUserService ius = new UserService(new DataBaseContext()))
            {
                User user = ius.Authentication(userViewModel.User.Mail, userViewModel.User.Password);
                if (user != null) // bon mot de passe
                {
                    Console.WriteLine("connexion reussie");

                    List<Claim> userClaims = new List<Claim>()
                    {
                         new Claim(ClaimTypes.Name, user.Id.ToString()),
                         new Claim(ClaimTypes.Role, user.Role.ToString()),
                    };
                    
                    Console.WriteLine(userClaims);

                    var ClaimIdentity = new ClaimsIdentity(userClaims, "User Identity");

                    var userPrincipal = new ClaimsPrincipal(new[] { ClaimIdentity });

                    HttpContext.SignInAsync(userPrincipal);

                    if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    // Retrieve the ClaimsIdentity (assuming there's one identity)
                    var claimIdentity = userPrincipal.Identity as ClaimsIdentity;

                    // Retrieve the user.Id value from the Name claim
                    if (claimIdentity != null)
                    {
                        var userIdClaim = claimIdentity.FindFirst(ClaimTypes.Name);
                        if (userIdClaim != null)
                        {
                            var userId = userIdClaim.Value; // This is the user.Id value
                            Console.WriteLine($"User ID: {userId}");
                        }
                        else
                        {
                            Console.WriteLine("User ID claim not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No ClaimsIdentity found.");
                    }



                    //return Redirect("/Login/DashBoardAdmin");
                    return Redirect(user.Role);
                }
                ModelState.AddModelError("Utilisateur.Nom", "Nom et/ou mot de passe incorrect(s)");
                return View(userViewModel);
            }
           

        }

        [Authorize(Roles = "ORGANIZER")]
        public IActionResult DashBoardOrganizer(UserRole DashboardType)
        { 
            ViewData["Role"] = "ORGANIZER";
            return View();
        }

        [Authorize(Roles = "ADMIN")]
        public IActionResult DashBoardAdmin(UserRole DashboardType)
        {
            ViewData["Role"] = "ADMIN";
            return View();
        }
        [Authorize(Roles = "PROVIDER")]
        public IActionResult DashBoardProvider(UserRole DashboardType)
        {
            ViewData["Role"] = "PROVIDER";
            return View();
        }
        [Authorize(Roles = "MEMBER")]
        public IActionResult DashBoardMEMBER(UserRole DashboardType)
        {
            ViewData["Role"] = "MEMBER";
            return View();
        }
        [Authorize(Roles = "PREMIUM")]
        public IActionResult DashBoardPREMIUM(UserRole DashboardType)
        {
            ViewData["Role"] = "PREMIUM";
            return View();
        }
        [Authorize]
        public IActionResult Redirect(UserRole DashboardType)
        {
            
            Console.WriteLine("voici le dashboardtype : " + DashboardType.ToString());
            TempData["Role"] = DashboardType;
            switch (DashboardType.ToString())
            {
                case "ADMIN":
                    return View("DashBoardAdmin");
                case "MEMBER":
                    return View("DashBoardMember");
                case "PREMIUM":
                    return View("DashBoardPremium");
                case "ORGANIZER":
                    return View("DashBoardOrganizer");
                case "PROVIDER":
                    return View("DashBoardProvider");
                default:
                    Console.WriteLine("je suis en role par defaut");
                    return null;
                }
        }
       
        public IActionResult Disconnection()
        {
            HttpContext.SignOutAsync();
          
            return RedirectToAction("Connection", "Login");
        }


    }
}
