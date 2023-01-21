using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PDFCreatorFromImages.PdfElements;
using PDFCreatorFromImages.Utils;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using ListViewItem = System.Windows.Controls.ListViewItem;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace PDFCreatorFromImages.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private List<PdfImagePage> _pdfElementsCache = new List<PdfImagePage>();
        public static PdfDocument ResultDocument;

        public MainWindow()
        {
            InitializeComponent();
            Title = "PDFCreatorFromImages v" + Constants.VERSION;

            AddButton.Content = new Image
            {
                Source = ResourcesManager.GetImage("add_icon")
            };
            SaveButton.Content = new Image
            {
                Source = ResourcesManager.GetImage("save_icon")
            };
            ClearButton.Content = new Image
            {
                Source = ResourcesManager.GetImage("clear_icon")
            };
            GitButton.Content = new Image
            {
                Source = ResourcesManager.GetImage("git_icon")
            };
        }

        public void AddPdfElementToList(string filePath)
        {
            ListViewItem newImage = new ListViewItem();
            newImage.Content = filePath;
            if (!filePath.EndsWith(".pdf") && !filePath.EndsWith(".png") && !filePath.EndsWith(".jpg") && !filePath.EndsWith(".jpeg"))
            {
                newImage.Background = new SolidColorBrush(Color.FromRgb(255, 125, 125));
            }

            MenuItem deleteButtonFromContextMenu = new MenuItem();
            deleteButtonFromContextMenu.Header = "Delete";
            deleteButtonFromContextMenu.Click += OnContextMenuDeleteButtonClick;

            newImage.ContextMenu = new ContextMenu();
            newImage.ContextMenu.Items.Add(deleteButtonFromContextMenu);

            Pages.Items.Add(newImage);
        }

        /**
         * Clear and unload image cache
         */
        public void ClearPdfElementsCache()
        {
            foreach (PdfImagePage element in _pdfElementsCache)
            {
                element.Dispose();
            }
        }

        /**
         * Create and save PDF from items
         */
        public void SaveDocument()
        {
            ResultDocument = new PdfDocument();
 
            // Create pages
            foreach (ListViewItem page in Pages.Items)
            {
                if (page.Content.ToString().EndsWith(".pdf"))
                {
                    PdfDocument pdfItemDocument = PdfReader.Open(page.Content.ToString(), PdfDocumentOpenMode.Import);

                    foreach (PdfPage pageFromItemFile in pdfItemDocument.Pages)
                    {
                        ResultDocument.AddPage(pageFromItemFile);
                    }

                    pdfItemDocument.Dispose();
                }
                else
                {
                    _pdfElementsCache.Add(new PdfImagePage(page.Content.ToString()));
                    _pdfElementsCache[_pdfElementsCache.Count - 1].DrawPage(ResultDocument.AddPage());
                }
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF (*.pdf)|*.pdf";
            saveFileDialog.FileName = FileUtils.GetFileName(((ListViewItem)Pages.Items.GetItemAt(0)).Content.ToString());

            if (saveFileDialog.ShowDialog().GetValueOrDefault(false)) {
                ResultDocument.Save(saveFileDialog.FileName);
                if (OpenAfterSaveCheckBox.IsChecked.GetValueOrDefault(false))
                    Process.Start(saveFileDialog.FileName);
            }
            ClearPdfElementsCache();
        }

        //**************
        //*** Events ***
        //**************
        private void OnFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files != null)
                {
                    foreach (string file in files)
                    {
                        AddPdfElementToList(file);
                    }
                }
            }
        }

        //**********************
        //*** Buttons events ***
        //**********************
        private void OnAddPdfElementButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.ShowDialog();
            foreach (string filePath in openFileDialog.FileNames)
            {
                AddPdfElementToList(filePath);
            }
        }

        private void OnClearPdfElementsListButtonClick(object sender, RoutedEventArgs e)
        {
            Pages.Items.Clear();
            ClearPdfElementsCache();
        }

        private void OnSaveDocumentButtonClick(object sender, RoutedEventArgs e)
        {
            SaveDocument();
        }

        private void OnOpenGitButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Alemakave/PDFCreatorFromImages");
        }

        //********************************
        //*** ContextMenu items events ***
        //********************************
        /**
         * Delete item from list
         */
        private void OnContextMenuDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            ClearPdfElementsCache();
            Pages.Items.RemoveAt(Pages.SelectedIndex);
        }
    }
}