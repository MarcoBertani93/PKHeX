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
//using XmlPictureCreation;

namespace PKHeX.PokePic
{

    public partial class ExportForm : Form
    {
        private readonly List<PokePicConfig> _pokePicConfigs = [];
        private readonly PKM _pkm;
        private readonly AsyncDataLoader _asyncDataLoader;

        public ExportForm(AsyncDataLoader asyncDataLoader, PKM pk)
        {
            _pkm = pk;
            _asyncDataLoader = asyncDataLoader;

            StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();

            PreviewBox.SizeMode = PictureBoxSizeMode.CenterImage;
            //LoadFiles();

            _asyncDataLoader.ProcessorLoaded += _asyncDataLoader_ItemLoaded;

            foreach (var namedProcessor in _asyncDataLoader.NamedProcessors)
            {
                var vm = new ListItemViewModel()
                {
                    Text = namedProcessor.Name,
                    Errors = [.. namedProcessor.Errors.Select(err => err.Message)]
                };

                var control = new ListItemControl()
                {
                    ViewModel = vm,
                    Dock = DockStyle.Top
                };

                control.Click += (s, e) =>
                {
                    MessageBox.Show($"Clicked on {namedProcessor.Name}");
                };

                //var config = new PokePicConfig(Path.GetDirectoryName(namedProcessor.Name) ?? "");
                //_pokePicConfigs.Add(config);
                //ConfigList.Items.Add(control);
                listPanel.Controls.Add(control);
                control.BringToFront();
            }
        }

        private void _asyncDataLoader_ItemLoaded(NamedProcessor obj)
        {
            throw new NotImplementedException();
        }

        private void LoadFiles()
        {
            var directories = Directory.GetDirectories("PokePic");

            foreach (var directory in directories)
            {
                var configFileFullPath = Path.Combine(directory, "config.xml");
                if (File.Exists(configFileFullPath))
                {
                    var configDirectoryName = new DirectoryInfo(directory).Name;
                    var config = new PokePicConfig(directory);
                    _pokePicConfigs.Add(config);
                    ConfigList.Items.Add(config.Name);
                }
            }

            if (_pokePicConfigs.Count == 0)
            {
                MessageBox.Show("No valid configuration folders found");
                return;
            }

            ConfigList.SelectedValueChanged += ConfigList_SelectedValueChanged;
            ConfigList.SelectedIndex = 0;
        }

        private void ConfigList_SelectedValueChanged(object? sender, EventArgs e)
        {
            LoadPreview(ConfigList.SelectedIndex);
        }

        async void LoadPreview(int index)
        {
            try
            {
                var config = _pokePicConfigs[index];
                Image image = null;

                var result = await CreateImage(_pkm, config.ConfigFile);

                if (File.Exists(config.PreviewFileJpg))
                {
                    image = Image.FromFile(config.PreviewFileJpg);
                }
                else if (File.Exists(config.PreviewFilePng))
                {
                    image = Image.FromFile(config.PreviewFilePng);
                }
                else if (result.Success)
                {
                    image = result.Bitmap!;
                }
                else
                {
                    PreviewBox.Image = null;
                    return;
                }

                var resized = ResizeToFit(PreviewBox, image);

                int xPad = 0, yPad = 0;
                if (resized.Width < PreviewBox.Width)
                    xPad = (PreviewBox.Width - resized.Width) / 2;
                if (resized.Height < PreviewBox.Height)
                    yPad = (PreviewBox.Height - resized.Height) / 2;

                PreviewBox.Image = resized;
                PreviewBox.Padding = new Padding(xPad, yPad, 0, 0);

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


        static async Task<ProcessResult> CreateImage(PKM pk, string configFile)
        {

            var loaderResult = XmlPictureLoader.LoadXml(configFile);

            var result = loaderResult.Processor.Process();
            //PkmHelper.AddVariables(creator, pk);
            //PkmHelper.AddImages(creator, pk);

            //var result = await creator.Process(configFile);

            return result;
        }

        static async void SaveImage(PKM pk, string configFile)
        {
            var result = await CreateImage(pk, configFile);

            foreach (var error in result.Errors)
                Console.WriteLine($"ERR: {error.Message}");

            var message = string.Join(Environment.NewLine, result.Errors.Select(e => e.Message).ToArray());

            if (result.Success)
            {
                var title = "SUCCESS!";
                if (result.Errors.Length > 0)
                {
                    title += string.Format(" ({0} error{1})", result.Errors.Length, result.Errors.Length > 1 ? "s" : "");
                    message += Environment.NewLine;
                    message += Environment.NewLine;
                    message += "Fix your configuration file for a better result!";
                }
                MessageBox.Show(message, title);
                result.Bitmap!.Save("result.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                var title = string.Format("FAIL! ({0} error{1})", result.Errors.Length, result.Errors.Length > 1 ? "s" : "");
                message += Environment.NewLine;
                message += Environment.NewLine;
                message += "Fix your configuration file and try again";
                MessageBox.Show(message, title);
            }
        }


        private void SaveButton_Click(object sender, EventArgs e)
        {
            var index = ConfigList.SelectedIndex;

            SaveImage(_pkm, _pokePicConfigs[index].ConfigFile);
        }

        private void ExportForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _asyncDataLoader.ProcessorLoaded -= _asyncDataLoader_ItemLoaded;
        }
    }

    class PokePicConfig
    {
        readonly string DirectoryPath;

        public string Name => new DirectoryInfo(DirectoryPath).Name;
        public string ConfigFile => Path.Combine(DirectoryPath, "config.xml");
        public string PreviewFileJpg => Path.Combine(DirectoryPath, "preview.jpg");
        public string PreviewFilePng => Path.Combine(DirectoryPath, "preview.png");

        public PokePicConfig(string directory)
        {
            DirectoryPath = Path.GetFullPath(directory);
        }
    }
}
