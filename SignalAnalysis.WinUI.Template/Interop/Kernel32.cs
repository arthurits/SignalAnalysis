using System.Runtime.InteropServices;

namespace SignalAnalysis.Template.Interop;
internal static partial class Win32
{
    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
}
