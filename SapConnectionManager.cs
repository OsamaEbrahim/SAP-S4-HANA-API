using SAP.Middleware.Connector;
using System;
using System.Configuration;

namespace SAP_S4_Hana_API
{
    public sealed class SapConnectionManager
    {
        private static readonly Lazy<RfcDestination> lazyConnection = new Lazy<RfcDestination>(() =>
        {
            var configParams = new RfcConfigParameters
        {
            { RfcConfigParameters.Name, ConfigurationManager.AppSettings["SAPDestinationName"] },
            { RfcConfigParameters.AppServerHost, ConfigurationManager.AppSettings["SAPAppServerHost"] },
            { RfcConfigParameters.SystemNumber, ConfigurationManager.AppSettings["SAPSystemNumber"] },
            { RfcConfigParameters.User, ConfigurationManager.AppSettings["SAPUser"] },
            { RfcConfigParameters.Password, ConfigurationManager.AppSettings["SAPPassword"] },
            { RfcConfigParameters.Client, ConfigurationManager.AppSettings["SAPClient"] },
            { RfcConfigParameters.Language, ConfigurationManager.AppSettings["SAPLanguage"] },
            { RfcConfigParameters.PeakConnectionsLimit, ConfigurationManager.AppSettings["MaxPoolSize"] }
        };

            return RfcDestinationManager.GetDestination(configParams);
        });

        public static RfcDestination Connection => lazyConnection.Value;

        private SapConnectionManager() { }
    }
}