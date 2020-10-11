using Manifest.Config;
using Manifest.Services.Rds;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manifest.Services
{
    public class DataFactory
    {
        private static readonly Lazy<DataFactory> lazy = new Lazy<DataFactory>(() => new DataFactory());
        private static readonly IDataClient RdsClient = new RdsClient(RdsConfig.BaseUrl);
        private static IDataClient DefaultDataClient = RdsClient;

        public static DataFactory Instance { get { return lazy.Value; } }

        public IDataClient GetDataClient()
        {
            return DefaultDataClient;
        }

        public IDataClient GetDataClient(string name)
        {
            switch (name)
            {
                case "RDS":
                    return RdsClient;
                default:
                    return DefaultDataClient;
            }
        }

    }
}
