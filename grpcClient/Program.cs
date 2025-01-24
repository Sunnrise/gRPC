// See https://aka.ms/new-console-template for more information

using Grpc.Net.Client;
using grpcMessageClient;

var channel = GrpcChannel.ForAddress("http://localhost:5247/");

////greet.proto
// var greeterClient = new Greeter.GreeterClient(channel);
// HelloReply helloReply = await greeterClient.SayHelloAsync(new HelloRequest{ Name = "SelamünAleyküm" });
// Console.WriteLine(helloReply.Message);

//message.proto
var messageClient = new Message.MessageClient(channel);

////Unary
// MessageReply messageReply =await messageClient.sendMessageAsync(new MessageRequest{ Name = "SelamünAleyküm", Message = "Nasılsınız?" });
// Console.WriteLine(messageReply.Message);

////Server Streaming
// var response = messageClient.SendMessage(new MessageRequest{ Name = "SelamünAleyküm", Message = "Nasılsınız?" });
CancellationTokenSource cts = new CancellationTokenSource();
//
// while (await response.ResponseStream.MoveNext(cts.Token))
// {
//     System.Console.WriteLine(response.ResponseStream.Current.Message);
// }

////Client Streaming
// var request = messageClient.SendMessage();
// for (int i = 0; i < 10; i++)
// {
//     await Task.Delay(1000);
//     await request.RequestStream.WriteAsync(new MessageRequest
//     {
//         Name = "SelamünAleyküm",
//         Message = "Message"+i
//     });
// }
// await request.RequestStream.CompleteAsync();
// System.Console.WriteLine((await request.ResponseAsync).Message);

////Bi-Directional Streaming
var request = messageClient.SendMessage();
var task1 = Task.Run(async () =>
{
    for (int i = 0; i < 10; i++)
    {
        await Task.Delay(1000);
        await request.RequestStream.WriteAsync(new MessageRequest
        {
            Name = "SelamünAleyküm",
            Message = "Message" + i
        });
    }
});
while (await request.ResponseStream.MoveNext(cts.Token))
{
    System.Console.WriteLine(request.ResponseStream.Current.Message);
}

await task1;
await request.RequestStream.CompleteAsync();