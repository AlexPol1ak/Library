namespace Library.DatabaseСonnection
{
    /// <summary>
    /// Класс содержит ключи используемые в json строке подключений к базе данных
    /// </summary>
    public class DbConnectionsData
    {
        static public Dictionary<string, Dictionary<string, string>> ServerData = new();

        static DbConnectionsData()
        {
            Dictionary<string, string> localServer = new Dictionary<string, string>()
            {
                {"ConnectionName", "LocalConnection" },
                {"SqlVersion", "MySQLVersionLocal" },
                {"pass", "" }
            };

            Dictionary<string, string> BegetServer = new Dictionary<string, string>()
            {
                {"ConnectionName", "ServerBegetConnection" },
                {"SqlVersion", "MySQLVersionBeget" },
                {"pass", "12345" }
            };

            ServerData.Add("Local", localServer);
            ServerData.Add("Beget Server", BegetServer);
        }
    }
}
