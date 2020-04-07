using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Netty.Idle.Client
{
	class Program
	{
        static string Host => "127.0.0.1";
        static int Port => 8007;
        static HashedWheelTimer timer = new HashedWheelTimer();
        static async Task RunClientAsync()
        {

            var group = new MultithreadEventLoopGroup(1);
            var workGroup = new MultithreadEventLoopGroup(2);
            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        pipeline.AddLast(new LoggingHandler());
                        pipeline.AddLast("connectionWatchdog", new ConnectionWatchdog(bootstrap, timer, Port, Host, true));
                        pipeline.AddLast("timeout", new IdleStateHandler(0, 0, 5));
                        pipeline.AddLast("idleStateTrigger", new ConnectorIdleStateTrigger());
                        pipeline.AddLast("decoder", new StringDecoder());
                        pipeline.AddLast("encoder", new StringEncoder());
                        pipeline.AddLast(workGroup, new HeartBeatClientHandler());
                    }));
                IChannel bootstrapChannel;

                bootstrapChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Host), Port));

                Console.ReadLine();

                await bootstrapChannel.CloseAsync();
            }
            finally
            {
                group.ShutdownGracefullyAsync().Wait(1000);
            }
        }

        public static void Main() => RunClientAsync().Wait();
    }
}
