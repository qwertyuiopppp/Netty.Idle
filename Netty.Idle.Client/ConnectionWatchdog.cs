using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Netty.Idle.Client
{
	public class ConnectionWatchdog : ChannelHandlerAdapter, ITimerTask
	{
		ConnectorIdleStateTrigger idleStateTrigger = new ConnectorIdleStateTrigger();
		private Bootstrap bootstrap;
		private ITimer timer;
		private int port;

		private String host;

		private volatile bool reconnect = true;
		private int attempts;


		public ConnectionWatchdog(Bootstrap bootstrap, ITimer timer, int port, String host, bool reconnect)
		{
			this.timer = new HashedWheelTimer();
			this.bootstrap = bootstrap;
			this.timer = timer;
			this.port = port;
			this.host = host;
			this.reconnect = reconnect;
			Console.WriteLine();
		}

		/**
         * channel链路每次active的时候，将其连接的次数重新☞ 0
         */
		public override void ChannelActive(IChannelHandlerContext ctx)
		{
			Console.WriteLine("当前链路已经激活了，重连尝试次数重新置为0");
			attempts = 0;
			ctx.FireChannelActive();
		}

		public override void ChannelInactive(IChannelHandlerContext ctx)
		{
			Console.WriteLine("链接关闭");
			Reconnect();
			ctx.FireChannelInactive();
		}

		public void Reconnect()
		{
			if (reconnect)
			{
				Console.WriteLine("链接关闭，将进行重连");
				if (attempts < 12)
				{
					attempts++;
					//重连的间隔时间会越来越长
					int timeout = 2 << attempts;
					var delay = new TimeSpan(0, 0, 5);
					timer.NewTimeout(this, delay);

				}
			}
		}



		public void Run(ITimeout timeout)
		{
			try
			{
				var channel = bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(host), port)).GetAwaiter().GetResult();
			}
			catch (Exception e)
			{
				Console.WriteLine($"连接失败:{e}");
				Reconnect();
			}
		}
	}
}
