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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using Wider.Interfaces.Services;

namespace Wider.Interfaces
{
    /// <summary>
    /// The abstract class which has to be inherited if you want to create a tool
    /// </summary>
    public abstract class ToolViewModel : ViewModelBase, ITool
    {
        #region Members

        protected String _contentId = null;
        protected Boolean _isActive = false;
        protected Boolean _isSelected = false;
        private Boolean _isVisible = true;

        protected String _title = null;

        #endregion

        #region Property

        /// <summary>
        /// The name of the tool
        /// </summary>
        public String Name { get; protected set; }

        /// <summary>
        /// The visibility of the tool
        /// </summary>
        public Boolean IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged("IsVisible");
                }
            }
        }


        /// <summary>
        /// pane location
        /// </summary>
        public abstract PaneLocation PreferredLocation { get; }

        /// <summary>
        /// Prefered width
        /// </summary>
        public virtual Double PreferredWidth => 200;

        /// <summary>
        /// prefered height
        /// </summary>
        public virtual Double PreferredHeight => 200;

        /// <summary>
        /// The content model
        /// </summary>
        /// <value>The model.</value>
        public virtual ToolModel Model { get; set; }

        /// <summary>
        /// The content view
        /// </summary>
        /// <value>The view.</value>
        public virtual IContentView View { get; set; }

        /// <summary>
        /// The title of the document
        /// </summary>
        /// <value>The title.</value>
        public virtual String Title
        {
            get => _title;
            protected set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        public IReadOnlyList<String> Menus => new List<String>() { "a", "b", "c" };

        /// <summary>
        /// The image source that can be used as an icon in the tab
        /// </summary>
        /// <value>The icon source.</value>
        public virtual ImageSource IconSource { get; protected set; }

        /// <summary>
        /// The content ID - unique value for each document
        /// </summary>
        /// <value>The content id.</value>
        public virtual String ContentId
        {
            get => _contentId;
            protected set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    RaisePropertyChanged("ContentId");
                }
            }
        }

        /// <summary>
        /// Is the document selected
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public virtual Boolean IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// Is the document active
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public virtual Boolean IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }
        }

        #endregion
    }
}