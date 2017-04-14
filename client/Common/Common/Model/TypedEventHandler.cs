using System;

namespace StudioMobile
{
	public delegate void TypedEventHandler<TSender, TResult>(
		TSender sender, 
		TResult args
	);
}

