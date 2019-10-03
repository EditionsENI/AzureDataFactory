using Microsoft.Rest;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Rest.Serialization;
using System;
using System.Collections.Generic;

namespace ADFLivre
{
    class Program
    {

        public static string _repertoireId  = "18907841-8c13-49e8-9fc3-7f8b8fcbc2f5";
        public static string _applicationId = "f2280bac-61b9-4feb-9f69-c4e5e5f32644";
        public static string _applicationKey = "lC6/d1dmMrSzu7sLuguhSUF358D561ZsbuvT+THaFsg=";
        public static string _abonnementId = "9cf0531d-ac3b-4585-ad1c-e0e1e028b749";
        public static string _ressourceGroup = "livre";
        public static string _region = "West Europe";
        public static string _dataFactoryName = "CSharpFactory";
        
        public static string _IntegrationRuntimeName = "LivreIRAutoHeb";
        public static string _FS_PartageOnPremiseName = "FS_PartageOnPremise";
        public static string _SQDB_AdventureWorksName = "SQDB_AdventureWorks";
        public static string _FS_CustomerName = "FS_Customer";
        public static string _SQDB_Col_CustomerName = "SQDB_Col_Customer";
        static void Main(string[] args)
        {
            //Authentification auprès d'Azure avec l'application svc_adf
            AuthenticationContext context = new AuthenticationContext("https://login.windows.net/" + _repertoireId);
            ClientCredential cc = new ClientCredential(_applicationId, _applicationKey);
            AuthenticationResult result = context.AcquireTokenAsync("https://management.azure.com/", cc).Result;
            ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);
            DataFactoryManagementClient ADFclient = new DataFactoryManagementClient(cred) { SubscriptionId = _abonnementId };

            //Création d'une Azure Data Factory

            Factory dataFactory = new Factory
            {
                Location = _region,
                Identity = new FactoryIdentity()
            };
            ADFclient.Factories.CreateOrUpdate(_ressourceGroup, _dataFactoryName, dataFactory);
            Console.WriteLine(SafeJsonConvert.SerializeObject(dataFactory, ADFclient.SerializationSettings));
            var toto = ADFclient.Factories.Get(_ressourceGroup, _dataFactoryName).ProvisioningState;
            while (ADFclient.Factories.Get(_ressourceGroup, _dataFactoryName).ProvisioningState == "PendingCreation")
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("*");
            }

            ////Création d'un Integration Runtime Auto-Hébergé

            //IntegrationRuntimeResource integrationRuntimeResource = new IntegrationRuntimeResource(
            //new SelfHostedIntegrationRuntime
            //{
            //    Description = "L'Integration Runtime du projet ..."
            //}
            //);
            //ADFclient.IntegrationRuntimes.CreateOrUpdate(_ressourceGroup, _dataFactoryName, _IntegrationRuntimeName, integrationRuntimeResource);
            //Console.WriteLine(SafeJsonConvert.SerializeObject(integrationRuntimeResource, ADFclient.SerializationSettings));
            //Console.WriteLine("Authkey : " + ADFclient.IntegrationRuntimes.ListAuthKeys(_ressourceGroup, _dataFactoryName, _IntegrationRuntimeName).AuthKey1);

            //Création service lié File System on premise

            IntegrationRuntimeReference integrationRuntimeReference = new IntegrationRuntimeReference(_IntegrationRuntimeName);
            SecureString secureString = new SecureString("MonPassword");
            LinkedServiceResource FS_PartageOnPremise = new LinkedServiceResource(
                    new FileServerLinkedService
                    {
                        Description = "Service lié référençant un espace partagé dans le réseau privé de l'entreprise",
                        ConnectVia = integrationRuntimeReference,
                        Host = @"\\IRAutoHeberge\Dépôt",
                        UserId = "chsauget",
                        Password = secureString
                    }
                );

            ADFclient.LinkedServices.CreateOrUpdate(_ressourceGroup, _dataFactoryName, _FS_PartageOnPremiseName, FS_PartageOnPremise);
            Console.WriteLine(SafeJsonConvert.SerializeObject(FS_PartageOnPremise, ADFclient.SerializationSettings));

            //Création service lié Azure SQLDB

            SecureString SQLsecureString = new SecureString("integrated security=False;encrypt=True;connection timeout=30;data source=adflivre.database.windows.net;initial catalog=advwrks;user id=chsauget;Password=toto");

            LinkedServiceResource SQDB_AdventureWorks = new LinkedServiceResource(
                    new AzureSqlDatabaseLinkedService
                    {
                        Description = "Service lié référençant un espace partagé dans le réseau privé de l'entreprise",
                        ConnectionString = SQLsecureString,
                    }
                );
            ADFclient.LinkedServices.CreateOrUpdate(_ressourceGroup, _dataFactoryName, _SQDB_AdventureWorksName, SQDB_AdventureWorks);
            Console.WriteLine(SafeJsonConvert.SerializeObject(SQDB_AdventureWorks, ADFclient.SerializationSettings));

            //Création jeu de données FS_Customer 

            DatasetResource FS_Customer = new DatasetResource(
                                new FileShareDataset
                                {
                                    LinkedServiceName = new LinkedServiceReference
                                    {
                                        ReferenceName = _FS_PartageOnPremiseName
                                    }
                                    , FolderPath = "AdventureWorks CSV"
                                    , FileName = "Customer.csv"
                                    , Format = new TextFormat
                                    {
                                        ColumnDelimiter = "\t",
                                        RowDelimiter = "\n",
                                        FirstRowAsHeader = false
                                    }
                                    ,Structure = new List<DatasetDataElement>
                                                    {
                                                        new DatasetDataElement
                                                        {
                                                            Name = "CustomerID",
                                                            Type = "Int32"
                                                        },
                                                        new DatasetDataElement
                                                        {
                                                            Name = "PersonID",
                                                            Type = "Int32"
                                                        },
                                                        new DatasetDataElement
                                                        {
                                                            Name = "StoreID",
                                                            Type = "Int32"
                                                        },
                                                        new DatasetDataElement
                                                        {
                                                            Name = "TerritoryID",
                                                            Type = "Int32"
                                                        },
                                                        new DatasetDataElement
                                                        {
                                                            Name = "AccountNumber",
                                                            Type = "String"
                                                        },
                                                        new DatasetDataElement
                                                        {
                                                            Name = "rowguid",
                                                            Type = "String"
                                                        },
                                                        new DatasetDataElement
                                                        {
                                                            Name = "ModifiedDate",
                                                            Type = "DateTime"
                                                        }
                                                    }
                                }
                                , name: _FS_CustomerName
                                );
            ADFclient.Datasets.CreateOrUpdate(_ressourceGroup, _dataFactoryName, _FS_CustomerName, FS_Customer);
            Console.WriteLine(SafeJsonConvert.SerializeObject(FS_Customer, ADFclient.SerializationSettings));

            //Création jeu de données SQDB_Col_Customer

            DatasetResource SQDB_Col_Customer = new DatasetResource(
                    new AzureSqlTableDataset
                    {
                        LinkedServiceName = new LinkedServiceReference
                        {
                            ReferenceName = _SQDB_AdventureWorksName
                        },
                        TableName = "col.Customer"
                    }
                    ,name: _SQDB_Col_CustomerName
                );
            ADFclient.Datasets.CreateOrUpdate(_ressourceGroup, _dataFactoryName, _SQDB_AdventureWorksName, SQDB_Col_Customer);
            Console.WriteLine(SafeJsonConvert.SerializeObject(SQDB_Col_Customer, ADFclient.SerializationSettings));

            //Création de l'activité de copie du fichier Customer
            CopyActivity CustomerCopy = new CopyActivity
            {
                Name = "Copy - Customer"
                ,
                Inputs = new List<DatasetReference>{
                    new DatasetReference()
                    {
                        ReferenceName = _FS_CustomerName
                    }
                }
                ,
                Outputs = new List<DatasetReference>{
                    new DatasetReference()
                    {
                        ReferenceName = _SQDB_Col_CustomerName

                    }
                }
                ,
                Source = new FileSystemSource
                {

                }
                ,
                Sink = new AzureTableSink
                {

                }
            };
            //Création de l'activité de copie du fichier Customer
            PipelineResource PipelineCustomer = new PipelineResource
            {
                Activities = new List<Activity> { CustomerCopy }
                ,Folder = new PipelineFolder { Name = "AdventureWorks"}
            };

            ADFclient.Pipelines.CreateOrUpdate(_ressourceGroup, _dataFactoryName, "Col_Customer", PipelineCustomer);
            Console.WriteLine(SafeJsonConvert.SerializeObject(PipelineCustomer, ADFclient.SerializationSettings));

            //Demander une execution du pipeline 
            CreateRunResponse runResponse = ADFclient.Pipelines.CreateRunWithHttpMessagesAsync(_ressourceGroup, _dataFactoryName, "Col_Customer").Result.Body;

            //Contrôler l'execution du pipeline
            PipelineRun run = ADFclient.PipelineRuns.Get(_ressourceGroup, _dataFactoryName, runResponse.RunId); 
            while (run.Status == "InProgress")
            {
                run = ADFclient.PipelineRuns.Get(_ressourceGroup, _dataFactoryName, runResponse.RunId);
                Console.WriteLine("Status: " + run.Status);
            }

//Déclencheur Quotidien 
TriggerResource scheduleTrigger = new TriggerResource(
    new ScheduleTrigger
    {
        Pipelines = new List<TriggerPipelineReference>{
            new TriggerPipelineReference {
                PipelineReference = new PipelineReference("Col_Customer")
            }
        }
        ,
        Recurrence = new ScheduleTriggerRecurrence
        {
            StartTime = DateTime.Parse("2019-03-30T01:00:00Z")
            ,
            Frequency = "Day"
            ,
            Interval = 1
        }
    }
    , name: "Daily_01h_Schedule"
    );
ADFclient.Triggers.CreateOrUpdate(_ressourceGroup, _dataFactoryName, "Daily_01h_Schedule", scheduleTrigger);
Console.WriteLine(SafeJsonConvert.SerializeObject(scheduleTrigger, ADFclient.SerializationSettings));

ADFclient.Triggers.BeginStart(_ressourceGroup, _dataFactoryName, "Daily_01h_Schedule");

        }
    }
}
