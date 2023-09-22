using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

var cts = new CancellationTokenSource();
var sqsClient = new AmazonSQSClient();

var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var receiveMessageRequest = new ReceiveMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    AttributeNames = new List<string> { "All" },
    MessageAttributeNames = new List<string> { "All" }
};

while (!cts.IsCancellationRequested)
{
    var response = await sqsClient.ReceiveMessageAsync(receiveMessageRequest, cts.Token);

    foreach (var message in response.Messages)
    {
        Console.WriteLine($"Message ID: {message.MessageId}");
        Console.WriteLine($"Message Body: {message.Body}");
        Console.WriteLine($"Attributes: {JsonSerializer.Serialize(message.MessageAttributes)}");

        await sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle);
    }
    await Task.Delay(3000);
}
