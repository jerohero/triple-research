param location string = 'westeurope'
param storageAccountName string = 'triplejbstorage'
param sqlServerName string = 'triplejbdbserver'
param sqlDatabaseName string = 'tripjedbdb'
param blobContainerNameModel string = 'trained-model'
param queueNameStreamPollChunk string = 'stream-poll-chunk'
param functionAppName string = 'triplejbfunctions'
param webPubSubName string = 'triplejbpubsub'
param webPubSubConnectionString string
param sqlConnectionString string
param sqlServerAdminLogin string
@secure() param sqlServerAdminLoginPassword string

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: storageAccountName
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
  name: '${sqlServer.name}/${sqlDatabaseName}'
  location: location
  sku: {
    name: 'Free'
    tier: 'Free'
    capacity: 5
  }
  properties: {
    max_size_bytes: 2147483648 // 2GB
    zone_redundant: false
  }
}

resource dbFirewallRules 'Microsoft.Sql/servers/firewallRules@2022-02-01-preview'= {
  parent: sqlServer
  name: 'AllowAllAzureIPs'
  properties:{
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

resource blobContainerModel 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-09-01' = {
  name: '${storageAccount.name}/default/${blobContainerNameModel}'
  properties: {
    publicAccess: 'Blob'
  }
}

resource queueStreamPollChunk 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = {
  name: '${storageAccount.name}/default/${queueNameStreamPollChunk}'
}

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        {
          name: 'AzureWebPubSub'
          value: webPubSubConnectionString
        }
        {
          name: 'SqlConnectionString'
          value: sqlConnectionString
        }
      ]
      linuxFxVersion: 'DOTNET-ISOLATED|7.0'
      minTlsVersion: '1.2'
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource webPubSub 'Microsoft.SignalRService/WebPubSub@2020-10-01' = {
  name: webPubSubName
  location: location
  sku: {
    name: 'Free_F1'
    capacity: 1
  }
}
