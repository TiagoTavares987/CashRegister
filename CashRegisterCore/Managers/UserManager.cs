using CashRegisterCore.Entities;
using CashRegisterCore.Providers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CashRegisterCore.Managers
{
    public class UserManager
    {
        private UserProvider _provider;

        public UserManager() => _provider = new UserProvider();

        public IEnumerable<User> GetAll() => _provider.GetAll();

        public User GetUser(int id) => _provider.GetUser(id);

        public void SaveUser(User user)
        {
            ValidateUser(user);
            if (user.Id == 0)
            {
                var existingUser = _provider.GetUserByUsername(user.Username);
                if (existingUser != null)
                    throw new ArgumentException("Username already exists.");

                user.Password = null;
                var id = _provider.InsertUser(user);
                if (id > 0)
                    user.Id = id;
            }
            else
                _provider.UpdateUser(user);
        }

        public int DeleteUser(int id)
        {
            User user = GetUser(id);
            if (user == null)
                throw new ArgumentNullException("Invalid user.");

            return _provider.DeleteUser(user);
        }

        public bool ChangePassword(int id, string oldPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return false;

            var user = GetUser(id);
            if (user is null)
                return false;

            if (!string.IsNullOrWhiteSpace(user.Password) && !EncriptPassword(user.Password).Equals(oldPassword))
                return false;

            user.Password = EncriptPassword(newPassword);
            return _provider.UpdateUser(user) == 1;
        }

        public bool ValidateLogin(int id, string password)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(password))
                return false;

            User user = _provider.GetUser(id);
            if (user == null)
                return false;

            if (EncriptPassword(password).Equals(user.Password))
            {
                AppCore.UserId = user.Id;
                return true;
            }

            return false;
        }

        private void ValidateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Invalid username.");
            if (!user.IsAdmin && user.Id == AppCore.UserId && _provider.GetUser(AppCore.UserId).IsAdmin)
                throw new ArgumentException("Admin can not remove it self.");

        }

        private static string EncriptPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                    sb.Append(hashBytes[i].ToString("X2"));

                return sb.ToString().ToLower();
            }
        }
    }
}
