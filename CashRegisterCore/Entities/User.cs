using Database;

namespace CashRegisterCore.Entities
{
    [IsDbTable(nameof(User))]
    public class User
    {
        [IsDbField(true)]
        public int Id { get; set; }
        [IsDbField]
        public bool IsAdmin { get; set; }
        [IsDbField]
        public string Username { get; set; }
        [IsDbField]
        public string Password { get; set; }
        [IsDbField]
        public int ImageId { get; set; }

        public User Clone() => (User)MemberwiseClone();
    }
}
