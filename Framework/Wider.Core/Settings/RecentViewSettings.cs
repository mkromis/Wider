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

using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Serialization;
using Wider.Core.Controls;
using Wider.Core.Services;
using Wider.Core.Settings;

namespace Wider.Core.Settings
{
    internal class RecentViewSettings : AbstractSettings, IRecentViewSettings
    {
        private readonly AbstractMenuItem recentMenu;
        private readonly List<String> menuGuids;
        private readonly DelegateCommand<String> recentOpen;
        private readonly IContainerExtension _container;

        private IOpenDocumentService fileService;

        public RecentViewSettings(IContainerExtension container)
        {
            recentMenu = new MenuItemViewModel("Recentl_y opened..", 100);
            menuGuids = new List<String>();
            recentOpen = new DelegateCommand<String>(ExecuteMethod);
            _container = container;
        }

        [UserScopedSetting()]
        public List<RecentViewItem> ActualRecentItems
        {
            get
            {
                if ((List<RecentViewItem>)this["ActualRecentItems"] == null)
                {
                    this["ActualRecentItems"] = new List<RecentViewItem>((Int32)TotalItems);
                }

                return (List<RecentViewItem>)this["ActualRecentItems"];
            }
            set => this["ActualRecentItems"] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("10")]
        public UInt32 TotalItems
        {
            get => (UInt32)this["TotalItems"];
            set
            {
                this["TotalItems"] = value;
                ActualRecentItems.Capacity = (Int32)value;
                menuGuids.Capacity = (Int32)value;
            }
        }

        public void Update(ContentViewModel<ContentModel> viewModel)
        {
            RecentViewItem item = new RecentViewItem
            {
                ContentID = viewModel.ContentId,
                DisplayValue = viewModel.Model.Location.ToString()
            };

            if (ActualRecentItems.Contains(item))
            {
                ActualRecentItems.Remove(item);
            }
            ActualRecentItems.Add(item);
            Save();
            RecentMenu.Refresh();
        }

        private void ExecuteMethod(String s)
        {
            if (fileService == null)
            {
                fileService = _container.Resolve<IOpenDocumentService>();
            }
            fileService.OpenFromID(s, true);
        }

        [XmlIgnore]
        public AbstractMenuItem RecentMenu
        {
            get
            {
                Int32 i = RecentItems.Count;
                foreach (String guid in menuGuids)
                {
                    recentMenu.Remove(guid);
                    i--;
                }

                menuGuids.Clear();

                for (i = RecentItems.Count; i > 0; i--)
                {
                    Int32 priority = RecentItems.Count - i + 1;
                    String number = "_" + priority.ToString() + " " + RecentItems[i - 1].DisplayValue;
                    menuGuids.Add(recentMenu.Add(new MenuItemViewModel(number, priority, null, recentOpen, null)
                                                     {CommandParameter = RecentItems[i - 1].ContentID}));
                }
                return recentMenu;
            }
        }

        [XmlIgnore]
        public IReadOnlyList<IRecentViewItem> RecentItems => ActualRecentItems.AsReadOnly();
    }
}