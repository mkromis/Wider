using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Wider.Content.NodeEditor.Events
{
    /// <summary>
    /// Arguments to the ItemsAdded and ItemsRemoved events.
    /// </summary>
    public class CollectionItemsEventArgs : EventArgs
    {
        public CollectionItemsEventArgs(ICollection items) => Items = items;

        /// <summary>
        /// The list of items that were cleared from the list.
        /// </summary>
        public ICollection Items { get; } = null;
    }
}
