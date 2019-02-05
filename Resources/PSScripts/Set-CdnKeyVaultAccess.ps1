[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$KeyVaultName,
    [Parameter(Mandatory=$true)]
    [string]$KeyVaultResourceGroup,
    [Parameter(Mandatory=$false)]
    [string]$CdnServicePrincipalName = '$CdnServicePrincipalName',
    [Parameter(Mandatory=$false)]
    [string]$CdnServicePrincipalObjectId = '$CdnServicePrincipalObjectId'
)

$KeyVault = Get-AzureRmKeyVault -VaultName $KeyVaultName
if ($KeyVault) {

    $CdnSpnKeyVaultAccessPolicy = $KeyVault.AccessPolicies | Where-Object { $_.ObjectId -eq $CdnServicePrincipalObjectId }
    if (!$CdnSpnKeyVaultAccessPolicy) {

        Write-Verbose -Message "Setting KeyVault access policy for CDN Service Principal: $($CdnServicePrincipalName)"
        Set-AzureRmKeyVaultAccessPolicy -VaultName  $KeyVaultName -ResourceGroupName $KeyVaultResourceGroup -ServicePrincipalName $CdnServicePrincipalName -PermissionsToSecrets get -Verbose:$VerbosePreference

    }
    else {

        Write-Host "CDN Service Principal Access Policy:"
        $CdnSpnKeyVaultAccessPolicy

    }
    
}
else {

    throw "KeyVault: $KeyVaultName doesn't exist"

}