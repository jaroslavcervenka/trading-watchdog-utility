using Ardalis.GuardClauses;

namespace Watchdog.App.Model
{
    public class ConnectionConfig
    {
        public string Ip { get; }
        
        public ulong Login { get; }
        
        public string Password { get; }

        public ConnectionConfig(
            string ip,
            ulong login,
            string password)
        {
            Guard.Against.NullOrEmpty(ip, nameof(ip));
            Guard.Against.Default(login, nameof(login));
            Guard.Against.NullOrEmpty(password, nameof(password));

            Ip = ip;
            Login = login;
            Password = password;
        }
    }
}