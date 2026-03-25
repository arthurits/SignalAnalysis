using CommunityToolkit.Mvvm.ComponentModel;
using $safeprojectname$.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace $safeprojectname$.ViewModels;

public partial class StartUpViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial string StrTest { get; set; } = string.Empty;

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        StrTest = "StrAppSettings".GetLocalized("Settings");
    }
}
