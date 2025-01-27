// See https://aka.ms/new-console-template for more information

using Google.Protobuf;
using Grpc.Net.Client;
using grpcFileTransportClient;

//using grpcMessageClient;

var channel = GrpcChannel.ForAddress("http://localhost:5247/");

////greet.proto
// var greeterClient = new Greeter.GreeterClient(channel);
// HelloReply helloReply = await greeterClient.SayHelloAsync(new HelloRequest{ Name = "SelamünAleyküm" });
// Console.WriteLine(helloReply.Message);

//message.proto
//var messageClient = new Message.MessageClient(channel);

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
// var request = messageClient.SendMessage();
// var task1 = Task.Run(async () =>
// {
//     for (int i = 0; i < 10; i++)
//     {
//         await Task.Delay(1000);
//         await request.RequestStream.WriteAsync(new MessageRequest
//         {
//             Name = "SelamünAleyküm",
//             Message = "Message" + i
//         });
//     }
// });
// while (await request.ResponseStream.MoveNext(cts.Token))
// {
//     System.Console.WriteLine(request.ResponseStream.Current.Message);
// }
//
// await task1;
// await request.RequestStream.CompleteAsync();

//File upload
var client = new FileService.FileServiceClient(channel);
string file = "";
using FileStream fileStreamUpload = new FileStream(file, FileMode.Open);
var content = new BytesContent()
{
    FileSize = fileStreamUpload.Length,
    ReadBytes = 0,
    FileInfo = new grpcFileTransportClient.FileInfo() { FileName = Path.GetFileNameWithoutExtension(fileStreamUpload.Name), FileExtension = Path.GetExtension(fileStreamUpload.Name) }
};
var upload = client.FileUpload();
byte [] bufferUpload = new byte[2048];

while((content.ReadBytes = await fileStreamUpload.ReadAsync(bufferUpload, 0, bufferUpload.Length)) > 0)
{
    content.Buffer = ByteString.CopyFrom(bufferUpload);
    await upload.RequestStream.WriteAsync(content);
}
await upload.RequestStream.CompleteAsync();
fileStreamUpload.Close();

//File download

string downloadPath = @"C:\\Users\\alper\\Desktop\\gençay\\gRPC\\grpcClient";

var fileInfo = new grpcFileTransportClient.FileInfo()
{
    FileName = "test",
    FileExtension = ".mp4"
};

FileStream fileStreamDownload = null;

var request = client.FileDownload(fileInfo);

int count = 0;
decimal chunkSize = 0;
while(await request.ResponseStream.MoveNext(cts.Token))
{
    if (count++ == 0)
    {
        fileStreamDownload = new FileStream($@"{downloadPath}\{request.ResponseStream.Current.FileInfo.FileName}{request.ResponseStream.Current.FileInfo.FileExtension}", FileMode.CreateNew);
        fileStreamDownload.SetLength(request.ResponseStream.Current.FileSize);

        var bufferDownload = request.ResponseStream.Current.Buffer.ToByteArray();
        await fileStreamDownload.WriteAsync(bufferDownload, 0, request.ResponseStream.Current.ReadBytes);
        System.Console.WriteLine($"{Math.Round(((chunkSize += request.ResponseStream.Current.ReadBytes)*100)/request.ResponseStream.Current.FileSize)}");
        Console.WriteLine("Downloaded...");
        await fileStreamDownload.DisposeAsync();
        fileStreamDownload.Close();
    }
}