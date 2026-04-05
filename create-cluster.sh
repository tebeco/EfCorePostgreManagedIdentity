#!/usr/bin/env bash

subscription="blabla"
resourceGroup="blabla"
location="northeurope"
serverName="efcore-mi"

username="postgres"
password="postgres"

entraAdminDisplayName=$(az ad signed-in-user show --query userPrincipalName -o tsv)
entraAdminObjectId=$(az ad signed-in-user show --query id -o tsv)
entraAdminUserType="User"

ip=$(curl -s https://checkip.amazonaws.com/)

az postgres flexible-server create \
  --subscription $subscription \
  --resource-group $resourceGroup \
  --location $location \
  --name $serverName \
  --public-access $ip \
  --version 18 \
  --sku-name "Standard_B1ms" \
  --tier "Burstable" \
  --storage-size 32 \
  --zonal-resiliency Disabled \
  --password-auth Enabled \
  --admin-user $username \
  --admin-password $password \
  --microsoft-entra-auth Enabled \
  --admin-object-id $entraAdminObjectId \
  --admin-display-name $entraAdminDisplayName \
  --admin-type $entraAdminUserType
