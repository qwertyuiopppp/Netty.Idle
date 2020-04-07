using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netty.Idle.Client
{
	public interface IChannelHandlerHolder 
	{

		IChannelHandler[] handlers();
	}
}
