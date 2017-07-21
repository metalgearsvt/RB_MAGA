using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MetalBuddy.etc;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MetalBuddy
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        public SettingsWindow()
        {
            InheritanceBehavior = InheritanceBehavior.SkipToThemeNext;
            InitializeComponent();
            TitleText.Text = Plugin.Constants.ACRONYM;
        }

        public void updateSettings()
        {
            // Add all interface elements here; the UI needs to reflect the current Plugin.Variables.settings.
            EnableOverlayCB.IsChecked = Plugin.Variables.settings.EnableOverlay;
            ForegroundOnly.IsChecked = Plugin.Variables.settings.OnlyForeground;
            EnableStats.IsChecked = Plugin.Variables.settings.EnableStats;

            ShowAetherCurrents.IsChecked = Plugin.Variables.settings.ShowAetherCurrents;

            OverlayXPosBox.Text = Plugin.Variables.settings.OverlayXPos.ToString();
            OverlayYPosBox.Text = Plugin.Variables.settings.OverlayYPos.ToString();

            OverlayAlert.Text = Plugin.Variables.settings.AlertObject;
        }

        // Window dragging.
        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        // Close button.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void EnableOverlayCB_Checked(object sender, RoutedEventArgs e)
        {
            Plugin.Variables.settings.EnableOverlay = true;
        }

        private void EnableOverlayCB_Unchecked(object sender, RoutedEventArgs e)
        {
            Plugin.Variables.settings.EnableOverlay = false;
        }

        private void ShowAetherCurrents_Checked(object sender, RoutedEventArgs e)
        {
            Plugin.Variables.settings.ShowAetherCurrents = true;
        }

        private void ShowAetherCurrents_Unchecked(object sender, RoutedEventArgs e)
        {
            Plugin.Variables.settings.ShowAetherCurrents = false;
        }

        private void ForegroundOnly_Checked(object sender, RoutedEventArgs e)
        {
            Plugin.Variables.settings.OnlyForeground = true;
        }

        private void ForegroundOnly_Unchecked(object sender, RoutedEventArgs e)
        {
            Plugin.Variables.settings.OnlyForeground = false;
        }

        private void OverlayXPosBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32 res;
            int.TryParse(OverlayXPosBox.Text, out res);
            Plugin.Variables.settings.OverlayXPos = res;
        }

        private void OverlayYPosBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32 res;
            int.TryParse(OverlayYPosBox.Text, out res);
            Plugin.Variables.settings.OverlayYPos = res;
        }

        private void EnableStats_Checked(object sender, RoutedEventArgs e)
        {
            Plugin.Variables.settings.EnableStats = true;
        }

        private void EnableStats_Unchecked(object sender, RoutedEventArgs e)
        {
            Plugin.Variables.settings.EnableStats = false;
        }

        private void OverlayAlert_TextChanged(object sender, TextChangedEventArgs e)
        {
            Plugin.Variables.settings.AlertObject = OverlayAlert.Text;
        }

        private void AlertYPosBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32 res;
            int.TryParse(AlertYPosBox.Text, out res);
            Plugin.Variables.settings.AlertYPos = res;
        }

        private void AlertXPosBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32 res;
            int.TryParse(AlertXPosBox.Text, out res);
            Plugin.Variables.settings.AlertXPos = res;
        }

        private void AttackablePlayers_Checked(object sender, RoutedEventArgs e)
        {
            Plugin.Variables.settings.ShowTargetablePlayers = true;
        }

        private void AttackablePlayers_Unchecked(object sender, RoutedEventArgs e)
        {
            Plugin.Variables.settings.ShowTargetablePlayers = false;
        }
    }
}
