# Azure deployment script for Medical System
# This script will create and deploy resources to Azure Free Tier

# Variables - change these as needed
$resourceGroupName = "MedicalSystemRG"
$location = "eastus"  # Choose a location close to you
$appServicePlanName = "MedicalSystemPlan"
$webAppName = "medical-system-api"  # This must be globally unique
$dbServerName = "medical-system-db"  # This must be globally unique
$dbName = "medicalsystemdb"
$dbAdminUser = "dbadmin"
$dbAdminPassword = "P@ssw0rd1234!"  # Change this to a secure password

# Login to Azure
Write-Host "Logging in to Azure..."
az login

# Create Resource Group
Write-Host "Creating Resource Group..."
az group create --name $resourceGroupName --location $location

# Create App Service Plan (Free Tier)
Write-Host "Creating App Service Plan (F1 Free Tier)..."
az appservice plan create --name $appServicePlanName --resource-group $resourceGroupName --sku F1 --is-linux

# Create Web App
Write-Host "Creating Web App..."
az webapp create --name $webAppName --resource-group $resourceGroupName --plan $appServicePlanName --runtime "DOTNETCORE:8.0"

# Create PostgreSQL Flexible Server (Basic Tier)
Write-Host "Creating PostgreSQL Flexible Server..."
az postgres flexible-server create `
    --name $dbServerName `
    --resource-group $resourceGroupName `
    --location $location `
    --admin-user $dbAdminUser `
    --admin-password $dbAdminPassword `
    --sku-name B_Standard_B1ms `
    --tier Burstable `
    --storage-size 32 `
    --version 15 `
    --database-name $dbName

# Allow Azure services to access PostgreSQL
Write-Host "Configuring PostgreSQL firewall rules..."
az postgres flexible-server firewall-rule create `
    --name AllowAzureServices `
    --resource-group $resourceGroupName `
    --server-name $dbServerName `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0

# Build and publish the application
Write-Host "Building and publishing the application..."
dotnet publish -c Release -o ./publish MedicalSystem.API/MedicalSystem.API.csproj

# Deploy to Azure Web App
Write-Host "Deploying to Azure Web App..."
Compress-Archive -Path ./publish/* -DestinationPath ./publish.zip -Force
az webapp deployment source config-zip --resource-group $resourceGroupName --name $webAppName --src ./publish.zip

# Configure connection string in Azure
Write-Host "Configuring connection string..."
$connectionString = "Host=$dbServerName.postgres.database.azure.com;Port=5432;Database=$dbName;Username=$dbAdminUser;Password=$dbAdminPassword;SSL Mode=Require;"
az webapp config connection-string set --resource-group $resourceGroupName --name $webAppName --connection-string-type PostgreSQL --settings DefaultConnection="$connectionString"

# Configure app settings
Write-Host "Configuring app settings..."
az webapp config appsettings set --resource-group $resourceGroupName --name $webAppName --settings ASPNETCORE_ENVIRONMENT="Production"

Write-Host "Deployment completed successfully!"
Write-Host "Your API is available at: https://$webAppName.azurewebsites.net"
Write-Host "Swagger UI is available at: https://$webAppName.azurewebsites.net/swagger" 