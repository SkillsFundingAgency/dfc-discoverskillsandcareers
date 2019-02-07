[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$StorageAccountName,
    [Parameter(Mandatory = $true)]
    [string]$StorageAccountKey,
    [Parameter(Mandatory = $true)]
    [string[]]$AllowedOrigins
)

Write-Verbose -Message "Setting Storage Context"
$StorageContext = New-AzureStorageContext -StorageAccountName $StorageAccountName -StorageAccountKey $StorageAccountName

# ---- CORS Settings
foreach ($AllowedOrigin in $AllowedOrigins) {

    $CORSRules = (@{
        AllowedHeaders  = @("*");
        AllowedOrigins  = @("$AllowedOrigin");
        MaxAgeInSeconds = 3600;
        AllowedMethods  = @("Get")
    })
    
    try {
        
        # ---- Set CORS Rules
        Write-Verbose -Message "Setting CORS rule on $($StorageContext.BlobEndPoint)"
        Set-AzureStorageCORSRule -ServiceType Blob -CorsRules $CORSRules -Context $StorageContext
    
    }
    catch {
    
        throw "Failed to get Storage Context and set CORS settings: $_"
    
    }

}



