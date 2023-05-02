param storageName string
param containerNames array = [ 'con','models' ]
param location string = resourceGroup().location
param functionAppName string
param storageName string
param sqlServerName string
param sqlServerAdminLogin string
@secure()
param sqlServerAdminLoginPassword string
param dbName string
param webPubSubName string
param aksName string

resource appStorage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Cool'
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }
}

resource sqlServer 'Microsoft.Sql/servers@2022-02-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlServerAdminLogin
    administratorLoginPassword: sqlServerAdminLoginPassword
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-02-01-preview' = {
  name: dbName
  location: location
  sku: {
    capacity: 5
    name: 'Free'
    tier: 'Free'
  }
  parent: sqlServer
}

resource imageContainers 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-09-01' = [for name in containerNames: {
  name: '${appStorage.name}/default/${name}'
  properties: {
    publicAccess: 'Blob'
  }
}]

resource queue 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-09-01' = {
  name: '${appStorage.name}/default/altitude-position'
}
resource queue2 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-09-01' = {
  name: '${appStorage.name}/default/ecam-position'
}

resource fileshare 'Microsoft.Storage/storageAccounts/fileServices/shares@2022-05-01' = {
  name: '${appStorage.name}/default/dependencies'
}

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${appStorage.listKeys().keys[0].value}'
        }
        {
          name: 'SqlConnectionString'
          value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${dbName};Persist Security Info=False;User ID=${sqlServerAdminLogin};Password=${sqlServerAdminLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
        }
        {
          name: 'AzureWebPubSub'
          value: 'Endpoint=https://triplejbpubsub.webpubsub.azure.com;AccessKey=D4Pa2W274942Lwx+Q79RdJWP/rUzodEfPWCh30MZz7M=;Version=1.0;'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'StorageContainer'
          value: 'con'
        }
      ]
      minTlsVersion: '1.2'
    }
  }
}
