using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netty.Idle.Client
{
	public class ConnectorIdleStateTrigger : ChannelHandlerAdapter
	{
		private static IByteBuffer HEARTBEAT_SEQUENCE = Unpooled.UnreleasableBuffer(Unpooled.CopiedBuffer("Heartbeat", Encoding.UTF8));

		public override void UserEventTriggered(IChannelHandlerContext context, object evt)
		{
			Console.WriteLine("ConnectorIdleStateTrigger.UserEventTriggered");
			if (evt is IdleStateEvent) {
				IdleState state = ((IdleStateEvent)evt).State;
				if (state == IdleState.AllIdle)
				{
					// write heartbeat to server
					context.WriteAndFlushAsync(HEARTBEAT_SEQUENCE.Duplicate());
				}
			} else
			{
				base.UserEventTriggered(context, evt);
			}
		}
	}
}
