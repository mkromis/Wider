using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiderRibbonDemo.Models;

namespace WiderRibbonDemo.ViewModels
{
    public class NodeEditorViewModel : Wider.Content.NodeEditor.ViewModels.NodeEditorViewModel
    {
        public NodeEditorViewModel(IContainerExtension container) : base (container)
        {
            Model = new EmptyModel();
        }
    }
}
