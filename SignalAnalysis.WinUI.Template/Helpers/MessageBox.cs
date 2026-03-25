using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace SignalAnalysis.Template.Helpers;

// This code is adapted from the original MessageBox here:
// https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/MessageBox.cs
public static class MessageBox
{
    private const int IDOK = 1;
    private const int IDCANCEL = 2;
    private const int IDABORT = 3;
    private const int IDRETRY = 4;
    private const int IDIGNORE = 5;
    private const int IDYES = 6;
    private const int IDNO = 7;
    private const int DEFAULT_BUTTON1 = 0x00000000;
    private const int DEFAULT_BUTTON2 = 0x00000100;
    private const int DEFAULT_BUTTON3 = 0x00000200;

    
    // Specifies identifiers to indicate the return value of a dialog box.
    public enum MessageBoxResult
    {
        // Nothing is returned from the dialog box. This means that the modal dialog continues running.
        None = 0,

        // The dialog box return value is OK (usually sent from a button labeled OK).
        OK = 1,

        // The dialog box return value is Cancel (usually sent from a button labeled Cancel).
        Cancel = 2,

        // The dialog box return value is Yes (usually sent from a button labeled Yes).
        Yes = 6,

        // The dialog box return value is No (usually sent from a button labeled No).
        No = 7,

        // NOTE: if you add or remove any values in this enum, be sure to update MessageBox.IsValidMessageBoxResult()
    }
    
    [Flags]
    public enum MessageBoxOptions
    {
        // Specifies that all default options should be used.
        None = 0x00000000,

        // Specifies that the message box is displayed on the active desktop. 
        ServiceNotification = 0x00200000,

        // Specifies that the message box is displayed on the active desktop. 
        DefaultDesktopOnly = 0x00020000,

        // Specifies that the message box text is right-aligned.
        RightAlign = 0x00080000,

        // Specifies that the message box text is displayed with Rtl reading order.
        RtlReading = 0x00100000,
    }
    
    public enum MessageBoxImage
    {
        /// Specifies that the message box contain no symbols. 
        None,

        /// Specifies that the message box contains a hand symbol. 
        Hand,

        /// Specifies that the message box contains a hand icon. 
        Error,

        /// Specifies that the message box contains a question mark symbol. 
        Question,

        /// Specifies that the message box contains an exclamation symbol. 
        Exclamation,

        /// Specifies that the message box contains an asterisk symbol. 
        Asterisk,

        /// Completed task
        Completed,

        /// Specifies that the message box contains a hand icon. This field is constant.
        Stop = Hand,

        /// Specifies that the message box contains an exclamation icon. 
        Warning = Exclamation,

        /// Specifies that the message box contains an asterisk icon. 
        Information = Asterisk,

        // NOTE: if you add or remove any values in this enum, be sure to update MessageBox.IsValidMessageBoxIcon()    
    }
    
    public enum MessageBoxButton
    {
        // Specifies that the message box contains an OK button. This field is constant.
        OK = 0x00000000,

        // Specifies that the message box contains OK and Cancel button. This field is constant.
        OKCancel = 0x00000001,

        // Specifies that the message box contains Yes, No, and Cancel button. This field is constant.
        YesNoCancel = 0x00000003,

        // Specifies that the message box contains Yes and No button. This field is constant.
        YesNo = 0x00000004,

        // NOTE: if you add or remove any values in this enum, be sure to update MessageBox.IsValidMessageBoxButton()
    }

    public enum MessageBoxButtonDefault
    {
        // Specifies that the message box contains an OK button. This field is constant.
        PrimaryButton,

        // Specifies that the message box contains OK and Cancel button. This field is constant.
        SecondaryButton,

        // Specifies that the message box contains Yes, No, and Cancel button. This field is constant.
        CloseButton,

        // NOTE: if you add or remove any values in this enum, be sure to update MessageBox.IsValidMessageBoxButton()
    }

    public static async Task<ContentDialogResult> Show(
        XamlRoot root,
        string messageBoxText,
        string caption,
        string primaryButtonText = "OK",
        string secondaryButtonText = "",
        string closeButtonText = "",
        MessageBoxButtonDefault defaultButton = MessageBoxButtonDefault.PrimaryButton,
        MessageBoxImage icon = MessageBoxImage.None,
        int iconSize = 36)
    {
        // Check validity of the parameters passed
        if (!IsValidMessageBoxButtonDefault(defaultButton))
        {
            throw new InvalidEnumArgumentException("button", (int)defaultButton, typeof(MessageBoxButtonDefault));
        }
        if (!IsValidMessageBoxImage(icon))
        {
            throw new InvalidEnumArgumentException("icon", (int)icon, typeof(MessageBoxImage));
        }
        //if (!IsValidMessageBoxResult(defaultResult))
        //{
        //    throw new InvalidEnumArgumentException("defaultResult", (int)defaultResult, typeof(MessageBoxResult));
        //}
        //if (!IsValidMessageBoxOptions(options))
        //{
        //    throw new InvalidEnumArgumentException("options", (int)options, typeof(MessageBoxOptions));
        //}

        FontIcon? fontIcon = null;
        if (icon != MessageBoxImage.None)
        {
            // https://stackoverflow.com/questions/5390270/how-to-add-a-stackpanel-in-a-button-in-c-sharp-code-behind
            fontIcon = new()
            {
                Glyph = icon switch
                {
                    MessageBoxImage.Hand => "\uEB90",
                    MessageBoxImage.Error => "\uE783",
                    MessageBoxImage.Question => "\uE9CE",
                    MessageBoxImage.Exclamation => "\uE7BA",
                    MessageBoxImage.Asterisk => "\uE946",
                    MessageBoxImage.Completed => "\uEC61",
                    _ => string.Empty
                },
                Foreground = icon switch
                {
                    MessageBoxImage.Hand => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0)),
                    MessageBoxImage.Error => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0)),
                    MessageBoxImage.Question => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 255)),
                    MessageBoxImage.Exclamation => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 180, 0)),
                    MessageBoxImage.Asterisk => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 255)),
                    MessageBoxImage.Completed => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 180, 0)),
                    _ => new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0))
                },
                Margin = new Thickness(0, 0, 8, 0),
                FontSize = iconSize,
            };

        }
        
        TextBlock text = new()
        {
            Text = messageBoxText,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center
        };

        StackPanel stk = new()
        {
            Orientation = Microsoft.UI.Xaml.Controls.Orientation.Horizontal,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch,
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch
        };
        //stk.Children.Add(img);
        if (fontIcon is not null)
        {
            stk.Children.Add(fontIcon);
        }
        stk.Children.Add(text);

        //var window = (Application.Current as App)?.Window as MainWindow;
        var dialog = new ContentDialog
        {
            XamlRoot = root,
            Title = caption,
            Content = stk,
            PrimaryButtonText = primaryButtonText,
            SecondaryButtonText = secondaryButtonText,
            CloseButtonText = closeButtonText,
            DefaultButton = defaultButton switch
            {
                MessageBoxButtonDefault.PrimaryButton => ContentDialogButton.Primary,
                MessageBoxButtonDefault.SecondaryButton => ContentDialogButton.Secondary,
                MessageBoxButtonDefault.CloseButton => ContentDialogButton.Close,
                _ => ContentDialogButton.None,
            },
        };

        return await dialog.ShowAsync();
    }

    public static async Task<ContentDialogResult> Show(
        string messageBoxText,
        string caption,
        string primaryButtonText = "OK",
        string secondaryButtonText = "",
        string closeButtonText = "",
        MessageBoxButtonDefault defaultButton = MessageBoxButtonDefault.PrimaryButton,
        MessageBoxImage icon = MessageBoxImage.None,
        int iconSize = 36)
    {
        return await Show(
            App.MainWindow.Content.XamlRoot,
            messageBoxText,
            caption,
            primaryButtonText,
            secondaryButtonText,
            closeButtonText,
            defaultButton,
            icon,
            iconSize);
    }

    private static bool IsValidMessageBoxButton(MessageBoxButton value)
    {
        return value == MessageBoxButton.OK
            || value == MessageBoxButton.OKCancel
            || value == MessageBoxButton.YesNo
            || value == MessageBoxButton.YesNoCancel;
    }

    private static bool IsValidMessageBoxButtonDefault(MessageBoxButtonDefault value)
    {
        return value == MessageBoxButtonDefault.PrimaryButton
            || value == MessageBoxButtonDefault.SecondaryButton
            || value == MessageBoxButtonDefault.CloseButton;
    }

    private static bool IsValidMessageBoxImage(MessageBoxImage value)
    {
        return value == MessageBoxImage.Asterisk
            || value == MessageBoxImage.Completed
            || value == MessageBoxImage.Error
            || value == MessageBoxImage.Exclamation
            || value == MessageBoxImage.Hand
            || value == MessageBoxImage.Information
            || value == MessageBoxImage.None
            || value == MessageBoxImage.Question
            || value == MessageBoxImage.Stop
            || value == MessageBoxImage.Warning;
    }

    private static bool IsValidMessageBoxResult(MessageBoxResult value)
    {
        return value == MessageBoxResult.Cancel
            || value == MessageBoxResult.No
            || value == MessageBoxResult.None
            || value == MessageBoxResult.OK
            || value == MessageBoxResult.Yes;
    }

    private static bool IsValidMessageBoxOptions(MessageBoxOptions value)
    {
        int mask = ~((int)MessageBoxOptions.ServiceNotification |
                     (int)MessageBoxOptions.DefaultDesktopOnly |
                     (int)MessageBoxOptions.RightAlign |
                     (int)MessageBoxOptions.RtlReading);

        return ((int)value & mask) == 0;
    }

}
