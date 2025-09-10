using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PKHeX.PokePic
{
    public partial class ListItemControl : UserControl
    {
        private ListItemViewModel _viewModel;
        private readonly ToolTip _tooltip;

        public ListItemViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                if (_viewModel != null)
                    _viewModel.PropertyChanged -= OnViewModelPropertyChanged;

                _viewModel = value;

                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged += OnViewModelPropertyChanged;
                    UpdateUI();
                }
            }
        }

        public ListItemControl()
        {
            InitializeComponent();
            _tooltip = new()
            {
                AutoPopDelay = 5000,
                ShowAlways = true
            };

            lblText.Click += (s, e) => OnClick(e);
            pictureBoxAlert.Click += (s, e) => OnClick(e);

            lblText.MouseEnter += (s, e) => OnMouseEnter(e);
            pictureBoxAlert.MouseEnter += (s, e) => OnMouseEnter(e);

        }

        protected override void OnClick(EventArgs e)
        {
            OnMouseLeave(e);
            base.OnClick(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            BackColor = SystemColors.ControlLight;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseEnter(e);
            BackColor = Parent.BackColor;
        }


        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnViewModelPropertyChanged(sender, e)));
                return;
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_viewModel == null) return;

            lblText.Text = _viewModel.Text;
            pictureBoxAlert.Visible = _viewModel.HasErrors;

            if (_viewModel.HasErrors && _viewModel.Errors.Count > 0)
            {
                var errorText = string.Join(Environment.NewLine + "• ", _viewModel.Errors);
                _tooltip.SetToolTip(pictureBoxAlert, "Errori:" + Environment.NewLine + "• " + errorText);
            }
            else
            {
                _tooltip.SetToolTip(pictureBoxAlert, "");
            }
        }


       
    }
}
