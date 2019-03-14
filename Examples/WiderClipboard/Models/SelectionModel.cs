using Prism.Ioc;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Wider.Content.TextDocument;
using Wider.Content.TextDocument.ViewModels;
using Wider.Content.TextDocument.Views;
using Wider.Core.Services;
using WiderClipboard.ViewModels;

namespace WiderClipboard.Models
{
    internal class SelectionModel : ContentModel
    {
        private readonly IContainerExtension _container;

        public SelectionModel(IContainerExtension container) => _container = container;

        // Menu command to refersh the cliboard command
        public String[] Refresh()
        {
            IWorkspace workspace = _container.Resolve<IWorkspace>();

            workspace.Documents.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();

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
                    ContentViewModel<ContentModel> document = workspace.Documents.Where(x => x.Title == format).First();
                    workspace.ActiveDocument = document;
                    return;
                }

                switch (data)
                {
                    case String stringData:
                        {
                            TextDocument(format, workspace, stringData);
                            return;
                        }
                    case String[] stringArray:
                        {
                            TextDocument(format, workspace, String.Join("\n", stringArray));
                            return;
                        }
                    case System.IO.MemoryStream memory:
                        {
                            HexViewerViewModel viewModel = _container.Resolve<HexViewerViewModel>();
                            viewModel.Title = format;
                            viewModel.MemoryStreamItem = memory;
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

        /// <summary>
        /// Open document from based on string data. Barrowed from Wider.Content.TextDocument
        /// </summary>
        /// <param name="format"></param>
        /// <param name="workspace"></param>
        /// <param name="stringData"></param>
        private void TextDocument(String format, IWorkspace workspace, String stringData)
        {
            TextViewModel vm = _container.Resolve<TextViewModel>();
            TextModel model = _container.Resolve<TextModel>();
            TextView view = _container.Resolve<TextView>();

            // set viewmodel to model?, copied from Wider.Content.TextDocument
            view.DataContext = model;

            //Clear the undo stack
            model.Document.UndoStack.ClearAll();
            model.Document.Text = stringData;
            model.IsDirty = false;

            //Set the model and view
            vm.Model = model;
            vm.View = view;
            vm.Title = format;

            workspace.Documents.Add(vm);
            workspace.ActiveDocument = vm;
        }
    }
}
