﻿using Microsoft.AspNetCore.Mvc;
using Projet2Groupe1.Models;
using Projet2Groupe1.ViewModels;

namespace Projet2Groupe1.Controllers
{
    public class PaymentController : Controller
    {
        [HttpGet]
        public IActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Payment(PaymentViewModel Paiement)
        {
            if (Paiement == null)
            {
                Console.WriteLine("Le modèle Paiement est null !");
                return View("Error"); // Ou une autre vue appropriée
            }
            using (IMemberService ims = new MemberService(new DataBaseContext())) 
            { 
                Member Member = ims.GetMember(Paiement.MemberId);
                Console.WriteLine("Le Member avant modification : " + Member.ToString());
                Member.IsPayed = true;
                Member UpdatedMember = ims.UpdateMember(Member);
                
                Console.WriteLine("Le Member apres modification : " + UpdatedMember.ToString());

            }
                Console.WriteLine("Apres paiement valide - memberId vaut :" + Paiement.MemberId);
            return RedirectToAction("Connection", "Login");
        }
    }
}