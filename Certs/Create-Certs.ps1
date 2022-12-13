[string]$Password = [Guid]::NewGuid().ToString("N")
Set-Content -Path "${PSScriptRoot}\Password.txt" -Value $Password -NoNewline

docker run --rm --entrypoint="/bin/bash" -v "${PSScriptRoot}:/Certs" -w="/Certs" mcr.microsoft.com/dotnet/aspnet:3.1 "/Certs/CreateCerts.sh"

Copy-Item -Path "${PSScriptRoot}\test-ca.crt" -Destination "${PSScriptRoot}\..\NutritionWebClient\test-ca.crt" -Force
Copy-Item -Path "${PSScriptRoot}\test-ca.crt" -Destination "${PSScriptRoot}\..\APIGateway\test-ca.crt" -Force
Copy-Item -Path "${PSScriptRoot}\test-ca.crt" -Destination "${PSScriptRoot}\..\UserLogin\test-ca.crt" -Force
Copy-Item -Path "${PSScriptRoot}\test-ca.crt" -Destination "${PSScriptRoot}\..\JWTService\test-ca.crt" -Force
Copy-Item -Path "${PSScriptRoot}\test-ca.crt" -Destination "${PSScriptRoot}\..\UserRegister\test-ca.crt" -Force
Copy-Item -Path "${PSScriptRoot}\test-ca.crt" -Destination "${PSScriptRoot}\..\ProductsCatalog\test-ca.crt" -Force
Copy-Item -Path "${PSScriptRoot}\test-ca.crt" -Destination "${PSScriptRoot}\..\PredefinedMeals\test-ca.crt" -Force
Copy-Item -Path "${PSScriptRoot}\test-ca.crt" -Destination "${PSScriptRoot}\..\MyDayService\test-ca.crt" -Force
Copy-Item -Path "${PSScriptRoot}\test-ca.crt" -Destination "${PSScriptRoot}\..\LogService\test-ca.crt" -Force
Copy-Item -Path "${PSScriptRoot}\test-ca.crt" -Destination "${PSScriptRoot}\..\UserRegister\test-ca.crt" -Force

Copy-Item -Path "${PSScriptRoot}\NutritionWebClient.pfx" -Destination "${PSScriptRoot}\..\NutritionWebClient\NutritionWebClient.pfx" -Force

New-Item -Path "${PSScriptRoot}\..\" -Name ".env" -ItemType "file" -Value "Kestrel__Certificates__Default__Password=${Password}`n" -Force

Add-Content "${PSScriptRoot}\..\.env" "APIGateway_Cert=/Certs/APIGateway.pfx"
Add-Content "${PSScriptRoot}\..\.env" "NutritionWebClient_Cert=/Certs/NutritionWebClient.pfx"
Add-Content "${PSScriptRoot}\..\.env" "UserLogin_Cert=/Certs/UserManagement.pfx"
Add-Content "${PSScriptRoot}\..\.env" "Jwt_Cert=/Certs/JWTService.pfx"
Add-Content "${PSScriptRoot}\..\.env" "Products_Cert=/Certs/ProductsCatalog.pfx"
Add-Content "${PSScriptRoot}\..\.env" "Meals_Cert=/Certs/PredefinedMeals.pfx"
Add-Content "${PSScriptRoot}\..\.env" "MyDay_Cert=/Certs/MyDayService.pfx"
Add-Content "${PSScriptRoot}\..\.env" "Logs_Cert=/Certs/LogsService.pfx"
Add-Content "${PSScriptRoot}\..\.env" "Register_Cert=/Certs/UserRegister.pfx"

$testCaCert = New-Object -TypeName "System.Security.Cryptography.X509Certificates.X509Certificate2" @("${PSScriptRoot}\test-ca.crt", $null)

$storeName = [System.Security.Cryptography.X509Certificates.StoreName]::Root;
$storeLocation = [System.Security.Cryptography.X509Certificates.StoreLocation]::CurrentUser
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store($storeName, $storeLocation)
$store.Open(([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite))
try
{
    $store.Add($testCaCert)
}
finally
{
    $store.Close()
    $store.Dispose()
}
