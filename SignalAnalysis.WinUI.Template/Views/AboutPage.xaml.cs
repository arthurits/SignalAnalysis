using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SignalAnalysis.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Windows.System;

namespace SignalAnalysis.Template.Views;

public sealed partial class AboutPage : Page
{
    public AboutViewModel ViewModel
    {
        get;
    }

    public AboutPage()
    {
        ViewModel = App.GetService<AboutViewModel>();
        InitializeComponent();
    }

    private async void OnCompanyHyperlinkClick(Hyperlink sender, HyperlinkClickEventArgs args)
    {
        // Check if the DataContext or ViewModel is an AboutViewModel and has the OpenCompanyUrlCommand
        var vm = DataContext as AboutViewModel ?? ViewModel as AboutViewModel;

        if (vm is not null)
        {
            // If the CommunityToolkit has created an IAsyncRelayCommand for the OpenCompanyUrlCommand, execute it
            if (vm.OpenCompanyUrlCommand is IAsyncRelayCommand asyncCmd)
            {
                await asyncCmd.ExecuteAsync(null);
                return;
            }

            // If it is a regular ICommand, execute it (note: this won't await, so it may not be ideal for async operations)
            if (vm.OpenCompanyUrlCommand is ICommand cmd && cmd.CanExecute(null))
            {
                cmd.Execute(null);
                return;
            }

            // Fallback: launch the URL directly if it's valid (this is a last-ditch effort and may not be ideal if the command is doing more than just launching the URL)
            if (Uri.TryCreate(vm.StrCompanyUrl, UriKind.Absolute, out var uri))
            {
                await Launcher.LaunchUriAsync(uri);
                return;
            }
        }

        // Last fallback: if we can't find the command or the URL, we can hardcode a URL or show an error message
        await Launcher.LaunchUriAsync(new Uri("https://github.com/arthurits"));
    }
}
