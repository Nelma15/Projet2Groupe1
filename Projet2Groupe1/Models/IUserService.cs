﻿namespace Projet2Groupe1.Models
{
    public interface IUserService : IDisposable
    {
        public int CreateUser(string FirstName, string LastName, int Phone, string Mail, string Password, UserRole Role = UserRole.DEFAULT);
        public User searchUser(int id);
    }
}
