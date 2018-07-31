﻿#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using DryIoc;
using Prism.Events;
using Wider.Interfaces;

namespace Wider.Core
{
    /// <summary>
    /// Class Workspace
    /// </summary>
    internal class Workspace : AbstractWorkspace
    {
        /// <summary>
        /// The generic workspace that will be used if the application does not have its workspace
        /// </summary>
        /// <param name="container">The injected container - can be used by custom flavors of workspace</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public Workspace(IContainer container, IEventAggregator eventAggregator)
            : base(container, eventAggregator)
        {
        }
    }
}