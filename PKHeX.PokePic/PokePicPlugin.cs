using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.PokePic;
using PKHeX.PokePic.Helpers;
using PKHeX.PokePic.Properties;

namespace PKHeX.WinForms
{

    public class PokePicPlugin : IPlugin
    {
        public string Name => "PokePic";

        public int Priority => 100;

        public ISaveFileProvider SaveFileEditor { get; private set; } = null!;
        protected IPKMView PKMEditor { get; private set; } = null!;

        private readonly AsyncDataLoader dataLoader = new();

        public void Initialize(params object[] args)
        {
            SaveFileEditor = (ISaveFileProvider)(
                 Array.Find(args, z => z is ISaveFileProvider)
                 ?? throw new Exception("Null ISaveFileProvider")
             );
            PKMEditor = (IPKMView)(
                Array.Find(args, z => z is IPKMView) ?? throw new Exception("Null IPKMView")
            );
            var menu = (ToolStrip)(
                Array.Find(args, z => z is ToolStrip) ?? throw new Exception("Null ToolStrip")
            );

            

            LoadMenuStrip(menu);
        }


        private void LoadMenuStrip(ToolStrip menuStrip)
        {
            var items = menuStrip.Items;
            if (items.Find("Menu_Tools", false)[0] is not ToolStripDropDownItem tools)
                return;

            var modMenu = new ToolStripMenuItem("PokéPic") { Name = "PokePic_Menu" };

            var exportPkm = new ToolStripMenuItem("Export to PokéPic")
            {
                Image = Resources.export
            };
            exportPkm.Click += MainMenuExportPkm;

            var exportBox = new ToolStripMenuItem("Export Box to PokéPic")
            {
                Image = Resources.exportbox
            };
            exportBox.Click += MainMenuExportBox;

            var importPkm = new ToolStripMenuItem("Import from a PokéPic")
            {
                Image = Resources.import
            };
            importPkm.Click += MainMenuImportPkm;

            modMenu.Image = Resources.pikapic;
            modMenu.DropDownItems.Add(exportPkm);
            modMenu.DropDownItems.Add(exportBox);
            modMenu.DropDownItems.Add(importPkm);

            tools.DropDownItems.Add(modMenu);
        }


        public virtual void NotifySaveLoaded()
        {
            Console.WriteLine($"{Name} was notified that a Save File was just loaded.");
        }

        public virtual bool TryLoadFile(string filePath)
        {
            Console.WriteLine(
                $"{Name} was provided with the file path, but chose to do nothing with it."
            );
            return false; // no action taken
        }

        private void MainMenuExportPkm(object? sender, EventArgs e)
        {
            if (PKMEditor.Data == null)
            {
                MessageBox.Show("No Pokémon loaded.");
                return;
            }

            var pk = PKMEditor.Data;

            new ExportForm(dataLoader, pk).ShowDialog();
        }

        private void MainMenuExportBox(object? sender, EventArgs e)
        {
            var currentBox = SaveFileEditor.CurrentBox;
            var pkms = SaveFileEditor.SAV.GetBoxData(currentBox);

            if (pkms is null || pkms.Length == 0 || !pkms.Any(p => p.Species != 0))
            {
                MessageBox.Show("The selected box is empty.");
                return;
            }

            new ExportForm(dataLoader, pkms).ShowDialog();
        }

        private void MainMenuImportPkm(object? sender, EventArgs e)
        {
        }
                
    }
}
