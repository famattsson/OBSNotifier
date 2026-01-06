using OBSNotifier.Modules.NvidiaLike;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OBSNotifier.Modules.Nvidia_like
{
    /// <summary>
    /// Interaction logic for RecordingIndicator.xaml
    /// </summary>
    public partial class RecordingIndicator : Window
    {
        const double default_window_width = 90;
        const double default_window_height = 90;

        NvidiaCustomAnimationConfig currentParams;
        NvidiaCustomAnimationConfig previousParams;
        readonly NvidiaCustomAnimationConfig defaultParams = new NvidiaCustomAnimationConfig();

        NvidiaNotification owner;
        private int addDataHash = -1;

        bool IsPositionedOnTop { get => (NvidiaNotification.Positions)owner.ModuleSettings.Option == NvidiaNotification.Positions.TopRight; }


        public RecordingIndicator(NvidiaNotification module)
        {
            currentParams = new NvidiaCustomAnimationConfig();
            owner = module;
            InitializeComponent();
        }

        void UpdateParameters()
        {

            if (owner.ModuleSettings.AdditionalData != null && owner.ModuleSettings.AdditionalData.GetHashCode() != addDataHash)
            {
                addDataHash = owner.ModuleSettings.AdditionalData.GetHashCode();

                // Recreate but remember preview state
                bool prev = currentParams.IsPreviewNotif;
                currentParams = new NvidiaCustomAnimationConfig()
                {
                    IsPreviewNotif = prev,
                };
                Utils.ConfigParseString(owner.ModuleSettings.AdditionalData, ref currentParams);
            }
            // General params
            currentParams.Duration = owner.ModuleSettings.OnScreenTime;
            currentParams.IsOnRightSide = (NvidiaNotification.Positions)owner.ModuleSettings.Option == NvidiaNotification.Positions.TopRight;

            UtilsWinApi.SetWindowIgnoreMouse(this.GetHandle(), currentParams.ClickThrough && !currentParams.ShowQuickActions);


            // Sizes

            Width = Math.Ceiling(currentParams.LineWidth * currentParams.Scale);
            Height = Math.Ceiling(default_window_height * currentParams.Scale);

            // Position
            var pe = (NvidiaNotification.Positions)owner.ModuleSettings.Option;
            var anchor = (Utils.AnchorPoint)Enum.Parse(typeof(Utils.AnchorPoint), pe.ToString());
            Point pos = Utils.GetWindowPosition(anchor, new Size(Width, Height), owner.ModuleSettings.Offset);

            Left = pos.X;
            Top = pos.Y;
        }


        public void ShowIndicator()
        {
            if (currentParams.IsPreviewNotif)
                return;

            previousParams = currentParams.Duplicate();
            UpdateParameters();

            if (!IsPositionedOnTop)
            {
                var delta = Height - ActualHeight;
                if (delta > 0)
                    Top += delta;
            }

            Show();
        }
    }
}
