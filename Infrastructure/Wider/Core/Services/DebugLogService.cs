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
using Prism.Logging;
using System;
using System.Diagnostics;
using System.Reflection;
using Wider.Interfaces.Events;
using Wider.Interfaces.Services;

namespace Wider.Core.Services
{
    /// <summary>
    /// The NLogService for logging purposes
    /// </summary>
    internal class DebugLogService : ILoggerService
    {
        private readonly IEventAggregator _aggregator;

        /// <summary>
        /// The NLogService constructor
        /// </summary>
        /// <param name="aggregator">The injected event aggregator</param>
        public DebugLogService(IEventAggregator aggregator = null) => _aggregator = aggregator;

        private void WriteMessage(String message, Category category, Priority priority)
        {
        }

        #region ILoggerService Members

        /// <summary>
        /// The logging function
        /// </summary>
        /// <param name="message">A message to log</param>
        /// <param name="category">The category of logging</param>
        /// <param name="priority">The priority of logging</param>
        public void Log(String message, Category category, Priority priority)
        {
            Message = message;
            Category = category;
            Priority = priority;

            StackTrace trace = new StackTrace();
            StackFrame frame = trace.GetFrame(1); // 0 will be the inner-most method
            MethodBase method = frame.GetMethod();

            Debug.WriteLine($"{method.DeclaringType}: {message}, {category}, {priority}");

            _aggregator.GetEvent<LogEvent>().Publish(
                new DebugLogService {Message = Message, Category = Category, Priority = Priority});
        }

        /// <summary>
        /// The message which was last logged using the service
        /// </summary>
        public String Message { get; internal set; }

        /// <summary>
        /// The log message's category
        /// </summary>
        public Category Category { get; internal set; }

        /// <summary>
        /// The log message's priority
        /// </summary>
        public Priority Priority { get; internal set; }

        #endregion
    }
}