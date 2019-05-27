using System;
using System.Net;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.Lambda.APIGatewayEvents;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json; 
using Newtonsoft.Json.Serialization;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace MessagingApi
{
    public class Handler
    {
       public async Task<APIGatewayProxyResponse> SendMessage(APIGatewayProxyRequest request, ILambdaContext context)
       {
            // Uncomment for debugging: Console.WriteLine(request.Body);
            JsonSerializerSettings serSettings = new JsonSerializerSettings();
            serSettings.ContractResolver = new DefaultContractResolver();
            AddMetricsRequest metricsRequest = JsonConvert.DeserializeObject<AddMetricsRequest>(request.Body, serSettings);

            Task<int> createItemTask = CreateItem(metricsRequest);
            int result = await createItemTask;

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (result == 0) ? (int)HttpStatusCode.OK : (int)HttpStatusCode.InternalServerError,
                Body =  $"{{\"Success\": {result} }}",
                Headers = new Dictionary<string, string> {{ "Content-Type", "application/json" }}
            };

            return response;
       }

       private async Task<int> CreateItem(AddMetricsRequest metrics)
       {
            try 
            {
                var putItemData = CreatePutItemData(metrics);
                AmazonDynamoDBClient client = new AmazonDynamoDBClient();
                Task<PutItemResponse> putTask = client.PutItemAsync("MessagingAppMessages", putItemData);
                var response = await putTask;

                // return 0 for success, otherwise failure of -1
                return response.HttpStatusCode == HttpStatusCode.OK ? 0 : -1;
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return 0;
        }
    
        private Dictionary<string, AttributeValue> CreatePutItemData(AddMetricsRequest metrics) 
        {
            var items = new Dictionary<string, AttributeValue>()
            {
                { "groupId", new AttributeValue {
                      S = metrics.GroupId
                  }},
                { "message", new AttributeValue {
                      S = metrics.Message
                  }},
                { "messageDateTime", new AttributeValue {
                      S = metrics.MessageDateTime
                  }}
            };

            return items;
        }
    }

    public class AddMetricsRequest
    {
      public string GroupId { get; set; }
      public string Message {get; set;}
      public string MessageDateTime {get; set;}

      public AddMetricsRequest(
        string groupId,
        string message, 
        string messageDateTime) 
      {
          GroupId = groupId;
          Message = message;
          MessageDateTime = messageDateTime;
      }
    }
}
