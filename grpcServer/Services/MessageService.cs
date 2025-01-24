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

    public override async Task SendMessage(
        MessageRequest request,
        IServerStreamWriter<MessageReply> responseStream,
        ServerCallContext context
    )
    {
        Console.WriteLine($"Message {request.Message} | Name {request.Name}");

        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(1000);
            await responseStream.WriteAsync(new MessageReply
            {
                Message = $"Hello {request.Name} {i}"
            });
        }
    }
}