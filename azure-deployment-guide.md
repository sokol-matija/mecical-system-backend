# Manual Deployment Guide for Medical System to Azure

This guide will walk you through the steps to manually deploy the Medical System application to Azure using the Azure Portal.

## Prerequisites

- An Azure account with an active subscription
- The Medical System application built and packaged (publish.zip)

## Step 1: Create a Resource Group

1. Sign in to the [Azure Portal](https://portal.azure.com)
2. Click on "Resource groups" in the left navigation
3. Click "Create" to create a new resource group
4. Enter the following details:
   - Subscription: Select your subscription
   - Resource group name: `MedicalSystemRG`
   - Region: Select a region close to you (e.g., East US)
5. Click "Review + create" and then "Create"

## Step 2: Create an Azure Database for PostgreSQL Flexible Server

1. In the Azure Portal, click "Create a resource"
2. Search for "Azure Database for PostgreSQL" and select it
3. Select "Flexible server" and click "Create"
4. Enter the following details:
   - Subscription: Select your subscription
   - Resource group: `MedicalSystemRG`
   - Server name: `medical-system-db` (must be globally unique)
   - Region: Same as your resource group
   - PostgreSQL version: 15
   - Workload type: Development
   - Compute + storage: Burstable B1ms (1 vCore, 2GB RAM)
   - Availability zone: No preference
   - High availability: No (to stay within free tier limits)
   - Admin username: `dbadmin`
   - Password: Create a secure password
5. Click "Next: Networking"
6. For "Firewall rules", select "Allow public access from any Azure service within Azure to this server"
7. Click "Review + create" and then "Create"
8. Wait for the deployment to complete

## Step 3: Create a Database

1. Once the PostgreSQL server is created, navigate to it
2. Click on "Databases" in the left navigation
3. Click "Add" to create a new database
4. Enter the following details:
   - Name: `medicalsystemdb`
   - Character set: UTF8
   - Collation: en_US.utf8
5. Click "Save"

## Step 4: Create an App Service Plan

1. In the Azure Portal, click "Create a resource"
2. Search for "App Service Plan" and select it
3. Click "Create"
4. Enter the following details:
   - Subscription: Select your subscription
   - Resource group: `MedicalSystemRG`
   - Name: `MedicalSystemPlan`
   - Operating System: Linux
   - Region: Same as your resource group
   - Pricing plan: Free F1 (1GB memory, 60 minutes/day compute)
5. Click "Review + create" and then "Create"
6. Wait for the deployment to complete

## Step 5: Create a Web App

1. In the Azure Portal, click "Create a resource"
2. Search for "Web App" and select it
3. Click "Create"
4. Enter the following details:
   - Subscription: Select your subscription
   - Resource group: `MedicalSystemRG`
   - Name: `medical-system-api` (must be globally unique)
   - Publish: Code
   - Runtime stack: .NET 8 (LTS)
   - Operating System: Linux
   - Region: Same as your resource group
   - App Service Plan: `MedicalSystemPlan`
5. Click "Review + create" and then "Create"
6. Wait for the deployment to complete

## Step 6: Configure Connection String

1. Once the Web App is created, navigate to it
2. Click on "Configuration" in the left navigation
3. Under "Connection strings", click "New connection string"
4. Enter the following details:
   - Name: `DefaultConnection`
   - Value: `Host=medical-system-db.postgres.database.azure.com;Port=5432;Database=medicalsystemdb;Username=dbadmin;Password=YOUR_PASSWORD;SSL Mode=Require;`
   - Type: PostgreSQL
5. Click "OK"
6. Click "Save" at the top of the page

## Step 7: Deploy the Application

1. In the Web App, click on "Deployment Center" in the left navigation
2. Select "Local Git/FTPS" as the source
3. Click "Save"
4. Click on "Advanced Tools" in the left navigation
5. Click "Go" to open Kudu
6. Click on "ZIP Deploy" in the top navigation
7. Drag and drop your `publish.zip` file into the file area
8. Wait for the deployment to complete

## Step 8: Test the Application

1. Navigate back to your Web App
2. Click on "Overview"
3. Click on the URL to open your application
4. Add "/swagger" to the URL to access the Swagger UI
5. Test the API endpoints

## Troubleshooting

If you encounter any issues:

1. Check the application logs in the Web App by going to "Log stream" in the left navigation
2. Verify the connection string is correct
3. Make sure the PostgreSQL server allows connections from Azure services
4. Check that the application was deployed correctly

## Monitoring

1. Set up Application Insights for monitoring by going to "Application Insights" in the left navigation of your Web App
2. Enable it and configure as needed

## Scaling Considerations

Note that the Free tier has limitations:
- 60 minutes of compute per day
- 1GB of memory
- No custom domains
- No scaling capabilities

For production use, consider upgrading to a Basic or Standard tier. 