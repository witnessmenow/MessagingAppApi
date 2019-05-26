echo "***** SPF: running build script *****"

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "***** SPF: running in $DIR *****"

# Build the .net core 2 put-message function
# NOTE - the getmessage  service does not require any pre-compilation/build (python)
cd $DIR/putmessage
dotnet add package AWSSDK.DynamoDBv2 --version 3.3.6
dotnet add package Amazon.Lambda.APIGatewayEvents
./build-macos.sh

echo "***** SPF: finished build stage *****"

echo "***** SPF: running sls deploy stage *****"

cd $DIR
serverless deploy -v

echo "***** SPF: finished sls deploy stage *****"