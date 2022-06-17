using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NettyServer
{
    public class DotNettyServer
    {
        private IChannel boundChannel;
        private IEventLoopGroup boosGroup;
        private IEventLoopGroup workGroup;
        public async void Listen(int port, Func<string, string> handle)
        {
            boosGroup = new MultithreadEventLoopGroup(1);
            workGroup = new MultithreadEventLoopGroup();

            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(boosGroup, workGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new LengthFieldPrepender(4));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    //pipeline.AddLast(new IdleStateHandler(0, 0, 15));
                    pipeline.AddLast(new ServerMessageHandler(handle));
                }));
            boundChannel = await bootstrap.BindAsync(port);
        }
        public async void Close()
        {
            if (boundChannel != null)
            {
                await boundChannel.CloseAsync();
                await boosGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                await workGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }
    }
    public class ServerMessageHandler : ChannelHandlerAdapter
    {
        Func<string, string> Handle;
        public ServerMessageHandler(Func<string, string> handle)
        {
            this.Handle = handle;
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = message as IByteBuffer;
            if (buffer != null)
            {
                string receiveData = buffer.ToString(Encoding.UTF8);
                Console.WriteLine("服务端获取到:" + receiveData);
                var transportMessage = JsonConvert.DeserializeObject<TransportMessage>(receiveData);

                if (transportMessage.TransoprtType == TransoprtType.Request)
                {
                    string responseData = Handle?.Invoke(transportMessage.Message);
                    transportMessage.TransoprtType = TransoprtType.Response;
                    transportMessage.Message = responseData;
                }
                else
                {
                    transportMessage.TransoprtType = TransoprtType.Response;
                    transportMessage.Message = "不需要处理的";
                }


                var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(transportMessage));
                IByteBuffer byteBuffer = Unpooled.Buffer(256);
                byteBuffer.WriteBytes(messageBytes);
                context.WriteAndFlushAsync(byteBuffer);
            }
        }
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            context.CloseAsync();
        }
    }
}
