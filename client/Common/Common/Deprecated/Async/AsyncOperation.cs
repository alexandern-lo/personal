using System;
using System.Threading;
using System.Threading.Tasks;
using SL4N;

namespace StudioMobile
{
	public interface IAsyncOperation
	{
		event Action<IAsyncOperation> Start;
		event Action<IAsyncOperation> Stop;

		bool IsRunning { get; }

		Task Task { get; }

		void Cancel();
	}

	[Flags]
	public enum AsyncOperationMode
	{
		WaitPrevious = 0x1,
		ThrowCancellationException = 0x2,
		ThrowExceptions = 0x4,
		CancelPrevious = 0x8,
		Normal = CancelPrevious
	}

	public class AsyncOperation : IAsyncOperation
	{
		private static readonly ILogger LOG = LoggerFactory.GetLogger<AsyncOperation>();

		public struct Result<T> {
			public bool Cancelled;
			public T Value;

			public bool HasValue { get { return !Cancelled; } }
		}

		CancellationTokenSource runningTaskCTS, newTaskCTS;
		int requests = 0;
		AsyncOperationMode mode;
		Task runningTask;

		public AsyncOperation() : this(AsyncOperationMode.Normal)
		{
		}

		public AsyncOperation (AsyncOperationMode mode)
		{
			this.mode = mode;
			newTaskCTS = new CancellationTokenSource ();
		}

		public CancellationToken Token { get { return newTaskCTS.Token; } }

		public Task<Result<T>> Run<T> (Task<T> task)
		{
			return Run (_ => task);
		}

		public Task<bool> Run (Task task)
		{
			return Run (_ => task);
		}

		public Task<T> Run<T> (Task<T> task, T defaultValue)
		{
			return Run (_ => task, defaultValue);
		}

		public async Task<Result<T>> Run<T> (Func<CancellationToken, Task<T>> op)
		{
			var result = new Result<T> { 
				Cancelled = false,
				Value = default(T)
			};
			try {
				if (runningTaskCTS != null) {
					if (mode.HasFlag (AsyncOperationMode.CancelPrevious)) {						
						runningTaskCTS.Cancel ();
					}
					if (mode.HasFlag (AsyncOperationMode.WaitPrevious)) {
						try {
							await runningTask;
						} catch (Exception e) {
							LOG.Warn("{0} Exception during task cancellation {1}", GetHashCode(), e);
						}
					}
				}
				runningTaskCTS = newTaskCTS;
				newTaskCTS = new CancellationTokenSource();
				requests++;
				if (requests == 1) {
					if (Start != null) {
						Start(this);
					}
				}
				var task = op(runningTaskCTS.Token);
				runningTask = task;
				result.Value = await task;
				result.Cancelled = runningTaskCTS.IsCancellationRequested;
			} catch (OperationCanceledException) {
				result.Cancelled = true;
				if (mode.HasFlag(AsyncOperationMode.ThrowCancellationException)) {
					throw;
				}
			} catch (Exception) {
				result.Cancelled = true;
				if (mode.HasFlag(AsyncOperationMode.ThrowExceptions)) {
					throw;
				}
			} finally {
				requests--;
				if (requests == 0) {
					if (Stop != null) {
						Stop (this);
					}
				}
			}
			return result;
		}

		public async Task<bool> Run (Func<CancellationToken, Task> op)
		{
			var r = await Run<object> (async t => { 
				await op (t); 
				return null;
			});
			return r.HasValue;
		}

		public async Task<T> Run<T> (Func<CancellationToken, Task<T>> op, T defaultValue)
		{
			var result = await Run (op);
			if (result.Cancelled) {
				result.Value = defaultValue;
			}
			return result.Value;
		}

		public event Action<IAsyncOperation> Start;
		public event Action<IAsyncOperation> Stop;

		public Task Task { get { return runningTask; } }

		public bool IsRunning { get { return requests > 0; } }

		public void Cancel() {
			if (runningTaskCTS != null) {
				runningTaskCTS.Cancel ();
			}
		}
	}
}

