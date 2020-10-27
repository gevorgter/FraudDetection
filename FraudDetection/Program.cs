using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FraudDetection
{
    public class Program
    {
        public static ObjectAnalyzer _ob;
        public static void Main(string[] args)
        {
            _ob = new ObjectAnalyzer();
            _ob.Analyze(typeof(Transaction));

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
