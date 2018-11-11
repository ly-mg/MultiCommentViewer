using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiCommentViewer
{
	class MainViewModel
	{
		public event EventHandler<EventArgs> CloseRequested;
		public void RequestClose()
		{
			OnCloseRequested(EventArgs.Empty);
		}

		protected virtual void OnCloseRequested(EventArgs e)
		{
            CloseRequested?.Invoke(this, e);
        }

		public void Initialize()
		{
			Thread.Sleep(TimeSpan.FromSeconds(5));
		}


		public async Task InitializeAsync()
		{
            //use this to test the exception handling
            //throw new NotImplementedException();
            await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
		}

		
	}
}
