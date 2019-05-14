# Introduction 
This is a demo application for running NServiceBus in WebJobs, using Azure ServiceBus as transport, and AspCore as Web client publisher.
This project is based on https://docs.particular.net/tutorials/quickstart/

# Getting Started
Deploy the ARM project to Azure.
Copy the deployment output information as follow:
- azureServiceBusConnString to AzureServiceBusConnString appSettings from App.config (Billing.Webjob.csproj, Shipping.Webjob) and to AzureServiceBusConnString from appsettings.json (Sales.ClientUi.csproj)
- storageConnString to AzureWebJobsDashboard and AzureWebJobsStorage connectionStrings from App.config (Billing.Webjob.csproj, Shipping.Webjob) and to StorageConnString from appsettings.json (Sales.ClientUi.csproj)

# Build and Test
You have all the things in place to start the application

# Contribute
Feel free to contribute to improve and extend this demo.

# References
- [NServiceBus](https://docs.particular.net/tutorials/quickstart/)