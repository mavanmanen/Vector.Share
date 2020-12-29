<#
.PARAMETER Output
Output path
Default: Current directory + /output
.PARAMETER Runtime
Runtime for the NET5.0
Default: linux-x64
Options:
    - portable
    - win-x86
    - win-x64
    - win-arm
    - osx-x64
    - linux-x64
    - linux-arm
.NOTES
Author: Mitchell van Manen
Date:   December 29, 2020
#>
param(
    $Output = $null,
    $Runtime = $null
)

if($Output -eq $null) {
    $working_dir = gl
    $Output = "$working_dir\output"
    echo "No output dir provided, using default: $Output"
}

if($Runtime -eq $null) {
    $Runtime = 'linux-x64'
    echo "No runtime provided, using default: $Runtime"
}

$api_output = "$Output\api"
$app_output = "$Output\app"

# Clean output directory
if(Test-Path $Output) {
    rm -r -Force $Output
}

# Build api
echo "Building NET5.0 api"
$null = dotnet publish .\src\Api\Vector.Share.sln --verbosity quiet --output $api_output --configuration 'Release' --no-self-contained --runtime $Runtime -p:PublishSingleFile=true
$null = dotnet-ef database update --project .\src\Api\Vector.Share\Vector.Share.csproj --connection "Data Source=$api_output\files.db"
$null = cp .\src\Api\Vector.Share\web.config $api_output\web.config

# Build vuejs app
echo "Building Vue3 app"
$cwd = ".\src\App"
$null = yarn --cwd $cwd install
$null = yarn --cwd $cwd build --dest $app_output

echo "Done"