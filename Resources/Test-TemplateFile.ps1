$IsLoggedIn = (Get-AzureRMContext -ErrorAction SilentlyContinue).Account
if (!$IsLoggedIn) {
    Login-AzureRmAccount
}
elseif ($($IsLoggedIn.Id.split("@")[1] -eq "citizenazuresfabisgov.onmicrosoft.com") -or $($IsLoggedIn.Id.split("@")[1] -eq "fcsazuresfabisgov.onmicrosoft.com")) {
    throw "Logged in to SFA tenant.  Login to your personal tenant."
}

$ResourceGroupName = "dfc-my-skillscareers-rg"
$Location = "West Europe"
$ResourceGroup = Get-AzureRmResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
if (!$ResourceGroup) {
    Write-Host "- Creating resource group"
    New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location
}

$DeploymentParameters = @{
    ResourceGroup         = $ResourceGroupName
    TemplateParameterFile = ".\Resources\test-parameters.json"
    TemplateFile          = ".\Resources\template.json"
}

Write-Host "- Deploying template"
New-AzureRmResourceGroupDeployment @DeploymentParameters