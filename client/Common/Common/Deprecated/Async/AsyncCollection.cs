using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using SL4N;
namespace StudioMobile
{
	/*
	 * AsyncCollection stay between UI and domain object. 
	 * UI uses readonly IAsyncCollection interface to display items
	 * and react on loading notifications.
	 * Domain objects use mutable AsyncCollection to load it from 
	 * external storage or network and modify it upon UI request.
	 * Domain objects expose async collection as a readonly interface
	 * to UI so UI can display it and subscribe to events.
	 */
	public interface IAsyncCollection<T> : IReadOnlyCollection<T>
	{
		event Action<IAsyncCollection<T>> StartLoading;
		event Action<IAsyncCollection<T>> StopLoading;

		bool IsLoading { get; }

		Task Task { get; }

		T this [int idx] { get; }

	}

	//Mutable collection suitable for domain objects and loaders
	public class AsyncCollection<T> : IAsyncCollection<T>, IList<T>
	{
		private static readonly ILogger LOG = LoggerFactory.GetLogger<AsyncCollection<T>>();

		List<T> items = new List<T> ();
		AsyncOperation operation = new AsyncOperation();
		int version = 0;

		public AsyncCollection ()
		{
			operation.Start += NotifyStartLoading;
			operation.Stop += NotifyStopLoading;
		}

		public int Version { 
			get { return version; }
		}

		public bool IsLoading { 
			get { return operation.IsRunning; }
		}

		public T this [int idx] { 
			get { return items [idx]; } 
			set { items [idx] = value; }
		}

		public event Action<IAsyncCollection<T>> StartLoading;

		public void NotifyStartLoading (IAsyncOperation op)
		{
			if (StartLoading != null) {
				StartLoading (this);
			}
		}

		public event Action<IAsyncCollection<T>> StopLoading;

		public void NotifyStopLoading (IAsyncOperation op)
		{
			version++;
			if (StopLoading != null) {
				StopLoading (this);
			}
		}

		#region ReadonlyCollection

		public IEnumerator<T> GetEnumerator ()
		{
			return items.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return items.GetEnumerator ();
		}

		public int Count {
			get { return items.Count; }
		}

		#endregion

		#region Async mutators

		public Task Task { get { return operation.Task; } }

		public CancellationToken Token { get { return operation.Token; } }

		public async Task<bool> Reset (Task<IEnumerable<T>> task)
		{			
			var result = await operation.Run (async t => {
				var newItems = await task;
				items.Clear ();
				items.AddRange (newItems);
				return items;
			});
			return result.HasValue;
		}

		public async Task<IEnumerable<T>> AddRange (Task<IEnumerable<T>> task)
		{
			var result = await operation.Run(async t => {
				var newItems = await task;
				items.AddRange (newItems);
				return newItems;
			});
			return result.Value;
		}
			
		public async Task<int> Remove (Task<T> task)
		{
			var result = await operation.Run (async t => {
				var item = await task;
				var idx = items.IndexOf(item);
				items.Remove (item);
				return idx;
			});
			return result.Cancelled ? -1 : result.Value;
		}

		public async Task<bool> InsertRange (int index, Task<IEnumerable<T>> task)
		{
			var result = await operation.Run (async t => {
				var newItems = await task;
				items.InsertRange(index, newItems);	
				return newItems;
			});
			return result.HasValue;
		}

		//cancel runnig task or wait while task finished
		//if task was started without CancellationToken
		public async Task Cancel()
		{
			if (operation.IsRunning) {
				try {
					operation.Cancel ();
					await operation.Task;
				} catch (Exception e) {
					LOG.Warn ("Task canceled with exception", e);
					//ignore all errors since task is being canceled
				}
			}
		}
			
		#endregion


		#region Sync mutators

		public void Sort (Comparison<T> comparator)
		{
			items.Sort (comparator);
		}

		public void Merge (IList<T> newItems, Comparison<T> compare)
		{
			var result = new List<T> (items.Count + newItems.Count);
			int itemsIdx = 0;
			foreach (var item in newItems) {

				while (itemsIdx < items.Count && compare (item, items [itemsIdx]) >= 0) {
					result.Add(items [itemsIdx++]);
				}

				result.Add(item);
			}
			items = result;
		}

		public int IndexOf (T item)
		{
			return items.IndexOf (item);
		}

		public void Insert (int index, T item)
		{
			items.Insert (index, item);
		}

		public void InsertRange (int index, IEnumerable<T> range)
		{
			items.InsertRange (index, range);
		}

		public void RemoveAt (int index)
		{
			items.RemoveAt (index);
		}

		public void Add (T item)
		{
			items.Add (item);
		}

		public bool Contains (T item)
		{
			return items.Contains (item);
		}

		public void CopyTo (T[] array, int arrayIndex)
		{
			items.CopyTo (array, arrayIndex);
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove (T item)
		{
			return items.Remove (item);
		}

		public int RemoveFirst (T item)
		{
			var idx = items.IndexOf (item);
			if (idx >= 0) {
				items.RemoveAt (idx);
				return idx;
			} else {
				return -1;
			}
		}

		public void Clear ()
		{
			items.Clear ();
		}

		public void AddRange (IEnumerable<T> range)
		{
			items.AddRange (range);
		}

		#endregion
	}
}
