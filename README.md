## Overview
- Fontend: Vue3
- Backend: NET 5.0 API
- Database: SQLite3

## Building instructions
1. Edit the following files for the correct settings:
    - `src/App/.env.development`
    - `src/App/.env.production`
    - `src/Api/Vector.Share/appsettings.json`
    - `src/Api/Vector.Share/appsettings.Development.json`

2. In `src/App` run: `yarn install`

3. Run `build.ps1`, optionally with the following parameters:
   - `-Output` to specify the output directory, by default this is: `./output`
   - `-Runtime` to specify the NET 5.0 runtime, by default this is: `linux-x64`

4. Deploy the files in the output directory to your server and set up your webserver and service file. Example files for nginx and a systemd service file are provided in the examples folder.

## Requirements
1. Yarn
2. NET 5.0 SDK
3. dotnet-ef