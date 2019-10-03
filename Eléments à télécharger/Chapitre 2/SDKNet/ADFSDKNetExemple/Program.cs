using System;
using Microsoft.Rest;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ADFSDKNetExemple
{
    class Program
    {
        static void Main(string[] args)
        {
            /**************************************************************
             * Variables nécessaires à la création d'un client Azure Data Factory
             * *************************************************************/
            string tenantID = "<your tenant ID>";
            string applicationId = "<Application ID>";
            string authenticationKey = "<Authentication key pour l'application>";
            string subscriptionId = "<L'id de la souscription Azure>";
            string resourceGroup = "<Le nom du ressource group contenant l'Azure Data Factory>";
            string region = "West Europe";
            string dataFactoryName = "<Nom du service Azure de la Data Factory, celui-ci doit être unique au sein du tenant.>";

            // Authentification auprès d'Azure
            var context = new AuthenticationContext("https://login.windows.net/" + tenantID);
            ClientCredential cc = new ClientCredential(applicationId, authenticationKey);
            AuthenticationResult result = context.AcquireTokenAsync("https://management.azure.com/", cc).GetAwaiter().GetResult();
            ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);

            //Création d'un client Azure Data Factory
            var client = new DataFactoryManagementClient(cred) { SubscriptionId = subscriptionId };

            // Création d'une Azure data factory
            Console.WriteLine("Création data factory " + dataFactoryName + "...");
            Factory dataFactory = new Factory
            {
                Location = region,
                Identity = new FactoryIdentity()
            };
            client.Factories.CreateOrUpdate(resourceGroup, dataFactoryName, dataFactory);
        }
    }
}
