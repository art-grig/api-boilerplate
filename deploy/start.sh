#!/bin/bash

cat ./appsettings.json \
    | jq ".ApplicationInsights.InstrumentationKey=\"$SU_APPINSIGHTS_IKEY\"" \
    | jq ".ApplicationInsights.Environment=\"$SU_APPINSIGHTS_ENV\"" \
    > ./appsettings.json.tmp

cp ./appsettings.json.tmp ./appsettings.json
rm ./appsettings.json.tmp
cat ./appsettings.json

dotnet WI.ApiBoilerplate.dll
