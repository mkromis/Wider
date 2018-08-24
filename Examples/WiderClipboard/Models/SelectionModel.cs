using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wider.Core.Services;
using WiderClipboard.ViewModels;

namespace WiderClipboard.Models
{
    class SelectionModel : ContentModel
    {
        private readonly IContainer _container;

        public SelectionModel(IContainer container)
        {
            _container = container;
        }

        // Menu command to refersh the cliboard command
        public String[] Refresh()
        {
            IWorkspace workspace = _container.Resolve<IWorkspace>();

            workspace.Documents.Clear();
            return Clipboard.GetDataObject().GetFormats(false);
        }

        // Show the document for clipboard strings.
        public void LoadDocument(String format)
        {
            IWorkspace workspace = _container.Resolve<IWorkspace>();
            try
            {
                Object data = Clipboard.GetData(format);

                if (workspace.Documents.Any(x => x.Title == format))
                {
                    ContentViewModel document = workspace.Documents.Where(x => x.Title == format).First();
                    workspace.ActiveDocument = document;
                    return;
                }

                switch (data)
                {
                    case String stringData:
                        {
                            StringOutputViewModel viewModel = _container.Resolve<StringOutputViewModel>();
                            viewModel.Title = format;
                            viewModel.Content = stringData;
                            workspace.Documents.Add(viewModel);
                            workspace.ActiveDocument = viewModel;
                            return;
                        }
                    case String[] stringArray:
                        {
                            StringOutputViewModel viewModel = _container.Resolve<StringOutputViewModel>();
                            viewModel.Title = format;
                            viewModel.Content = String.Join("\n", stringArray);
                            workspace.Documents.Add(viewModel);
                            workspace.ActiveDocument = viewModel;
                            return;
                        }
                    case System.Windows.Interop.InteropBitmap bitmap:
                        {
                            BitmapViewModel viewModel = _container.Resolve<BitmapViewModel>();
                            viewModel.Title = format;
                            viewModel.ImageSource = bitmap;
                            workspace.Documents.Add(viewModel);
                            workspace.ActiveDocument = viewModel;
                            return;
                        }
                    default:
                        {
                            StringOutputViewModel viewModel = _container.Resolve<StringOutputViewModel>();
                            viewModel.Title = format;
                            viewModel.Content = $"Not processed type of {data.ToString()}";
                            workspace.Documents.Add(viewModel);
                            workspace.ActiveDocument = viewModel;
                            return;
                        }
                }
            }
            catch (COMException e)
            {
                StringOutputViewModel viewModel = _container.Resolve<StringOutputViewModel>();
                viewModel.Title = format;
                viewModel.Content = e.Message;
                workspace.Documents.Add(viewModel);
                workspace.ActiveDocument = viewModel;
            }
        }
    }
}
