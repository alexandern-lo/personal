using System;
using System.Threading.Tasks;
using SL4N;

namespace StudioMobile
{
	public static class TaskExtensions
	{
		private static readonly ILogger LOG = LoggerFactory.GetLogger<Task>();

		public static void Ignore (this Task task)
		{
			task.ContinueWith(t => {
				if (t.Status == TaskStatus.Faulted) {
					LOG.Warn ("Warning: Unhandled exception in Ignored task", t.Exception);
				}
			});
		}
	}
}

