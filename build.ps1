$working_dir = Get-Location
$output = "$working_dir\output"
$api_output = "$output\api"
$app_output = "$output\app"

# Clean output directory
if(Test-Path $output) {
    Remove-Item -Recurse -Force $output
}

# Build api
dotnet publish .\src\Api\Vector.Share.sln --verbosity minimal --output $api_output --configuration 'Release' --no-self-contained --runtime linux-x64 -p:PublishSingleFile=true
dotnet-ef database update --project .\src\Api\Vector.Share\Vector.Share.csproj --connection "Data Source=$api_output\files.db"
cp .\src\Api\Vector.Share\web.config $api_output\web.config

# Build vuejs app
$cwd = ".\src\App"
yarn --cwd $cwd install
yarn --cwd $cwd build --dest $app_output