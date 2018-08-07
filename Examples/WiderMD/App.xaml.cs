#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Windows;
using Wider.Interfaces;

namespace WiderMD
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MDBootstrapper b;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            b = new MDBootstrapper();
            b.Run();
            IShell shell = b.Container.Resolve(typeof(IShell), true) as IShell;
            (shell as Window).Loaded += App_Loaded;
            (shell as Window).Unloaded += App_Unloaded;
        }

        void App_Unloaded(Object sender, System.EventArgs e)
        {
            IShell shell = b.Container.Resolve(typeof(IShell), true) as IShell;
            shell.SaveLayout();
        }

        void App_Loaded(Object sender, RoutedEventArgs e)
        {
            IShell shell = b.Container.Resolve(typeof(IShell), true) as IShell;
            shell.LoadLayout();
        }
    }
}