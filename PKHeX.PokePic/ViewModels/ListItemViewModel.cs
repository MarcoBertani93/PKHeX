using PKHeX.Core;
using PKHeX.PokePic.Helpers;
using PKHeX.PokePic.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XmlPictureCreation;
using XmlPictureCreation.Aliases;

namespace PKHeX.PokePic
{
    public class ListItemViewModel : INotifyPropertyChanged
    {
        private readonly LoadResult loadResult;
        private ProcessResult? processResult;
        private readonly VariableCollection _pkmVars;
        private readonly ImageDictionary _pkmImgs;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }
        string _text = string.Empty;

        public Bitmap? Image => processResult?.Bitmap;  // Can be null

        public bool Processing
        {
            get => _processing;
            set
            {
                _processing = value;
                OnPropertyChanged();
            }
        }
        bool _processing;

        public bool ProcessDone => processResult != null;

        public bool LoadSuccess => loadResult.Success;
        public string[] LoadErrors => [.. loadResult.Errors.Select(e => e.Message)];
        public bool HasLoadErrors => loadResult.Errors.Length > 0;

        public bool ProcessSuccess => processResult?.Success ?? false;
        public string[] ProcessErrors => processResult?.Errors.Select(e => e.Message).ToArray() ?? [];
        public bool HasProcessErrors => processResult?.Errors.Length > 0;

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }
        bool _selected;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ListItemViewModel(NamedProcessor namedProcessor, VariableCollection vars, ImageDictionary imgs)
        {
            loadResult = namedProcessor.LoadResult;
            Text = namedProcessor.Name;
            _pkmImgs = imgs;
            _pkmVars = vars;
        }

        public void Select() => Selected = true;
        public void Unselect() => Selected = false;


        public async Task LoadPictureAsync()
        {
            // Failed during loading, can't process
            if (!loadResult.Success)
                return;

            // Already processed
            if (processResult?.Success == true)
                return;

            Processing = true;
            try
            {
                processResult = await loadResult.Processor!.ProcessAsync(_pkmVars, _pkmImgs);
            }
            finally
            {
                Processing = false;
            }

        }
    }
}
