[CmdletBinding()]
param(
    [switch]$DeployToOwnTenant
)

$Location = "West Europe"

$DeploymentParameters = @{
    TemplateParameterFile = ".\Resources\ArmTemplates\test-parameters.json"
    TemplateFile          = ".\Resources\ArmTemplates\template.json"
}

if($DeployToOwnTenant.IsPresent) {

    $IsLoggedIn = (Get-AzureRMContext -ErrorAction SilentlyContinue).Account
    if (!$IsLoggedIn) {
        Login-AzureRmAccount
    }
    elseif ($($IsLoggedIn.Id.split("@")[1] -eq "citizenazuresfabisgov.onmicrosoft.com") -or $($IsLoggedIn.Id.split("@")[1] -eq "fcsazuresfabisgov.onmicrosoft.com")) {
        throw "Logged in to SFA tenant.  Login to your personal tenant."
    }
    
    $TemplateParamsObject = Get-Content $DeploymentParameters['TemplateParameterFile'] | ConvertFrom-Json

    $ResourceGroupName = $TemplateParamsObject.parameters.ApimResourceGroup.value
    $ResourceGroup = Get-AzureRmResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
    if (!$ResourceGroup) {
        Write-Host "- Creating resource group $ResourceGroupName"
        New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location
    }

    $ResourceGroupName = "dfc-my-skillscareers-rg"
    $ResourceGroup = Get-AzureRmResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
    if (!$ResourceGroup) {
        Write-Host "- Creating resource group $ResourceGroupName"
        New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location
    }
    
    $DeploymentParameters['ResourceGroup'] = $ResourceGroupName
    $DeploymentParameters['DeploymentDebugLogLevel'] = "All"
    
    Write-Host "- Deploying template"
    New-AzureRmResourceGroupDeployment @DeploymentParameters

} 
else {

    $ResourceGroupName = "dfc-test-template-rg"

    $DeploymentParameters['ResourceGroup'] = $ResourceGroupName
    $DeploymentParameters['Verbose'] = $true

    Write-Host "- Validating template"
    Test-AzureRmResourceGroupDeployment @DeploymentParameters
    
}

