﻿#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Windows.Controls;
using Wider.Content.TextDocument.ViewModels;
using Wider.Interfaces;
using Wider.Interfaces.Services;

namespace WiderMD.Core
{
    internal class MDViewModel : TextViewModel
    {
        public MDViewModel(
            AbstractWorkspace workspace, ICommandManager commandManager, ILoggerService logger, IMenuService menuService)
            : base(workspace, commandManager, logger, menuService)
        {
        }

        internal void SetModel(ContentModel model) => base.Model = model;

        internal void SetView(IContentView view) => base.View = view;

        internal void SetHandler(IContentHandler handler) => Handler = handler;
    }
}