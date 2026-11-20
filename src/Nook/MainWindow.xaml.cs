using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Nook;

public partial class MainWindow : Window
{
    private bool _expanded;
    private int _reactionIndex;
    private Storyboard? _breathing;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Left = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width - Width) / 2;
        Top = SystemParameters.WorkArea.Top + 12;
        _breathing = FindResource("Breathe") as Storyboard;
        _breathing?.Begin(this, true);
    }

    private void Shell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is Button || e.OriginalSource is CheckBox)
            return;
        if (e.ClickCount == 2)
        {
            ToggleExpanded();
            e.Handled = true;
        }
        else if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void Character_Click(object sender, MouseButtonEventArgs e)
    {
        if (ReactionSetting.IsChecked != true)
            return;
        string[] reactions = { "Хи-хи! Ещё раз? ✦", "Ой! Щекотно!", "Мне нравится :)", "Я тут, не переживай ♡" };
        string[] mouths = { "ᴗ", "o", "ω", "ᴗ" };
        StatusText.Text = reactions[_reactionIndex % reactions.Length];
        Mouth.Text = mouths[_reactionIndex % mouths.Length];
        _reactionIndex++;
        e.Handled = true;
    }

    private void Toggle_Click(object sender, RoutedEventArgs e) => ToggleExpanded();

    private void ToggleExpanded()
    {
        _expanded = !_expanded;
        Details.Visibility = _expanded ? Visibility.Visible : Visibility.Collapsed;
        Height = _expanded ? 326 : 88;
        ExpandButton.Content = _expanded ? "⌃" : "⌄";
    }

    private void ShowPanel(StackPanel panel)
    {
        DevicesPanel.Visibility = panel == DevicesPanel ? Visibility.Visible : Visibility.Collapsed;
        AgentsPanel.Visibility = panel == AgentsPanel ? Visibility.Visible : Visibility.Collapsed;
        SettingsPanel.Visibility = panel == SettingsPanel ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Devices_Click(object sender, RoutedEventArgs e) => ShowPanel(DevicesPanel);
    private void Agents_Click(object sender, RoutedEventArgs e) => ShowPanel(AgentsPanel);
    private void Settings_Click(object sender, RoutedEventArgs e) => ShowPanel(SettingsPanel);

    private void AlwaysOnTop_Changed(object sender, RoutedEventArgs e)
    {
        if (AlwaysOnTopSetting != null)
            Topmost = AlwaysOnTopSetting.IsChecked == true;
    }

    private void Animation_Changed(object sender, RoutedEventArgs e)
    {
        if (_breathing == null || AnimationSetting == null)
            return;
        if (AnimationSetting.IsChecked == true)
            _breathing.Begin(this, true);
        else
        {
            _breathing.Stop(this);
            CharacterScale.ScaleX = 1;
            CharacterScale.ScaleY = 1;
        }
    }
}
