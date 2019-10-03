#Installation du module Azure si celui-ci n'est pas déjà présent
#Install-Module -Name Az -AllowClobber

#Authentification interactive
#Connect-AzAccount

#Authentification application
$applicationId = "f2280bac-61b9-4feb-9f69-c4e5e5f32644";
$securePassword = "lC6/d1dmMrSzu7sLuguhSUF358D561ZsbuvT+THaFsg=" | ConvertTo-SecureString -AsPlainText -Force
$credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $applicationId, $securePassword

Connect-AzAccount -ServicePrincipal -Credential $credential -Tenant "18907841-8c13-49e8-9fc3-7f8b8fcbc2f5"

#Configuration de l'environnement
$resourceGroupName = "LivreADF"
$region = "West Europe"
$dataFactoryName = "PowerShellFactory"


#Création de l'Azure Data Factory
$DataFactory = Set-AzDataFactoryV2 -ResourceGroupName $resourceGroupName `
    -Location $region -Name $dataFactoryName -Force

#Création de l'Integration Runtime Auto-Hébergé
$IntegrationRuntimeName = "LivreIRAutoHeb"
Set-AzDataFactoryV2IntegrationRuntime `
            -ResourceGroupName $resourceGroupName `
            -DataFactoryName $dataFactoryName `
            -Name $IntegrationRuntimeName `
            -Type SelfHosted `
            -Description 'Integration Runtime du projet ...' `
            -Force

#Changement du repertoire pour localiser les fichiers JSON
cd "D:\GIT\livres\Azure Data Factory\Chapitre 4 - Développement ADF\Samples\PowerShell"
#Deploiement du service lié relatif au File System
Set-AzDataFactoryV2LinkedService `
-DataFactoryName $dataFactoryName `
-ResourceGroupName $resourceGroupName -Name "FS_PartageOnPremise" `
-DefinitionFile ".\LS_FS_PartageOnPremise.json" `
-Force

#Deploiement du service lié relatif à la base SQL Azure
Set-AzDataFactoryV2LinkedService `
-DataFactoryName $dataFactoryName `
-ResourceGroupName $resourceGroupName -Name "SQDB_AdventureWorks" `
-DefinitionFile ".\LS_SQDB_AdventureWorks.json"`
-Force

#Deploiement des jeux de données

Set-AzDataFactoryV2Dataset `
-ResourceGroupName $resourceGroupName `
-DataFactoryName $dataFactoryName `
-Name "FS_Customer"`
-DefinitionFile ".\DS_FS_Customer.json"`
-Force

Set-AzDataFactoryV2Dataset `
-ResourceGroupName $resourceGroupName `
-DataFactoryName $dataFactoryName `
-Name "SQDB_Col_Customer"`
-DefinitionFile ".\DS_SQDB_Col_Customer.json"`
-Force

#Deploiement du pipeline

Set-AzDataFactoryV2Pipeline `
    -DataFactoryName $dataFactoryName `
    -ResourceGroupName $resourceGroupName `
    -Name "Col_Customer" `
    -DefinitionFile ".\P_Col_Customer.json"`
    -Force

$runId = Invoke-AzDataFactoryV2Pipeline `
-DataFactory $dataFactoryName `
-ResourceGroupName $resourceGroupName `
-PipelineName "Col_Customer"

#while ($True) {
#    $run = Get-AzDataFactoryV2PipelineRun -ResourceGroupName $resourceGroupName -DataFactoryName $dataFactoryName -PipelineRunId $runId

#    if ($run) {
#        if ($run.Status -ne 'InProgress') {
#            Write-Host "Pipeline terminé. Le statut est: " $run.Status -foregroundcolor "Yellow"
#            $run
#            break
#        }
#        Write-Host  "Traitement en cours..." -foregroundcolor "Yellow"
#    }

#    Start-Sleep -Seconds 30
#}

Set-AzDataFactoryV2Trigger `
-ResourceGroupName $resourceGroupName `
-DataFactoryName $dataFactoryName `
-Name "Daily_01h_Schedule" `
-DefinitionFile ".\TRG_Daily_01h_Schedule.json"

Start-AzDataFactoryV2Trigger `
-ResourceGroupName $resourceGroupName `
-DataFactoryName $dataFactoryName `
-Name "Daily_01h_Schedule"