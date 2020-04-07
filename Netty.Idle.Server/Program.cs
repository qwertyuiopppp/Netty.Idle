using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Threading.Tasks;

namespace Netty.Idle.Server
{
	class Program
	{
		static int Port => 8007;

		static async Task RunServerAsync()
		{
			var bossGroup = new MultithreadEventLoopGroup(1);
			var workerGroup = new MultithreadEventLoopGroup();
			try
			{
				var bootstrap = new ServerBootstrap();
				bootstrap
					.Group(bossGroup, workerGroup)
					.Channel<TcpServerSocketChannel>()
					.Option(ChannelOption.SoBacklog, 100)
					.Handler(new LoggingHandler("LSTN"))
					.ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
					{
						IChannelPipeline pipeline = channel.Pipeline;
						pipeline.AddLast(new IdleStateHandler(0, 0, 5));
						pipeline.AddLast("decoder", new StringDecoder());
						pipeline.AddLast("encoder", new StringEncoder());
						pipeline.AddLast(new HeartBeatServerHandler());
					}));

				IChannel bootstrapChannel = await bootstrap.BindAsync(Port);

				Console.ReadLine();

				await bootstrapChannel.CloseAsync();
			}
			finally
			{
				Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
			}
		}

		public static void Main() => RunServerAsync().Wait();
	}
}
