using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.PokePic;
using PKHeX.PokePic.Helpers;

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

            var exportPic = new ToolStripMenuItem("Export to PokéPic");
            exportPic.Click += MainMenuSavePic;
            modMenu.DropDownItems.Add(exportPic);
            modMenu.DropDownItems.Add(new ToolStripMenuItem("Import from PokéPic"));

            tools.DropDownItems.Add(modMenu);

            /*
            if (items.Find(ParentMenuParent, false)[0] is not ToolStripDropDownItem tools)
                return;
            var toolsitems = tools.DropDownItems;
            var modmenusearch = toolsitems.Find(ParentMenuName, false);
            var modmenu = GetModMenu(tools, modmenusearch);
            AddPluginControl(modmenu);*/
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


        private void MainMenuSavePic(object? sender, EventArgs e)
        {
            if (PKMEditor.Data == null)
            {
                MessageBox.Show("No Pokémon loaded.");
                return;
            }

            var pk = PKMEditor.Data;


            new ExportForm(dataLoader, pk).ShowDialog();

        }
    }
}
