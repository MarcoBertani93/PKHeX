using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.PokePic
{
    public class ListItemViewModel : INotifyPropertyChanged
    {
        private string _text = string.Empty;
        private List<string> _errors = [];
        private bool _hasErrors;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public List<string> Errors
        {
            get => _errors;
            set
            {
                _errors = value ?? new List<string>();
                HasErrors = _errors.Count > 0;
                OnPropertyChanged();
            }
        }

        public bool HasErrors
        {
            get => _hasErrors;
            private set
            {
                _hasErrors = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
