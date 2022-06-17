using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NettyServer
{
    public class DotNettyClient
    {
        private IChannel Channel { set; get; }
        private IEventLoopGroup Group { set; get; }

        private ClientMessageHandler ClientMessageHandler = new ClientMessageHandler();
        private string Host;
        private int Port;
        public DotNettyClient(string host, int port)
        {
            Host = host;
            Port = port;
        }
        public async void Start()
        {
            Group = new MultithreadEventLoopGroup();
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(Group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;

                    pipeline.AddLast(new LengthFieldPrepender(4));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    //pipeline.AddLast(new IdleStateHandler(15, 0, 0));
                    pipeline.AddLast(ClientMessageHandler);
                }));
            Channel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Host), Port));

        }
        public TransportMessage Send(string message)
        {
            if (message == null)
            {
                return null;
            }
            var buffer = Unpooled.Buffer(256);
            var transportMessage = new TransportMessage
            {
                Id = Guid.NewGuid().ToString("N"),
                Message = message,
                TransoprtType = TransoprtType.Request
            };
            var callbackTask = ClientMessageHandler.RegisterResultCallbackAsync(transportMessage.Id);

            if (!Channel.Active)
            {
                throw new Exception("无法连接到服务");
            }

            buffer.WriteBytes(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(transportMessage)));
            Channel.WriteAndFlushAsync(buffer);

            return callbackTask.Result;
        }

        public async void Close()
        {
            await Channel.CloseAsync();
            await Group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }
    }
    public class ClientMessageHandler : ChannelHandlerAdapter
    {
        public readonly ConcurrentDictionary<string, TaskCompletionSource<TransportMessage>> _resultDictionary =
         new ConcurrentDictionary<string, TaskCompletionSource<TransportMessage>>();
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var byteBuffer = message as IByteBuffer;

            if (byteBuffer != null)
            {
                var _message = JsonConvert.DeserializeObject<TransportMessage>(byteBuffer.ToString(Encoding.UTF8));
                if (_message.TransoprtType == TransoprtType.Response)
                {
                    if (_resultDictionary.TryGetValue(_message.Id, out TaskCompletionSource<TransportMessage> task))
                    {
                        task.SetResult(_message);
                    }
                }
            }
        }
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            context.CloseAsync();
        }
        public Task<TransportMessage> RegisterResultCallbackAsync(string id)
        {
            var task = new TaskCompletionSource<TransportMessage>();
            _resultDictionary.TryAdd(id, task);
            return task.Task;
        }
    }
}
