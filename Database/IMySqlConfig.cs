namespace Database
{
    public interface IMySqlConfig
    {
        string Address { get; set; }
        string Port { get; set; }
        string Database { get; set; }
        string Username { get; set; }
        string Password { get; set; }

        void Save();
    }
}
