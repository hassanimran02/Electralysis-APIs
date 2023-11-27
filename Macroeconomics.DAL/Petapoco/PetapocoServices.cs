using Microsoft.Extensions.Configuration;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.DAL.Petapoco
{
    public class PetapocoServices
    {
        private readonly IConfiguration Configuration;
        public string ConnectionString { get; set; }


        public PetapocoServices(IConfiguration configuration)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetConnectionString("argaamPlusConnection") ?? "";
        }

        public DatabaseService GetInstance()
        {
            return new DatabaseService(ConnectionString);
        }

        public class DatabaseService : Database
        {
            public DatabaseService(string config) : base(config, "System.Data.SqlClient")
            {

            }
        }
    }
}
