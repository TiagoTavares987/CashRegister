using CashRegisterCore.Entities;
using System.Collections.Generic;
using System.Linq;

namespace CashRegisterCore.Providers
{
    internal class UserProvider
    {
        public IEnumerable<User> GetAll() => AppCore.Db.GetAll<User>();

        public User GetUser(int id) => AppCore.Db.GetEntity<User>(id);

        public int InsertUser(User user) => AppCore.Db.Insert(user);

        public int UpdateUser(User user) => AppCore.Db.Update(user);

        public int DeleteUser(User user) => AppCore.Db.Delete(user);

        public User GetUserByUsername(string username)
            => AppCore.Db.GetAll<User>(new Dictionary<string, object>() { { nameof(User.Username), username } }).SingleOrDefault();
    }
}
