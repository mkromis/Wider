using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WiderRibbonDemo.Models
{
    internal class ThemeItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                "Default",
                "Light",
                "Dark",
                "Blue",
            };
        }
    }
}