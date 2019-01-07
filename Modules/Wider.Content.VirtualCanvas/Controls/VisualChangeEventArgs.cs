//-----------------------------------------------------------------------
// <copyright file="VirtualCanvas.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace Wider.Content.VirtualCanvas.Controls
{
    public class VisualChangeEventArgs : EventArgs
    {
        public Int32 Added { get; set; }
        public Int32 Removed { get; set; }
        public VisualChangeEventArgs(Int32 added, Int32 removed)
        {
            Added = added;
            Removed = removed;
        }
    }
}
