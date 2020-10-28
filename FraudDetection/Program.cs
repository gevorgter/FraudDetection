using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FraudDetection
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TransactionHelper.Init();
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
