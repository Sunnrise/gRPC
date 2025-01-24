﻿// See https://aka.ms/new-console-template for more information

using System.Threading.Channels;
using Grpc.Net.Client;
using grpcMessageClient;
using grpcServer;

Console.WriteLine("Hello, World!");

var channel = GrpcChannel.ForAddress("http://localhost:5247/");

////greet.proto
// var greeterClient = new Greeter.GreeterClient(channel);
// HelloReply helloReply = await greeterClient.SayHelloAsync(new HelloRequest{ Name = "SelamünAleyküm" });
// Console.WriteLine(helloReply.Message);

//message.proto
var messageClient = new Message.MessageClient(channel);
MessageReply messageReply =await messageClient.sendMessageAsync(new MessageRequest{ Name = "SelamünAleyküm", Message = "Nasılsınız?" });
Console.WriteLine(messageReply.Message);