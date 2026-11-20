using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
    private bool _detecting;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Left = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width - Width) / 2;
        double targetTop = SystemParameters.WorkArea.Top + 12;
        Top = targetTop - 44;
        Opacity = 0;

        _breathing = FindResource("Breathe") as Storyboard;
        if (AnimationSetting.IsChecked == true)
            _breathing?.Begin(this, true);

        Animate(this, Window.TopProperty, targetTop, 430);
        Animate(this, Window.OpacityProperty, 1, 430);
        await RefreshMiceAsync();
    }

    private static void Animate(DependencyObject target, DependencyProperty property,
                                double to, int durationMilliseconds)
    {
        var animation = new DoubleAnimation
        {
            To = to,
            Duration = TimeSpan.FromMilliseconds(durationMilliseconds),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        if (target is Window window)
        {
            double from = property == Window.HeightProperty ? window.Height
                        : property == Window.TopProperty ? window.Top : window.Opacity;
            animation.From = from;
            animation.Completed += (_, _) => window.SetValue(property, to);
            window.BeginAnimation(property, animation);
        }
        else if (target is UIElement element)
        {
            animation.From = element.Opacity;
            animation.Completed += (_, _) => element.SetValue(property, to);
            element.BeginAnimation(property, animation);
        }
    }

    private void Island_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Clicking the little character is an interaction, never a drag.
        if (e.OriginalSource is DependencyObject visual && Avatar.IsAncestorOf(visual))
            return;
        if (e.OriginalSource is Button || e.OriginalSource is CheckBox)
            return;

        if (e.ClickCount == 2)
        {
            ToggleExpanded();
            e.Handled = true;
            return;
        }

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            try { DragMove(); } catch (InvalidOperationException) { }
        }
    }

    private void Avatar_MouseEnter(object sender, MouseEventArgs e)
    {
        if (ReactionSetting?.IsChecked == true)
            StatusText.Text = "О, привет! ♡";
    }

    private void Avatar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (ReactionSetting.IsChecked != true)
            return;

        string[] reactions =
        {
            "Хи-хи! Щекотно ♡",
            "Я рад тебя видеть!",
            "Ещё раз? ✦",
            "Ура! Мы вместе."
        };

        StatusText.Text = reactions[_reactionIndex % reactions.Length];
        Smile.Visibility = _reactionIndex % 2 == 0 ? Visibility.Visible : Visibility.Collapsed;
        _reactionIndex++;
        e.Handled = true;
    }

    private void Expand_Click(object sender, RoutedEventArgs e) => ToggleExpanded();

    private void ToggleExpanded()
    {
        _expanded = !_expanded;
        if (_expanded)
        {
            ExpandedContent.Visibility = Visibility.Visible;
            Animate(this, HeightProperty, 435, 410);
            Animate(ExpandedContent, OpacityProperty, 1, 350);
            StatusText.Text = "Что посмотрим?";
        }
        else
        {
            Animate(this, HeightProperty, 88, 400);
            var fade = new DoubleAnimation(0, TimeSpan.FromMilliseconds(180));
            fade.Completed += (_, _) =>
            {
                if (!_expanded)
                    ExpandedContent.Visibility = Visibility.Collapsed;
            };
            ExpandedContent.BeginAnimation(OpacityProperty, fade);
            StatusText.Text = "Я рядом · готов помогать";
        }
    }

    private void ShowPanel(StackPanel panel, double height)
    {
        DevicesPanel.Visibility = panel == DevicesPanel ? Visibility.Visible : Visibility.Collapsed;
        AgentsPanel.Visibility = panel == AgentsPanel ? Visibility.Visible : Visibility.Collapsed;
        SettingsPanel.Visibility = panel == SettingsPanel ? Visibility.Visible : Visibility.Collapsed;
        if (_expanded)
            Animate(this, HeightProperty, height, 340);
    }

    private void Devices_Click(object sender, RoutedEventArgs e) => ShowPanel(DevicesPanel, 435);
    private void Agents_Click(object sender, RoutedEventArgs e) => ShowPanel(AgentsPanel, 275);
    private void Settings_Click(object sender, RoutedEventArgs e) => ShowPanel(SettingsPanel, 330);

    private void TopmostSetting_Changed(object sender, RoutedEventArgs e)
    {
        if (TopmostSetting != null)
            Topmost = TopmostSetting.IsChecked == true;
    }

    private void AnimationSetting_Changed(object sender, RoutedEventArgs e)
    {
        if (_breathing is null || AnimationSetting is null)
            return;

        if (AnimationSetting.IsChecked == true)
            _breathing.Begin(this, true);
        else
        {
            _breathing.Stop(this);
            AvatarScale.ScaleX = 1;
            AvatarScale.ScaleY = 1;
        }
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e) => await RefreshMiceAsync();

    private async Task RefreshMiceAsync()
    {
        if (_detecting)
            return;

        _detecting = true;
        MouseName.Text = "Проверяем устройства Windows…";
        MouseIndicator.Text = "🖱  поиск…";

        try
        {
            var mice = await Task.Run(DetectMiceWithWindows);
            if (mice.Count == 0)
            {
                MouseName.Text = "Мышь не обнаружена";
                MouseIndicator.Text = "🖱  —";
                DetectionHint.Text = "Windows не вернула подключённых мышей. Проверь приёмник и нажми «Обновить».";
            }
            else
            {
                MouseName.Text = string.Join(", ", mice.Take(2).Select(x => x.Name));
                MouseIndicator.Text = $"🖱  {mice.Count} подкл.";
                DetectionHint.Text = mice.Count == 1
                    ? "Мышь обнаружена через Windows PnP. Заряд по USB-приёмнику может быть недоступен."
                    : $"Windows видит {mice.Count} мышей (включая возможные виртуальные HID-устройства).";
            }

            MouseBattery.Text = "Заряд: неизвестен";
        }
        catch (Exception ex)
        {
            MouseName.Text = "Не удалось прочитать список устройств";
            MouseIndicator.Text = "🖱  ?";
            DetectionHint.Text = $"Поиск мыши не удался: {ex.Message}";
            MouseBattery.Text = "Заряд: неизвестен";
        }
        finally
        {
            _detecting = false;
        }
    }

    private record MouseDevice(string Name, string Id);

    private static List<MouseDevice> DetectMiceWithWindows()
    {
        // Windows PnP only: no administrator privileges or device-driver installs.
        // Do not confuse device discovery with wireless battery measurement.
        const string script = "[Console]::OutputEncoding=[System.Text.Encoding]::UTF8; " +
                              "@(Get-PnpDevice -Class Mouse -PresentOnly -ErrorAction Stop | " +
                              "Select-Object FriendlyName,InstanceId) | ConvertTo-Json -Compress";
        var encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
        var start = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = "-NoProfile -NonInteractive -EncodedCommand " + encoded,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = start };
        if (!process.Start())
            throw new InvalidOperationException("PowerShell не запустился.");

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        if (!process.WaitForExit(12000))
        {
            process.Kill(true);
            throw new TimeoutException("Windows слишком долго отвечает на запрос устройств.");
        }
        if (process.ExitCode != 0)
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(error)
                ? "Get-PnpDevice завершился с ошибкой." : error.Trim());

        if (string.IsNullOrWhiteSpace(output) || output.Trim() == "null" || output.Trim() == "[]")
            return new List<MouseDevice>();

        using var json = JsonDocument.Parse(output);
        IEnumerable<JsonElement> entries = json.RootElement.ValueKind == JsonValueKind.Array
            ? json.RootElement.EnumerateArray().ToArray()
            : new[] { json.RootElement };

        var result = new List<MouseDevice>();
        foreach (var item in entries)
        {
            var name = item.TryGetProperty("FriendlyName", out var friendly)
                ? friendly.GetString() : null;
            var id = item.TryGetProperty("InstanceId", out var instance)
                ? instance.GetString() : null;
            result.Add(new MouseDevice(string.IsNullOrWhiteSpace(name) ? "HID-мышь" : name!,
                                       id ?? string.Empty));
        }

        return result;
    }
}
