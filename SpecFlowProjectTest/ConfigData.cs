using Microsoft.Extensions.Configuration;

namespace SpecFlowProjectTest.Tests
{
    public static class ConfigData
    {
        public static ConfigDataModel Get()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            return config.Get<ConfigDataModel>()!;
        }
    }
    public class ConfigDataModel
    {
        public required string Browser { get; init; }
        public required string BaseUrl { get; init; }
        public required bool Headless { get; init; }
    }
}