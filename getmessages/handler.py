
import os
import json

from getmessages import decimalencoder
import boto3
from boto3.dynamodb.conditions import Key, Attr
from botocore.exceptions import ClientError, ParamValidationError

dynamodb = boto3.resource('dynamodb', region_name='us-east-1')

def getMessages(event, context):
    table = dynamodb.Table(os.environ['DYNAMODB_TABLE'])

    # fetch messages from the database
    inputGroup = '{}'.format(event['pathParameters']['groupId'])
    print('Group Input: ', inputGroup)
    
    try:
        result = table.query(
            TableName=os.environ['DYNAMODB_TABLE'],
            #IndexName='duration-index',
            KeyConditionExpression=Key('groupId').eq('{}'.format(inputGroup)),
            ProjectionExpression='groupId, #message, messageDateTime',
            ExpressionAttributeNames = { "#message": "message" },
            #ScanIndexForward=True # sort descending
        )
    except ParamValidationError as e:
        print("Parameter validation error: %s" % e)        
    except ClientError as e:
        print("Unexpected error: %s" % e)
    except Exception as e:
        print("Generic error: %s" % e)
        
    returnValue = ""

    try:
        if not result['Items']:
            print ("no records available for %s" % inputGroup)
        else:
            maxItem = result['Items'][0]
            print(maxItem)
            jsonString = json.dumps(maxItem, cls=decimalencoder.DecimalEncoder)
            print(jsonString)
            returnValue = jsonString
    except Exception as e:
        print("Generic error: %s" % e)  
    
    # create a response
    response = {
        "statusCode": 200,
        "body": returnValue
    }

    return response