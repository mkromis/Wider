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

using Autofac;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Wider.Core.Services;

namespace Wider.Core.Controls
{
    /// <summary>
    /// Class AbstractMenuItem - representation of a Menu
    /// </summary>
    public abstract class AbstractMenuItem : AbstractCommandable, IMenuService
    {
        #region Static

        /// <summary>
        /// The static separator count
        /// </summary>
        protected static Int32 sepCount = 1;

        #endregion

        #region Members

        /// <summary>
        /// The injected container
        /// </summary>
        protected IContainer _container;

        /// <summary>
        /// Is the menu checked
        /// </summary>
        protected Boolean _isChecked;

        #endregion

        #region Methods and Properties

        /// <summary>
        /// Gets a value indicating whether this instance is separator.
        /// </summary>
        /// <value><c>true</c> if this instance is separator; otherwise, <c>false</c>.</value>
        public virtual Boolean IsSeparator { get; internal set; }

        /// <summary>
        /// Gets the icon of the menu.
        /// </summary>
        /// <value>The icon.</value>
        public virtual ImageSource Icon { get; internal set; }

        /// <summary>
        /// Gets the tool tip.
        /// </summary>
        /// <value>The tool tip.</value>
        public virtual String ToolTip
        {
            get
            {
                String value = Header.Replace("_", "");
                if (!String.IsNullOrEmpty(InputGestureText))
                {
                    value += " " + InputGestureText;
                }
                return value;
            }
        }

        /// <summary>
        /// Gets the header of the menu.
        /// </summary>
        /// <value>The header.</value>
        public virtual String Header { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value><c>true</c> if this instance is checkable; otherwise, <c>false</c>.</value>
        public virtual Boolean IsCheckable { get; protected internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        public virtual Boolean IsVisible { get; protected internal set; }

        /// <summary>
        /// Gets a value indicating whether to hide this menu item when disabled.
        /// </summary>
        /// <value><c>true</c> if this instance should be hidden when disabled; otherwise, <c>false</c>.</value>
        public virtual Boolean HideDisabled { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value><c>true</c> if this instance is checked; otherwise, <c>false</c>.</value>
        public virtual Boolean IsChecked
        {
            get => _isChecked;
            protected internal set
            {
                _isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override String ToString() => Header;

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if successfully added, <c>false</c> otherwise</returns>
        /// <exception cref="System.ArgumentException">Expected a AbstractMenuItem as the argument. Only Menu's can be added within a Menu.</exception>
        public override String Add(AbstractCommandable item)
        {
            if (item.GetType().IsAssignableFrom(typeof (AbstractMenuItem)))
            {
                throw new ArgumentException(
                    "Expected a AbstractMenuItem as the argument. Only Menu's can be added within a Menu.");
            }
            return base.Add(item);
        }

        public virtual void Refresh()
        {
            RaisePropertyChanged("Header");
            RaisePropertyChanged("Command");
            RaisePropertyChanged("Children");
            RaisePropertyChanged("Icon");
            RaisePropertyChanged("ToolTip");
            RaisePropertyChanged("IsVisible");
        }

        #endregion

        #region CTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMenuItem"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="command">The command.</param>
        /// <param name="gesture">The gesture.</param>
        /// <param name="isCheckable">if set to <c>true</c> acts as a checkable menu.</param>
        protected AbstractMenuItem(String header, Int32 priority, ImageSource icon = null, ICommand command = null,
                                   KeyGesture gesture = null, Boolean isCheckable = false, Boolean hideDisabled = false)
        {
            Priority = priority;
            IsSeparator = false;
            Header = header;
            Key = header;
            Command = command;
            IsCheckable = isCheckable;
            HideDisabled = hideDisabled;
            Icon = icon;
            if (gesture != null && command != null)
            {
                Application.Current.MainWindow.InputBindings.Add(new KeyBinding(command, gesture));
                InputGestureText = gesture.DisplayString;
            }
            if (isCheckable)
            {
                IsChecked = false;
            }
            if (Header == "SEP")
            {
                Key = "SEP" + sepCount.ToString();
                Header = "";
                sepCount++;
                IsSeparator = true;
            }
        }

        #endregion
    }
}