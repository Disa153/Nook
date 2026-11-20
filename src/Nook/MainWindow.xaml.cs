using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Nook;

public partial class MainWindow : Window
{
    private bool _expanded;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Left = (SystemParameters.WorkArea.Width - Width) / 2 + SystemParameters.WorkArea.Left;
        Top = SystemParameters.WorkArea.Top + 12;
        if (FindResource("Breathe") is Storyboard storyboard)
            storyboard.Begin(this, true);
    }

    private void Shell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is System.Windows.Controls.Button)
            return;
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void Toggle_Click(object sender, RoutedEventArgs e)
    {
        _expanded = !_expanded;
        Details.Visibility = _expanded ? Visibility.Visible : Visibility.Collapsed;
        Height = _expanded ? 150 : 76;
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();
}
