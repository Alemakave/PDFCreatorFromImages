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
        private List<PdfImagePage> _imagesCache = new List<PdfImagePage>();
        public static PdfDocument ResultDocument;

        public MainWindow()
        {
            InitializeComponent();
            Title = "PDFCreatorFromImages v" + Constants.VERSION;

            Add.Content = new Image
            {
                Source = ResourcesManager.GetImage("add")
            };
            Save.Content = new Image
            {
                Source = ResourcesManager.GetImage("save")
            };
            Clear.Content = new Image
            {
                Source = ResourcesManager.GetImage("clear")
            };
            Git.Content = new Image
            {
                Source = ResourcesManager.GetImage("git")
            };
        }

        private void OnAddImageButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.ShowDialog();
            foreach (string filePath in openFileDialog.FileNames)
            {
                AddImage(filePath);
            }
        }

        private void OnClearImagesListButtonClick(object sender, RoutedEventArgs e)
        {
            Pages.Items.Clear();
            ClearImageCache();
        }

        private void OnSaveDocumentButtonClick(object sender, RoutedEventArgs e)
        {
            SaveDocument();
        }

        public void AddImage(string filePath)
        {
            ListViewItem newImage = new ListViewItem();
            newImage.Content = filePath;
            if (!filePath.EndsWith(".pdf") && !filePath.EndsWith(".png") && !filePath.EndsWith(".jpg") && !filePath.EndsWith(".jpeg"))
            {
                newImage.Background = new SolidColorBrush(Color.FromRgb(255, 95, 95));
            }
            Pages.Items.Add(newImage);
        }

        /**
         * Clear and unload image cache
         */
        public void ClearImageCache()
        {
            foreach (PdfImagePage image in _imagesCache)
            {
                image.Dispose();
            }
        }

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
                    _imagesCache.Add(new PdfImagePage(page.Content.ToString()));
                    _imagesCache[_imagesCache.Count - 1].DrawPage(ResultDocument.AddPage());
                }
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF (*.pdf)|*.pdf";
            saveFileDialog.FileName = FileUtils.GetFileName(((ListViewItem)Pages.Items.GetItemAt(0)).Content.ToString());

            if (saveFileDialog.ShowDialog().GetValueOrDefault(false)) {
                ResultDocument.Save(saveFileDialog.FileName);
                if (OpenAfterSave.IsChecked.GetValueOrDefault(false))
                    Process.Start(saveFileDialog.FileName);
            }
            ClearImageCache();
        }

        private void OnFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files != null)
                {
                    foreach (string file in files)
                    {
                        AddImage(file);
                    }
                }
            }
        }

        private void OnOpenGitButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Alemakave/PDFCreatorFromImages");
        }
    }
}