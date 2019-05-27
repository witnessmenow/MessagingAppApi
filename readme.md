This is a sample hello-world messaging API using lambda/API Gateway.


**Send Message**
POST example:

curl --header "Content-Type: application/json"   --request POST   --data '{"message":"This is a curl message","messageDateTime":"2930432","groupId":"2"}'   https://<awsurl>.execute-api.us-east-1.amazonaws.com/dev/messages

**Get Message**
GET example:
curl https://cszgaqlz8l.execute-api.us-east-1.amazonaws.com/dev/messages/1