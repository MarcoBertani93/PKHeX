using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public ExportForm(AsyncDataLoader asyncDataLoader, PKM pkm)
        {
            _pkmVars = PkmHelper.GetVariables(pkm);
            _pkmImgs = PkmHelper.GetImages(pkm);

            _asyncDataLoader = asyncDataLoader;

            StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();

            PreviewBox.SizeMode = PictureBoxSizeMode.CenterImage;
            SaveButton.Enabled = false;

            _asyncDataLoader.ProcessorLoaded += _asyncDataLoader_ItemLoaded;

            foreach (var namedProcessor in _asyncDataLoader.NamedProcessors)
            {
                AddProcessor(namedProcessor);
            }
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

        private void _asyncDataLoader_ItemLoaded(NamedProcessor obj)
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

            float dest_width = 0, dest_height = 0;

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
                Rectangle src_rect = new Rectangle(0, 0, img.Width, img.Height);
                Rectangle dest_rect = new Rectangle(0, 0, resized.Width, resized.Height);
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

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string filename = "result.png";
            try
            {
                selectedListItem?.Image.Save(filename);
                MessageBox.Show($"Picture saved as {filename}", "SUCCESS");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR"); ;
            }
            
        }

        private void ExportForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _asyncDataLoader.ProcessorLoaded -= _asyncDataLoader_ItemLoaded;

            foreach (var listItemControl in listItemViewModels)
                listItemControl.PropertyChanged -= ListItem_PropertyChanged;
            
        }
    }

    
}
