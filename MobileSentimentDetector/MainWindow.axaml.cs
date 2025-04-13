using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MobileSentimentDetector;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void DetectSentiment(object sender, RoutedEventArgs args)
    {
        sentimentPlaceholder.Text = $"You typed {input.Text}!";
    }
}