﻿namespace Projet2Groupe1.Models
{
    public interface IEventService : IDisposable
    {
        public int CreateEvent(TypeEvent TypeEvent, string NameEvent, DateTime StartEvent, DateTime EndEvent, Adress Adress, Artist Artist, Ticket Ticket, Service Service);
        public int CreateEvent(TypeEvent TypeEvent, string NameEvent, DateTime StartEvent, DateTime EndEvent, Adress? AdressData, Artist? Artist, Ticket? Ticket, Service? Service, int userId);

        public Event searchEvent(int id);
        public int UpdateEvent(int id, TypeEvent TypeEvent, string NameEvent, DateTime StartEvent, DateTime EndEvent, Adress? Adress, Artist? Artist, Ticket? Ticket, Service? Service);

        public List<Event> searchEventList(int userId);
        public void DeleteEvent(int id);


        public List<Event> GetAllEvents();

        public List<Event> GetFilteredEvents(string category = null, string city = null, string search = null);


        //pensez à ajouter le depot d'image =================================================================================================================
    }
}