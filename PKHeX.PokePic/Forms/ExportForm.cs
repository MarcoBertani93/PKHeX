using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.PokePic.Helpers;
using PKHeX.PokePic.ViewModels;
using XmlPictureCreation;
using XmlPictureCreation.Aliases;
//using XmlPictureCreation;

namespace PKHeX.PokePic
{
    public partial class ExportForm : Form
    {
        private readonly VariableCollection _pkmVars;
        private readonly ImageDictionary _pkmImgs;
        private readonly AsyncDataLoader _asyncDataLoader;
        private readonly List<ListItemViewModel> listItemViewModels = [];
        ListItemViewModel? selectedListItem;

        private readonly PKM _previewPkm;
        private readonly PKM[]? _boxPkm;


        // This is true when trying to export the entire box and not the selected Pokémon
        private readonly bool _isExportingBox;
        
        public ExportForm(AsyncDataLoader asyncDataLoader, PKM previewPkm)
        {
            _previewPkm = previewPkm;
            _pkmVars = PkmHelper.GetVariables(previewPkm);
            _pkmImgs = PkmHelper.GetImages(previewPkm);

            _asyncDataLoader = asyncDataLoader;

            StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();

            PreviewBox.SizeMode = PictureBoxSizeMode.CenterImage;
            SaveButton.Enabled = false;

            _asyncDataLoader.ProcessorLoaded += AsyncDataLoader_ItemLoaded;

            bool shouldSelect = true;
            foreach (var namedProcessor in _asyncDataLoader.NamedProcessors)
            {
                AddProcessor(namedProcessor);
                shouldSelect = false;
            }
        }

        // Takes as preview pkm the first valid box
        public ExportForm(AsyncDataLoader asyncDataLoader, PKM[] boxPkm) : this(asyncDataLoader, boxPkm.First(p => p.Species != 0))
        {
            _isExportingBox = true;

            _boxPkm = boxPkm;

            SaveButton.Text = "Export Box";
        }

        private void ExportForm_Load(object sender, EventArgs e)
        {
            listItemViewModels[0]?.Select();
        }

        private void AddProcessor(NamedProcessor namedProcessor)
        {
            var vm = new ListItemViewModel(namedProcessor, _pkmVars, _pkmImgs);
            vm.PropertyChanged += ListItem_PropertyChanged;
            listItemViewModels.Add(vm);

            var control = new ListItemControl(vm);

            listPanel.Controls.Add(control);
            control.BringToFront();
        }

        private void ListItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not ListItemViewModel vm)
                return;

            if (e.PropertyName == nameof(ListItemViewModel.Selected))
            {
                // If this is selected, unselect all the others
                if (!vm.Selected)
                    return;

                selectedListItem = vm;

                foreach (var item in listItemViewModels)
                {
                    if (item != vm)
                        item.Unselect();
                }
            }

            if (selectedListItem == vm)
                LoadPreview(vm.Image);


        }

        private void AsyncDataLoader_ItemLoaded(NamedProcessor obj)
        {
            AddProcessor(obj);
        }


        private void ConfigList_SelectedValueChanged(object? sender, EventArgs e)
        {
            //LoadPreview(ConfigList.SelectedIndex);
        }


        void LoadPreview(Image? preview)
        {
            try
            {
                if (preview == null)
                {
                    PreviewBox.Image = null;
                    SaveButton.Enabled = false;
                    return;
                }

                var resized = ResizeToFit(PreviewBox, preview);

                int xPad = 0, yPad = 0;
                if (resized.Width < PreviewBox.Width)
                    xPad = (PreviewBox.Width - resized.Width) / 2;
                if (resized.Height < PreviewBox.Height)
                    yPad = (PreviewBox.Height - resized.Height) / 2;

                PreviewBox.Image = resized;
                PreviewBox.Padding = new Padding(xPad, yPad, 0, 0);
                SaveButton.Enabled = true;

                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            PreviewBox.Image = null;
        }

        static public Image ResizeToFit(PictureBox pbox, Image img)
        {
            pbox.SizeMode = PictureBoxSizeMode.Normal;
            bool img_is_wider = (float)img.Width / img.Height > (float)pbox.Width / pbox.Height;

            float dest_width;
            float dest_height;

            if (img_is_wider)
            {
                dest_width = pbox.Width;
                dest_height = (float)img.Height / (float)img.Width * pbox.Width;
            }
            else
            {
                dest_width = (float)img.Width / (float)img.Height * pbox.Height;
                dest_height = pbox.Height;
            }

            var resized = new Bitmap((int)dest_width, (int)dest_height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            using (var g = Graphics.FromImage(resized))
            {
                Rectangle src_rect = new(0, 0, img.Width, img.Height);
                Rectangle dest_rect = new(0, 0, resized.Width, resized.Height);
                g.DrawImage(img, dest_rect, src_rect, GraphicsUnit.Pixel);
            }
            return resized;
        }

        // Mostly copied form https://stackoverflow.com/questions/16822138/fit-image-into-picturebox
        static public Image ResizeToFill(PictureBox pbox, Image img)
        {
            pbox.SizeMode = PictureBoxSizeMode.Normal;
            bool source_is_wider = (float)img.Width / img.Height > (float)pbox.Width / pbox.Height;

            var resized = new Bitmap(pbox.Width, pbox.Height);
            using (var g = Graphics.FromImage(resized))
            {
                var dest_rect = new Rectangle(0, 0, pbox.Width, pbox.Height);
                Rectangle src_rect;

                if (source_is_wider)
                {
                    float size_ratio = (float)pbox.Height / img.Height;
                    int sample_width = (int)(pbox.Width / size_ratio);
                    src_rect = new Rectangle((img.Width - sample_width) / 2, 0, sample_width, img.Height);
                }
                else
                {
                    float size_ratio = (float)pbox.Width / img.Width;
                    int sample_height = (int)(pbox.Height / size_ratio);
                    src_rect = new Rectangle(0, (img.Height - sample_height) / 2, img.Width, sample_height);
                }

                g.DrawImage(img, dest_rect, src_rect, GraphicsUnit.Pixel);
            }

            return resized;
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (_isExportingBox)
                await ExportSingleBox();
            else
                ExportSinglePkm();
        }

        private void ExportSinglePkm()
        {
            // This method takes advantage of the already loaded preview and saves that as picture
            try
            {
                if (TryGetSaveFile(_previewPkm.FileNameWithoutExtension + ".png", out string filename))
                {
                    selectedListItem?.Image.Save(filename);
                    MessageBox.Show($"Picture saved as {filename}", "SUCCESS");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR"); ;
            }
        }

        private async Task ExportSingleBox()
        {
            // This method has to process all pokemons from the box through the processor
            if (!TryGetFolder(out var folder))
                return;

            try
            {
                foreach (var pkm in _boxPkm)
                {
                    var pkmVars = PkmHelper.GetVariables(pkm);
                    var pkmImgs = PkmHelper.GetImages(pkm);

                    var result = await selectedListItem!.Processor!.ProcessAsync(pkmVars, pkmImgs);

                    if (!result.Success)
                        continue;

                    var filePath = Path.Combine(folder, pkm.FileNameWithoutExtension + ".png");

                    result.Bitmap!.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }
                MessageBox.Show($"Box exported successfully to {folder}", "SUCCESS");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR");
            }
        }

        // Copied from PKHeX.WinForms
        private bool TryGetFolder([NotNullWhen(true)] out string? folder)
        {
            using var fbd = new FolderBrowserDialog();
            fbd.Description = "Select a folder to export the boxes to.";
            fbd.ShowNewFolderButton = true;
            var result = fbd.ShowDialog(this);
            folder = fbd.SelectedPath;
            return result == DialogResult.OK;
        }

        private bool TryGetSaveFile(string defaultFileName, [NotNullWhen(true)] out string? file)
        {
            using var saveFileDialog = new SaveFileDialog
            {
                Title = "Export as PNG picture",
                Filter = "File PNG (*.png)|*.png",
                DefaultExt = "png",
                AddExtension = true,
                FileName = defaultFileName
            };

            
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                file = saveFileDialog.FileName;
                return true;
            }
            file = null;
            return false;
        }

        /*
        private bool TryGetOpenFile([NotNullWhen(true)] out string? file)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Title = "Seleziona un file PNG",
                Filter = "File PNG (*.png)|*.png",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                // Ad esempio, carico l'immagine in una PictureBox
                pictureBox1.Image = Image.FromFile(filePath);
            }
            return true;
        }
        */

        private void ExportForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _asyncDataLoader.ProcessorLoaded -= AsyncDataLoader_ItemLoaded;

            foreach (var listItemControl in listItemViewModels)
                listItemControl.PropertyChanged -= ListItem_PropertyChanged;

        }

        
    }


}
