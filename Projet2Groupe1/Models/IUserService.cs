﻿namespace Projet2Groupe1.Models
{
    public interface IUserService : IDisposable
    {
        public int CreateUser(string FirstName, string LastName, int Phone/*, string Mail*/, string Password/*, UserRole Role*/);
        public User searchUser(int id);
    }
}