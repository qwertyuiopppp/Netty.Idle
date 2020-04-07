using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netty.Idle.Server
{
	public class HeartBeatServerHandler : ChannelHandlerAdapter
	{

		public override void ChannelRead(IChannelHandlerContext ctx, Object msg)
		{
			Console.WriteLine("server channelRead..");
			Console.WriteLine(ctx.Channel.RemoteAddress + "->Server :" + msg.ToString());
		}

		public override void ExceptionCaught(IChannelHandlerContext ctx, Exception cause)
		{
			Console.WriteLine(cause);
			ctx.CloseAsync();
		}
	}
}
