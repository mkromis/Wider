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
using Wider.Core.Events;
using Wider.Core.Services;
using Wider.Tools.Logger.ViewModels;

namespace Wider.Tools.Logger
{
    [Module(ModuleName = "Wider.Tools.Logger")]
    public sealed class LoggerModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // register types
            // This interface has the view model handle the view construdtion
            containerRegistry.RegisterSingleton<LoggerViewModel>();
        }

        #region IModule Members
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IEventAggregator eventAggregator = containerProvider.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<SplashMessageUpdateEvent>()
                .Publish(new SplashMessageUpdateEvent {Message = "Loading Logger Module"});

            IWorkspace workspace = containerProvider.Resolve<IWorkspace>();
            workspace.Tools.Add(containerProvider.Resolve<LoggerViewModel>());
        }

        #endregion
    }
}