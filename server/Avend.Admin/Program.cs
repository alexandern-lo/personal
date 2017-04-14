using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Avend.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"
Usage: Avend.Admin <command> <arg1>, <arg2>, ...
Commands:
    user <id> - search user with name/email starting or equal to <id>.
    exprops - list extension properties defined in Azure AD.
    grant_admin <admin_uid> {Yes|No} - grant super admin rights to user.
");
                return;
            }

            var root = Directory.GetCurrentDirectory();
            var env = Environment.GetEnvironmentVariable("ADMIN_ENV") ?? "development";
            var builder = new ConfigurationBuilder()
                .SetBasePath(root)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", false)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var tenantConfig = new AppConfig();
            config.Bind(tenantConfig);

            var app = new AdminApp(tenantConfig);
            Run(args, app).Wait();
        }

        private static async Task Run(string[] args, AdminApp app)
        {
            var command = args[0];
            try
            {
                await app.Login();
                switch (command)
                {
                    case "exprops":
                    {
                        var props = await app.ListExtensionProps();
                        foreach (var p in props)
                        {
                            Console.WriteLine(p);
                        }
                        break;
                    }

                    case "grant_admin":
                    {
                        var userUid = args[1];
                        var grantType = args[2];
                        await app.GrantAdmin(userUid, grantType == "Yes");
                        break;
                    }

                    case "user":
                    {
                        var email = args[1];
                        var user = await app.GetUser(email);
                        Console.WriteLine(user);
                        break;
                    }
                    default:
                        Console.WriteLine("Unknown command {0}", command);
                        break;
                }
            }
            catch (AdminAppException e)
            {
                Console.WriteLine(e.Response);
                Console.WriteLine(e.Body);
                Console.WriteLine(e);
            }
        }
    }
}