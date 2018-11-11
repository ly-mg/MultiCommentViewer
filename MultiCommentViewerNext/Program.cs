using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MultiCommentViewer
{
    class Program
    {
        [STAThread]
        //static async Task Main(string[] args)
        static void Main()
        {
            AppNoStartupUri app = new AppNoStartupUri
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };
            app.InitializeComponent();
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

            var p = new Program();
            p.ExitRequested += (sender, e) =>
            {
                app.Shutdown();
            };

            var t = p.StartAsync();
            Handle(t);
            app.Run();
        }
        static async void Handle(Task t)
        {
            try
            {
                await t;
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public async Task StartAsync()
        {
            MainViewModel viewModel = new MainViewModel();
            viewModel.CloseRequested += viewModel_CloseRequested;

            EventHandler windowClosed = (sender, e) =>
            {
                viewModel.RequestClose();
            };

            SplashScreen splashScreen = new SplashScreen();  //not disposable, but I'm keeping the same structure
            {
                splashScreen.Closed += windowClosed; //if user closes splash screen, let's quit
                splashScreen.Show();

                await viewModel.InitializeAsync();

                MainWindow mainForm = new MainWindow();
                mainForm.Closed += windowClosed;
                mainForm.DataContext = viewModel;
                mainForm.Show();

                splashScreen.Owner = mainForm;
                splashScreen.Closed -= windowClosed;
                splashScreen.Close();
            }
        }

        public event EventHandler<EventArgs> ExitRequested;
        void viewModel_CloseRequested(object sender, EventArgs e)
        {
            OnExitRequested(EventArgs.Empty);
        }

        protected virtual void OnExitRequested(EventArgs e)
        {
            if (ExitRequested != null)
                ExitRequested(this, e);
        }
    }
}
