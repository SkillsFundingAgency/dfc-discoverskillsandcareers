function Get-AzureApiBearerToken {
<#   
    .SYNOPSIS
    Requests a bearer token from AAD for use with the Azure REST API https://docs.microsoft.com/en-us/rest/api/azure/

    .EXAMPLE

    .NOTES
    https://docs.microsoft.com/en-us/rest/api/azure/#create-the-request
#>    
    [CmdletBinding()]
    param (
        #Required.  The tenant id for the AAD tenant supplying the bearer token.
        [Parameter(Mandatory=$true)]
        [Guid]$TenantId,

        #Required.  The ApplicationId of the AAD registered application that will be requesting the bearer token.
        [Parameter(Mandatory=$true)]
        [Guid]$ApplicationId,

        #Required.  They key for the AAD registered application application that will be requesting the bearer token.
        [Parameter(Mandatory=$true)]
        [System.Security.SecureString]$AppRegistrationKey
    )
  
    Write-Host "Logging preference: $VerbosePreference"
  
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($AppRegistrationKey)
    $UnsecurePassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
  
    $Body = @{
      'resource'= "https://management.core.windows.net/"
      'client_id' = $ApplicationId
      'grant_type' = 'client_credentials'
      'client_secret' = $UnsecurePassword
    }
    Write-Debug $Body
  
    $TokenEndpoint = {https://login.windows.net/{0}/oauth2/token} -f $TenantId
    Write-Verbose $TokenEndpoint
  
    $Params = @{
        ContentType = 'application/x-www-form-urlencoded'
        Headers = @{'accept'='application/json'}
        Body = $Body
        Method = 'Post'
        URI = $TokenEndpoint
        UseBasicParsing = $true
    }
    Write-Debug $Params
  
    $Token = Invoke-RestMethod @Params
    # Show the raw token and expiration date converted in readable format: #
    $Token
  }

function Get-AzureRmStorageContainerSasToken {
<#
    .SYNOPSIS
    Creates a SAS token with read permissions and default duration for an Azure Storage container

    .EXAMPLE
    Get-AzureRmStorageContainerSasToken -ResourceGroup dfc-my-shared-rg -StorageAccountName dfcmysharedstr -ContainerName mycontainer
#>
    [CmdletBinding()]
    param(
        #Required.  Resource group containing the storage account
        [Parameter(Mandatory=$true)]
        [string]$ResourceGroupName,
        [Parameter(Mandatory=$true)]
        #Required.  Storage account name
        [string]$StorageAccountName,
        #Required.  Container name
        [Parameter(Mandatory=$true)]
        [string]$ContainerName,
        #Optional.  If the SAS token is going to be consumed in a SQL script to create a Data Source the leading question mark will need to be removed.  Add this switch to do that.
        [Parameter(Mandatory=$false)]
        [switch]$RemoveLeadingQuestionMark
    )

    $Key = Get-AzureRmStorageAccountKey -ResourceGroupName $($ResourceGroupName.ToLower()) -Name $($StorageAccountName.ToLower())
    $Context = New-AzureStorageContext -StorageAccountName $($StorageAccountName.ToLower()) -StorageAccountKey $Key[0].Value
    $SasToken = New-AzureStorageContainerSASToken -Name $($ContainerName.ToLower()) -Permission r -Context $Context

    if ($RemoveLeadingQuestionMark.IsPresent) {
        $SasToken.Substring(1)
    }
    else {

        $SasToken

    }

}

function New-Password {
    <#

    .SYNOPSIS
    Generate a random password

    .DESCRIPTION
    Generate a random password using System.Web.Security.Membership::GeneratePassword()

    .PARAMETER Length
    The number of characters in the generated password. The length must be between 1 and 128 characters.

    .PARAMETER NumberOfNonAlphaNumericCharacters
    The minimum number of non-alphanumeric characters (such as @, #, !, %, &, and so on) in the generated password.

    .EXAMPLE
    New-Password

    .EXAMPLE
    New-Password -Length 20 -NumberOfNonAlphaNumericCharacters 2

    .NOTES

    Suppressed Script Analyzer rules:
        - PSAvoidUsingConvertToSecureStringWithPlainText - This method is used as the property is passed as a parameter to another function
        - PSUseShouldProcessForStateChangingFunctions - The function does not alter the state of an object

    #>
    [Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSAvoidUsingConvertToSecureStringWithPlainText", "")]
    [Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSUseShouldProcessForStateChangingFunctions", "")]  
    Param(
        [Parameter(Mandatory=$false)]
        [Int]$Length = 15,
        [Parameter(Mandatory=$false)]
        [Int]$NumberOfNonAlphaNumericCharacters = 0
    )

    try {
        # --- https://msdn.microsoft.com/en-us/library/system.web.security.membership.generatepassword.aspx
        $null = [Reflection.Assembly]::LoadWithPartialName("System.Web")
        $Password = [System.Web.Security.Membership]::GeneratePassword($Length,$NumberOfNonAlphaNumericCharacters)
        $Password = $Password -replace ";" ,"S"
        [PSCustomObject]@{
            PasswordAsString = $Password
            PasswordAsSecureString = ($Password | ConvertTo-SecureString -AsPlainText -Force)
        }

    } catch {
        throw "Failed to generate a password: $_"
    }
}

function Write-Log {
    <#
    .SYNOPSIS
    Generic logging wrapper to be used with scripts

    .DESCRIPTION
    Generic logging wrapper to be used with scripts

    .PARAMETER LogLevel
    The severity of the log message. Valid options are Information, Warning, Verbose and Error

    .PARAMETER Message
    The message to log

    .EXAMPLE
    Write-Log -LogLevel Information -Message "An informational message"

    .EXAMPLE
    Write-Log -LogLevel Warning -Message "A warning message"

    .EXAMPLE
    Write-Log -LogLevel Verbose -Message "A verbose message"

    .EXAMPLE
    Write-Log -LogLevel Error -Message "An error message"

    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [ValidateSet("Information", "Warning", "Verbose", "Error")]
        [String]$LogLevel,
        [Parameter(Mandatory = $true, Position = 1)]
        [ValidateNotNullOrEmpty()]
        [String]$Message
    )

    if (!$ENV:SUPPRESSLOGGING) {
        # --- Add a timestamp for local execution
        if (!$ENV:TF_BUILD) {
            $TimeStamp = "[{0:MM/dd/yy} {0:HH:mm:ss}]" -f (Get-Date)
            $Message = "$Timestamp $Message"
        }

        Switch ($LogLevel) {
            "Information" {
                Write-Host "$($Message)"
                break
            }
            "Warning" {
                if ($ENV:TF_BUILD) {
                    # --- If we are in vsts use task.logissue
                    Write-Host "##vso[task.logissue type=warning;] $($Message)"
                }
                else {
                    Write-Warning -Message "$($Message)"
                }
                break
            }
            "Verbose" {
                Write-Verbose -Message "$($Message)"
                break
            }
            "Error" {
                Write-Error -Message "$($Message)"
                break
            }
        }
    }
}