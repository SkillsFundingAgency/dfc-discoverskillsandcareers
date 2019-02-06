[CmdletBinding()]
param(
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

Write-Verbose -Message "Setting Azure DevOps variable CdnServicePrincipalName to $($CdnServicePrincipal.ServicePrincipalNames[1])"
Write-Host "##vso[task.setvariable variable=CdnServicePrincipalName]$($CdnServicePrincipal.ServicePrincipalNames[1])"
Write-Verbose -Message "Setting Azure DevOps variable CdnServicePrincipalObjectId to $($CdnServicePrincipal.ObjectId)"
Write-Host "##vso[task.setvariable variable=CdnServicePrincipalObjectId]$($CdnServicePrincipal.ObjectId)"

Write-Verbose -Message "Disconnecting from AAD"
Disconnect-AzureAD