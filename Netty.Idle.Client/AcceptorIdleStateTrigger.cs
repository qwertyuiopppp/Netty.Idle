using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netty.Idle.Client
{
	public class AcceptorIdleStateTrigger : ChannelHandlerAdapter
	{
		public override void UserEventTriggered(IChannelHandlerContext context, object evt)
		{
			if (evt is IdleStateEvent)
			{
				IdleState state = ((IdleStateEvent)evt).State;
				if (state == IdleState.ReaderIdle)
				{
					throw new Exception("idle exception");
				}
			}
			else
			{
				base.UserEventTriggered(context, evt);
			}

		}
	}
}
