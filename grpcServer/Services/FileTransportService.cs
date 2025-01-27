using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using grpcFileTransportServer;
using FileInfo = grpcFileTransportServer.FileInfo;

namespace grpcServer.Services;

public class FileTransportService : FileService.FileServiceBase
{
    private readonly IWebHostEnvironment _env;
    public FileTransportService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public override async Task<Empty> FileUpload(
        IAsyncStreamReader<BytesContent> requestStream,
        ServerCallContext context
    )
    {
        string path = Path.Combine(_env.WebRootPath, "files");
        if(Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }
        
        int count = 0;
        FileStream fileStream = null;
        try
        {
            decimal chunkSize = 0;
            while (await requestStream.MoveNext() )
            {
                if(count++ == 0)
                {
                    fileStream = new FileStream($"{path}/{requestStream.Current.FileInfo.FileName}{requestStream.Current.FileInfo.FileExtension}", FileMode.CreateNew);
                    fileStream.SetLength(requestStream.Current.FileSize);
                }
                var buffer = requestStream.Current.Buffer.ToByteArray();
                await fileStream.WriteAsync(buffer, 0, buffer.Length);
                
                System.Console.WriteLine($"{Math.Round(((chunkSize += requestStream.Current.ReadBytes)*100)/requestStream.Current.FileSize)}");
            }
        }
        catch
        {
        }

        await fileStream.DisposeAsync();
        fileStream.Close();
        return new Empty();
    }

    public override async Task FileDownload(
        FileInfo request,
        IServerStreamWriter<BytesContent> responseStream,
        ServerCallContext context
    )
    {
        string path = Path.Combine(_env.WebRootPath, "files");
        using FileStream fileStream = new FileStream($"{path}/{request.FileName}{request.FileExtension}", FileMode.Open, FileAccess.Read);
        byte[] buffer = new byte[2048];
        BytesContent content = new BytesContent()
        {
            FileSize = fileStream.Length,
            FileInfo = new FileInfo(){FileName = Path.GetFileNameWithoutExtension(fileStream.Name), FileExtension = Path.GetExtension(fileStream.Name)},
            ReadBytes = 0
        };
        while ((content.ReadBytes = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            content.Buffer = ByteString.CopyFrom(buffer);
            await responseStream.WriteAsync(content);
        }
        fileStream.Close();
    }
}