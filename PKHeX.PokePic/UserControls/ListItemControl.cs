using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PKHeX.PokePic
{
    public partial class ListItemControl : UserControl
    {
        private readonly ToolTip _tooltip;

        public ListItemViewModel ViewModel { get; private set; }

        public ListItemControl(ListItemViewModel viewModel)
        {
            InitializeComponent();

            // To stack constrols inside the panel
            Dock = DockStyle.Top;
            
            _tooltip = new()
            {
                AutoPopDelay = 30000,    // Time to disappear
                ShowAlways = true
            };

            lblText.Click += (s, e) => OnClick(e);
            pictureBoxAlert.Click += (s, e) => OnClick(e);

            lblText.MouseEnter += (s, e) => OnMouseEnter(e);
            pictureBoxAlert.MouseEnter += (s, e) => OnMouseEnter(e);

            // Last action to update the UI
            ViewModel = viewModel;
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            UpdateUI();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (ViewModel != null)
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        protected override async void OnClick(EventArgs e)
        {
            OnMouseLeave(e);
            ViewModel.Select();
            await ViewModel.LoadPictureAsync();
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
            if (ViewModel == null)
                return;

            lblText.Text = ViewModel.Text;
            pictureBoxAlert.Visible = true;
            pictureBoxLoading.Visible = ViewModel.Processing;

            BorderStyle = ViewModel.Selected ? BorderStyle.FixedSingle : BorderStyle.None;

            if (!ViewModel.LoadSuccess || (ViewModel.ProcessDone && !ViewModel.ProcessSuccess))
            {
                pictureBoxAlert.Image = Properties.Resources.warn;
            }
            else if (ViewModel.HasLoadErrors || ViewModel.HasProcessErrors)
            {
                pictureBoxAlert.Image = Properties.Resources.hint;
            }
            else
            {
                pictureBoxAlert.Image = Properties.Resources.valid;
            }

            StringBuilder sb = new();
            if (ViewModel.HasLoadErrors && ViewModel.LoadErrors.Length > 0)
            {
                sb.AppendLine("Loading errors:");
                sb.AppendLine();
                foreach (var error in ViewModel.LoadErrors)
                    sb.AppendLine("• " + error);
                sb.AppendLine();
            }

            if (ViewModel.HasProcessErrors && ViewModel.ProcessErrors.Length > 0)
            {
                sb.AppendLine("Processing errors:");
                sb.AppendLine();
                foreach (var error in ViewModel.ProcessErrors)
                    sb.AppendLine("• " + error);
                sb.AppendLine();
            }

            var text = sb.ToString();

            if (string.IsNullOrWhiteSpace(text))
                text = "No errors";

            _tooltip.SetToolTip(pictureBoxAlert, text);

        }



    }
}
