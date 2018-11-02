#region License

// Copyright (c) 2018 Mark Kromis
// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Wider.Core.Events;
using Wider.Core.Services;
using Wider.Splash.Views;

namespace Wider.Splash
{
    [Module(ModuleName = "Wider.Splash")]
    public sealed class SplashModule : IModule
    {
        #region ctors
        public SplashModule(IEventAggregator eventAggregator_) => EventAggregator = eventAggregator_;

        #endregion

        #region Private Properties

        private IEventAggregator EventAggregator { get; set; }

        //private IShell Shell { get; set; }

        private AutoResetEvent WaitForCreation { get; set; }

        #endregion

        #region IModule Members

        public void RegisterTypes(IContainerRegistry containerRegistry) => containerRegistry.RegisterSingleton<ISplashView, SplashView>();

        public void OnInitialized(IContainerProvider containerProvider)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
            {
               (containerProvider.Resolve<IShell>() as Window).Show();
               EventAggregator.GetEvent<SplashCloseEvent>().Publish(new SplashCloseEvent());
            }));

            WaitForCreation = new AutoResetEvent(false);

            void showSplash()
            {
                Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
                {
                   ISplashView iSplashView = containerProvider.Resolve<ISplashView>();
                   if (iSplashView is Window splash)
                   {
                       EventAggregator.GetEvent<SplashCloseEvent>().Subscribe(
                           e_ => splash.Dispatcher.BeginInvoke((Action)splash.Close),
                           ThreadOption.PublisherThread, true);

                       splash.Show();
                       WaitForCreation.Set();
                   }
                }));

                Dispatcher.Run();
            }

            Thread thread = new Thread(showSplash) { Name = "Splash Thread", IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            WaitForCreation.WaitOne();
        }
        #endregion
    }
}