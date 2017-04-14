using System;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace StudioMobile
{
	public interface ICommand
	{
		event EventHandler CanExecuteChanged;

		bool CanExecute(object parameter);

		void Execute(object parameter);
	}

	public static class CommandExtensions
	{
		public static void Execute(this ICommand command)
		{
			command.Execute(null);
		}

		public static bool CanExecute(this ICommand command)
		{
			return command.CanExecute(null);
		}
	}

	public abstract class CommandBase : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public void RaiseCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
			{
				CanExecuteChanged(this, EventArgs.Empty);
			}
		}

		bool ICommand.CanExecute(object parameter)
		{
			return CheckCanExecute(parameter);
		}

		protected abstract bool CheckCanExecute(object parameter);

		public void Execute(object parameter)
		{
			if (CheckCanExecute(parameter))
			{
				DoExecute(parameter);
			}
		}

		protected abstract void DoExecute(object parameter);
	}

	public class Command : CommandBase
	{
		public Command()
		{
		}

		public Command(Action action)
		{
			Action = _ => action();
		}

		public Command(Action action, Func<bool> canExecute) : this(action)
		{
			CanExecute = _ => canExecute();
		}

		public Action<object> Action { get; set; }

		public Func<object, bool> CanExecute { get; set; }

		protected override bool CheckCanExecute(object parameter)
		{
			return Action != null && (CanExecute == null || CanExecute(parameter));
		}

		protected override void DoExecute(object parameter)
		{
			Action(parameter);
		}
	}

	public interface IAsyncCommand : ICommand, INotifyPropertyChanged
	{
		bool IsRunning { get; }
		Exception Error { get; }
		Task ExecuteAsync(object parameter);
		IAsyncCommand CancelCommand { get; }
		bool CanCancel { get; }
	}

	public static class AsyncCommandExtensions
	{
		public static Task ExecuteAsync(this IAsyncCommand command)
		{
			return command.ExecuteAsync(null);
		}
	}

	public static class AsyncCommandProperties
	{
		public static IProperty<bool> IsRunningProperty<T>(this T command)
			where T : IAsyncCommand
		{
			return command.Property<T, bool>("IsRunning");
		}

	}

	public abstract class AsyncCommandBase : CommandBase, IAsyncCommand
	{
		protected AsyncCommandBase()
		{
		}

		sealed protected override void DoExecute(object parameter)
		{
            ExecuteAsyncImpl(parameter).Ignore();
		}

		public async Task ExecuteAsync(object parameter)
		{
			if (!CheckCanExecute(parameter))
			{
				return;
			}
			await ExecuteAsyncImpl(parameter);
			if (Error != null)
			{
				throw Error;
			}
		}

		async Task ExecuteAsyncImpl(object parameter)
		{
			Error = null;
            IsRunningCount++;
			try
			{
				await DoExecuteAsync(parameter);
			}
			catch (Exception e)
			{
				Error = e;
			}
			finally
			{
                IsRunningCount--;
				if (cancelCommand != null && cancelCommand.IsRunning)
				{
					cancelCommand.IsRunning = false;
				}
				cts = null;
			}
		}

		protected abstract Task DoExecuteAsync(object parameter);

		CancelAsyncCommand cancelCommand;

		public IAsyncCommand CancelCommand
		{
			get
			{
				if (cancelCommand == null)
				{
					cancelCommand = new CancelAsyncCommand(this);
				}
				return cancelCommand;
			}
		}

		const string IsRunningErrorMessage = "Cannot change {Key} while command is running";
		public bool CanCancel
		{
			get { return true; }
		}

		CancellationTokenSource cts;

		public CancellationTokenSource CancellationTokenSource
		{
			get
			{
				if (cts == null)
					cts = new CancellationTokenSource();
				return cts;
			}
			set
			{
				Check.Argument(value, "value").NotNull();
				Check.State(IsRunning).IsFalse(IsRunningErrorMessage);
				cts = value;
			}
		}

		public CancellationToken Token { get { return CancellationTokenSource.Token; } }

		public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(PropertyChangedEventArgs key)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, key);
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string key = null)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(key));
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> key)
        {
            RaisePropertyChanged(PropertySupport.ExtractPropertyName(key));
        }

        int isRunningCount = 0;
        protected int IsRunningCount
        {
            get { return isRunningCount; }
            set
            {
                isRunningCount = value;
                IsRunning = isRunningCount > 0;
            }
        }

        bool isRunning;

		public bool IsRunning
		{
			get { return isRunning; }
			private set
			{
				if (isRunning != value)
				{
					isRunning = value;
                    RaisePropertyChanged();
					RaiseCanExecuteChanged();
				}
			}
		}

		void Cancel()
		{
            if (cts != null && !cts.IsCancellationRequested && IsRunning)
			{
				cts.Cancel();
				//if still running after call to Cancel then cancelCommand isRunning
				if (IsRunning)
				{
					cancelCommand.IsRunning = true;
				}
			}
		}

		public bool IsCancelRunning
		{
			get { return cancelCommand != null && cancelCommand.IsRunning; }
		}

		Exception error;
		public Exception Error
		{
			get { return error; }
			set
			{
				if (error != value)
				{
					error = value;
                    RaisePropertyChanged();
				}
			}
		}

		class CancelAsyncCommand : CommandBase, IAsyncCommand
		{
			readonly AsyncCommandBase owner;

			public CancelAsyncCommand(AsyncCommandBase owner)
			{
				this.owner = owner;
				owner.PropertyChanged += Owner_IsRunningChanged;
			}

			protected override bool CheckCanExecute(object parameter)
			{
				return !IsRunning && owner.IsRunning;
			}

			protected override void DoExecute(object parameter)
			{
                ExecuteAsyncImpl().Ignore();
			}

			public async Task ExecuteAsync(object parameter)
			{
				if (!CheckCanExecute(parameter))
				{
					return;
				}
				await ExecuteAsyncImpl();
			}

			TaskCompletionSource<object> completionSource;

			Task ExecuteAsyncImpl()
			{
				try
				{
					IsRunning = true;
					owner.Cancel();
				}
				catch (Exception e)
				{
					Error = e;
					IsRunning = false;
				}
				return IsRunning ? completionSource.Task : Task.FromResult<object>(null);
			}

			void Owner_IsRunningChanged(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == "IsRunning")
				{
					if (!owner.IsRunning && IsRunning)
					{
						IsRunning = false;
					}
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			public bool IsRunning
			{
				get { return completionSource != null; }
				set
				{
					if (value != IsRunning)
					{
						if (value)
						{
							completionSource = new TaskCompletionSource<object>();
						}
						else {
							completionSource.TrySetResult(null);
							completionSource = null;
						}
						if (PropertyChanged != null)
						{
							PropertyChanged(this, new PropertyChangedEventArgs("IsRunning"));
						}
						RaiseCanExecuteChanged();
					}
				}
			}

			Exception error;
			public Exception Error
			{
				get { return error; }
				private set
				{
					if (value != error)
					{
						error = value;
						if (PropertyChanged != null)
						{
							PropertyChanged(this, new PropertyChangedEventArgs("Error"));
						}
					}
				}
			}

			public IAsyncCommand CancelCommand
			{
				get { return null; }
			}

			public bool CanCancel
			{
				get { return false; }
			}
		}
	}

	public sealed class AsyncCommand : AsyncCommandBase
	{
		CancellationTokenSource delayToken = new CancellationTokenSource();

		public AsyncCommand()
		{ }

		public AsyncCommand(Func<Task> action) : this(action, null)
		{
		}

		public AsyncCommand(Func<Task> action, Func<bool> canExecute)
		{
			if (action != null)
				Action = _ => action();
			if (canExecute != null)
				CanExecute = _ => canExecute();
			Delay = TimeSpan.Zero;
		}

		protected override async Task DoExecuteAsync(object parameter)
		{
			if (Action != null)
			{
				try
				{
					if (delayToken != null && !delayToken.IsCancellationRequested)
						delayToken.Cancel();
					if (Delay > TimeSpan.Zero)
					{
						delayToken = new CancellationTokenSource();
						await Task.Delay(Delay, delayToken.Token);
						//check if action still can be executed after delay
						if (!CheckCanExecute(parameter)) return;
					}

					var task = Action(parameter);
					if (task != null)
					{
						await task;
					}
				}
				catch (OperationCanceledException)
				{
				}
			}
		}

		protected override bool CheckCanExecute(object parameter)
		{
			return Action != null && (CanExecute == null || CanExecute(parameter));
		}

		public Func<object, Task> Action { get; set; }

		public Func<object, bool> CanExecute { get; set; }

		public TimeSpan Delay { get; set; }

		public async Task ExecuteWithoutDelay(object param)
		{
			var oldDelay = Delay;
			try
			{
				Delay = TimeSpan.Zero;
				await ExecuteAsync(param);
			}
			finally
			{
				Delay = oldDelay;
			}
		}
	}
}
