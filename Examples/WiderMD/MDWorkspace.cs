#region License

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
using Prism.Logging;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wider.Core.Events;
using Wider.Core.Services;

namespace WiderMD
{
    internal class MDWorkspace : AbstractWorkspace
    {
        private String _document;
        private ILoggerService _logger;
        private const String _title = "Wider MD";

        public MDWorkspace(IContainerExtension container) : base(container)
        {
            IEventAggregator aggregator = Container.Resolve<IEventAggregator>();
            aggregator.GetEvent<ActiveContentChangedEvent>().Subscribe(ContentChanged);
            _document = "";
        }

        public override ImageSource Icon => new BitmapImage(new Uri("pack://application:,,,/WiderMD;component/Resources/Icon.ico"));

        public override String Title
        {
            get
            {
                String newTitle = _title;
                if (_document != "")
                {
                    newTitle += " - " + _document;
                }
                return newTitle;
            }
        }

        private ILoggerService Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = Container.Resolve<ILoggerService>();
                }

                return _logger;
            }
        }

        private void ContentChanged(ContentViewModel<ContentModel> model)
        {
            _document = model == null ? "" : model.Title;
            RaisePropertyChanged("Title");
            if(model != null)
            {
                Logger.Log($"Active document changed to {model.Title}", Category.Info, Priority.None);
            }
        }

        protected override void ModelChangedEventHandler(Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            String newValue = ActiveDocument == null ? "" : ActiveDocument.Title;
            if (_document != newValue)
            {
                _document = newValue;
                RaisePropertyChanged("Title");
                base.ModelChangedEventHandler(sender, e);
            }
        }
    }
}