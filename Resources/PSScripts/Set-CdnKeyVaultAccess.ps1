[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$KeyVaultName,
    [Parameter(Mandatory=$true)]
    [string]$KeyVaultResourceGroup,
    [Parameter(Mandatory=$true)]
    [string]$TenantId,
    [Parameter(Mandatory=$true)]
    [string]$EndpointClientId,
    [Parameter(Mandatory=$true)]
    [string]$EndpointClientSecret
)

if (!(Get-Module AzureAd)) {

    if (!(Get-InstalledModule AzureAd -ErrorAction SilentlyContinue)) {

        Install-Module AzureAd -Scope CurrentUser -Force

    }
    Import-Module AzureAd

}

try {

    if(!($ENV:TF_BUILD)) {

        Connect-AzureAD -TenantId $TenantId
    
    }
    else {
    
        $AADTokenUrl = "https://login.microsoftonline.com/$TenantId/oauth2/token"
        
        $Body = @{
            grant_type    = "client_credentials"
            client_id     = $EndpointClientId
            client_secret = $EndpointClientSecret
            resource      = "https://graph.windows.net/"
        }
        
        $Response = Invoke-RestMethod -Method POST -Uri $AADTokenUrl -ContentType "application/x-www-form-urlencoded" -Body $Body
        $Token = $Response.access_token
        
        Write-Verbose "Login to AzureAD with same application as endpoint"
        $null = Connect-AzureAD -AadAccessToken $Token -AccountId $EndpointClientId -TenantId $TenantId
    
    }

}
catch {

    throw "ERROR: unable to login to Active Directory tenant $TenantId with app registration ApplicationId $EndpointClientId`n$($Error[0])"

}

$CdnServicePrincipal = Get-AzureADServicePrincipal -SearchString Microsoft.Azure.Cdn
if (!$CdnServicePrincipal) {

    Write-Verbose -Message "Service Principle not registered for Microsoft.Azure.Cdn, registering ..."
    $CdnServicePrincipal = New-AzureADServicePrincipal -AppId "205478c0-bd83-4e1b-a9d6-db63a3e1e1c8" -AccountEnabled $true -Tags {WindowsAzureActiveDirectoryIntegratedApp} -Verbose:$VerbosePreference

}

Write-Verbose -Message "Disconnecting from AAD"
Disconnect-AzureAD

$KeyVault = Get-AzureRmKeyVault -VaultName $KeyVaultName
if ($KeyVault) {

    $CdnSpnKeyVaultAccessPolicy = $KeyVault.AccessPolicies | Where-Object { $_.ObjectId -eq $CdnServicePrincipal.ObjectId }
    if (!$CdnSpnKeyVaultAccessPolicy) {

        Write-Verbose -Message "Setting KeyVault access policy for CDN Service Principal: $($CdnServicePrincipal.ServicePrincipalNames[1])"
        Set-AzureRmKeyVaultAccessPolicy -VaultName  $KeyVaultName -ResourceGroupName $KeyVaultResourceGroup -ServicePrincipalName $CdnServicePrincipal.ServicePrincipalNames[1] -PermissionsToSecrets get -Verbose:$VerbosePreference

    }
    else {

        Write-Host "CDN Service Principal Access Policy:"
        $CdnSpnKeyVaultAccessPolicy

    }
    
}
else {

    throw "KeyVault: $KeyVaultName doesn't exist"

}