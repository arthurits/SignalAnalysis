using CommunityToolkit.Mvvm.ComponentModel;
using SignalAnalysis.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalAnalysis.Template.ViewModels;

public partial class StartUpViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial string StrTest { get; set; } = string.Empty;

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        StrTest = "StrAppSettings".GetLocalized("Settings");
    }
}
