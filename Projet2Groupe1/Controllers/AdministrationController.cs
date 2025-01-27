﻿using Microsoft.AspNetCore.Mvc;
using Projet2Groupe1.Models;
using Projet2Groupe1.ViewModels;

namespace Projet2Groupe1.Controllers
{
    public class AdministrationController : Controller
    {     
        public IActionResult HandlerRegistration(User user)
        {

            Console.WriteLine("Je suis dans la methode HandlerRegistration");
            
            Console.WriteLine("Lastname est " + user.LastName);
            using (IUserService ius = new UserService(new DataBaseContext())) 
            {
                User UpdatedUser = ius.UpdateUserStatus(user);
            }

            using (IAdminService ias = new AdminService(new DataBaseContext()))
            {
                List<User> OrganizerStatusPending = ias.GetUsersByStatus(statusRegistration.PENDING, UserRole.ORGANIZER);
                List<User> ProvierStatusPending = ias.GetUsersByStatus(statusRegistration.PENDING, UserRole.PROVIDER);

                HandlerRegistrationViewModel handlerRegistrationViewModel = new HandlerRegistrationViewModel()
                {
                    OrganizerStatusPending = OrganizerStatusPending,
                    ProviderStatusPending = ProvierStatusPending,
                };
                Console.WriteLine("list de pending organizer : " + handlerRegistrationViewModel.OrganizerStatusPending.Count);
                return View("ShowRegistration", handlerRegistrationViewModel);
            }
        }

        public IActionResult ShowRegistration() 
        {
            using (IAdminService ias = new AdminService(new DataBaseContext()))
            {
                List<User> OrganizerStatusPending = ias.GetUsersByStatus(statusRegistration.PENDING, UserRole.ORGANIZER);
                List<User> ProvierStatusPending = ias.GetUsersByStatus(statusRegistration.PENDING, UserRole.PROVIDER);

                HandlerRegistrationViewModel handlerRegistrationViewModel = new HandlerRegistrationViewModel()
                {
                    OrganizerStatusPending = OrganizerStatusPending,
                    ProviderStatusPending = ProvierStatusPending,
                };
                Console.WriteLine("list de pending organizer : " + handlerRegistrationViewModel.OrganizerStatusPending.Count);
                return View(handlerRegistrationViewModel);
            }
        }

        public IActionResult HandlerEvent(Event evenment) 
        {
            Console.WriteLine("Je suis dans la methode HandlerRegistration");

            Console.WriteLine("Event gere: " + evenment.NameEvent);

            using (IEventService ies = new EventService(new DataBaseContext())) 
            {
                Event evenmentUptader = ies.UpdateEventStatus(evenment);
            }

            using (IAdminService ias = new AdminService(new DataBaseContext()))
            {
                List<Event> EventStatusPending = ias.GetEventsByStatus(statusRegistration.PENDING);

                HandlerEventViewModel handlerEventViewModel = new HandlerEventViewModel()
                {
                    EventStatusPending = EventStatusPending
                };
                Console.WriteLine("list de pending organizer : " + handlerEventViewModel.EventStatusPending.Count());
                return View("ShowEvent", handlerEventViewModel);
            }
        }
        public IActionResult ShowEvent()
        {
            using (IAdminService ias = new AdminService(new DataBaseContext()))
            {
                List<Event> EventStatusPending = ias.GetEventsByStatus(statusRegistration.PENDING);

                HandlerEventViewModel handlerEventViewModel = new HandlerEventViewModel()
                {
                    EventStatusPending = EventStatusPending
                };
                Console.WriteLine("list de pending organizer : " + handlerEventViewModel.EventStatusPending.Count());
                return View(handlerEventViewModel);
            }
        }
    }
}