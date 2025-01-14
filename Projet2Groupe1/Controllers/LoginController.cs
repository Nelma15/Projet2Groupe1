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
                Console.WriteLine("dans le using connection, verification de lauthentification");
                User user = ius.Authentication(userViewModel.User.Mail, userViewModel.User.Password);
                Console.WriteLine("Apres authentification, user reucperer ? " + user.FirstName);
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
                    Console.WriteLine("Juste avant le if pour rediriger vers lerreur ");
                    if (user.StatusRegistration == statusRegistration.PENDING) {
                        Console.WriteLine("Le role est En attente. Le user ne peut pas se connecter et est redirige vesr error.cshtml");
                        return RedirectToAction("Error", "Error", new { errorCode = 1, Message = "Erreur, votre compte est en attente de validation. Il sera valide dans les 24h suivant l'inscription." });
                    } else if (user.StatusRegistration == statusRegistration.REFUSED) {
                        Console.WriteLine("Le role est Refused. Le user ne peut pas se connecter et est redirige vesr error.cshtml");
                        return RedirectToAction("Error", "Error", new { errorCode = 1, message = "Erreur, votre compte a ete refuse. Veuillez contacter l'administrateur." });
                    }
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
