using System;
using System.Windows;

namespace Wider.Content.NodeEditor.Events
{
    public class AdornerEventArgs : RoutedEventArgs
    {
        public AdornerEventArgs(RoutedEvent routedEvent, Object source, FrameworkElement adorner) :
            base(routedEvent, source) => Adorner = adorner;

        public FrameworkElement Adorner { get; } = null;
    }

    public delegate void AdornerEventHandler(Object sender, AdornerEventArgs e);
}
