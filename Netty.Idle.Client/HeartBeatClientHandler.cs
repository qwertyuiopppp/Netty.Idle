using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netty.Idle.Client
{
	public class HeartBeatClientHandler : ChannelHandlerAdapter
	{

		public override void ChannelActive(IChannelHandlerContext ctx)
		{
			Console.WriteLine("激活时间是：" + DateTime.Now);
			Console.WriteLine("HeartBeatClientHandler channelActive");
			ctx.FireChannelActive();
		}

		public override void ChannelInactive(IChannelHandlerContext ctx)
		{
			Console.WriteLine("停止时间是：" + DateTime.Now);
			Console.WriteLine("HeartBeatClientHandler channelInactive");
		}


		public override void ChannelRead(IChannelHandlerContext ctx, Object msg)
		{
			String message = (String)msg;
			Console.WriteLine(message);
			if (message.Equals("Heartbeat"))
			{
				ctx.WriteAndFlushAsync("has read message from server");
			}
			ReferenceCountUtil.Release(msg);
		}

	}
}
