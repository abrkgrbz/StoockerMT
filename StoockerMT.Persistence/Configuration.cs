using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace StoockerMT.Persistence
{
    public static class Configuration
    { 
        static public string ConnectionStringMasterDb
        {
            get
            {

                ConfigurationManager configurationManager = new();
                try
                {
                    configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../StoockerMT.API"));
                    configurationManager.AddJsonFile("appsettings.json");
                    configurationManager.AddJsonFile("appsettings.Development.json", optional: true);
                }
                catch(Exception e)
                {
                   Console.Write(e);
                }

                return configurationManager.GetConnectionString("MasterConnection");
            }
        }

        static public string ConnectionStringTenantDb
        {
            get
            {

                ConfigurationManager configurationManager = new();
                try
                {
                    configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../StoockerMT.API"));
                    configurationManager.AddJsonFile("appsettings.json");
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }

                return configurationManager.GetConnectionString("TenantConnection");
            }
        }
    }
}
