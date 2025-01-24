using Grpc.Core;
using grpcMessageServer;

namespace grpcServer.Services;

public class MessageService : Message.MessageBase
{
    private readonly ILogger<MessageService> _logger;
    public MessageService(ILogger<MessageService> logger)
    {
        _logger = logger;
    }
    
    
    public override Task<MessageReply> sendMessage(MessageRequest request, ServerCallContext context)
    {
        System.Console.WriteLine($"Message :  {request.Message}| Name : {request.Name}");
        return Task.FromResult(new MessageReply
        {
            Message = "Message Received"
        });
    }
}