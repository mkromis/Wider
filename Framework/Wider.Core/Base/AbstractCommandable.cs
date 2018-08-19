﻿#region License
// Copyright (c) 2018 Mark Kromis
// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Windows.Input;

namespace Wider.Core
{
    /// <summary>
    /// Class AbstractCommandable
    /// </summary>
    public class AbstractCommandable : AbstractPrioritizedTree<AbstractCommandable>, ICommandable
    {
        #region CTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCommandable"/> class.
        /// </summary>
        protected AbstractCommandable() : base()
        {
        }

        #endregion

        #region ICommandable

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public virtual ICommand Command { get; protected internal set; }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        /// <value>The command parameter.</value>
        public virtual Object CommandParameter { get; set; }

        /// <summary>
        /// Gets the input gesture text.
        /// </summary>
        /// <value>The input gesture text.</value>
        public String InputGestureText { get; internal set; }

        #endregion
    }
}