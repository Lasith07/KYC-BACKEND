using Microsoft.Extensions.Configuration;

namespace vue_ts.Helpers.Utils.GlobalAttributes
{
    public static class GlobalAttributes
    {
        public static MySqlConfiguration MySqlConfiguration { get; set; } = new MySqlConfiguration();
    }

    public class MySqlConfiguration
    {
        public string? connectionString { get; set; } 
    }
}