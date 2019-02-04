[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$KeyVaultName,
    [Parameter(Mandatory=$true)]
    [string]$KeyVaultResourceGroup
)

$CdnServicePrincipal = Get-AzureRmADServicePrincipal -DisplayName Microsoft.Azure.Cdn
if (!$CdnServicePrincipal) {

    Write-Verbose -Message "Service Principle not registered for Microsoft.Azure.Cdn, registering ..."
    $CdnServicePrincipal = New-AzureRmADServicePrincipal -ApplicationId "205478c0-bd83-4e1b-a9d6-db63a3e1e1c8"

}

$KeyVault = Get-AzureRmKeyVault -VaultName $KeyVaultName
if ($KeyVault) {

    $CdnSpnKeyVaultAccessPolicy = $KeyVault.AccessPolicies | Where-Object { $_.ObjectId -eq $CdnServicePrincipal.Id }
    if (!$CdnSpnKeyVaultAccessPolicy) {

        Write-Verbose -Message "Setting KeyVault access policy for CDN Service Principal"
        Set-AzureRmKeyVaultAccessPolicy -VaultName  $KeyVaultName -ResourceGroupName $KeyVaultResourceGroup -ServicePrincipalName "205478c0-bd83-4e1b-a9d6-db63a3e1e1c8" -PermissionsToSecrets get

    }
    else {

        Write-Host "CDN Service Principal Access Policy:"
        $CdnSpnKeyVaultAccessPolicy
    }
    
}
else {

    throw "KeyVault: $KeyVaultName doesn't exist"

}