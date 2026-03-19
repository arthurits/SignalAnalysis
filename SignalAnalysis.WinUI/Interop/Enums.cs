using System.Runtime.InteropServices;

namespace SignalAnalysis.Interop;
internal static partial class Win32
{
    [Flags]
    internal enum DwmBlurBehindFlags : uint
    {
        ENABLE = 0x00000001,
        BLURREGION = 0x00000002,
        TRANSITIONONMAXIMIZED = 0x00000004
    }

    internal enum DWMWINDOWATTRIBUTE : uint
    {
        USE_IMMERSIVE_DARK_MODE = 20,
        WINDOW_CORNER_PREFERENCE = 33
    }

    internal enum DWM_WINDOW_CORNER_PREFERENCE : uint
    {
        DEFAULT = 0,
        DONOTROUND = 1,
        ROUND = 2,
        ROUNDSMALL = 3
    }

    public enum PROCESS_ACCESS_TYPES
    {
        PROCESS_TERMINATE = 0x00000001,
        PROCESS_CREATE_THREAD = 0x00000002,
        PROCESS_SET_SESSIONID = 0x00000004,
        PROCESS_VM_OPERATION = 0x00000008,
        PROCESS_VM_READ = 0x00000010,
        PROCESS_VM_WRITE = 0x00000020,
        PROCESS_DUP_HANDLE = 0x00000040,
        PROCESS_CREATE_PROCESS = 0x00000080,
        PROCESS_SET_QUOTA = 0x00000100,
        PROCESS_SET_INFORMATION = 0x00000200,
        PROCESS_QUERY_INFORMATION = 0x00000400,
        STANDARD_RIGHTS_REQUIRED = 0x000F0000,
        SYNCHRONIZE = 0x00100000,
        PROCESS_ALL_ACCESS = PROCESS_TERMINATE | PROCESS_CREATE_THREAD | PROCESS_SET_SESSIONID | PROCESS_VM_OPERATION |
            PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_DUP_HANDLE | PROCESS_CREATE_PROCESS | PROCESS_SET_QUOTA |
            PROCESS_SET_INFORMATION | PROCESS_QUERY_INFORMATION | STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE
    }

    // Enums for GDI+ Status and Pixel Formats
    public enum GpStatus
    {
        Ok = 0,
        GenericError = 1,
        InvalidParameter = 2,
        OutOfMemory = 3,
        ObjectBusy = 4,
        InsufficientBuffer = 5,
        NotImplemented = 6,
        Win32Error = 7,
        WrongState = 8,
        Aborted = 9,
        FileNotFound = 10,
        ValueOverflow = 11,
        AccessDenied = 12,
        UnknownImageFormat = 13,
        FontFamilyNotFound = 14,
        FontStyleNotFound = 15,
        NotTrueTypeFont = 16,
        UnsupportedGdiplusVersion = 17,
        GdiplusNotInitialized = 18,
        PropertyNotFound = 19,
        PropertyNotSupported = 20,
        ProfileNotFound = 21
    }

    [Flags]
    public enum PixelFormat : int
    {
        DontCare = 0,                   // Flat out “I don't care”
        Format1bppIndexed = 0x000301,   // 1 bit per pixel, indexed palette
        Format4bppIndexed = 0x000304,   // 4 bpp
        Format8bppIndexed = 0x000308,
        Format16bppGrayScale = 0x0010100,
        Format16bppRGB555 = 0x000502,
        Format16bppRGB565 = 0x000606,
        Format24bppRGB = 0x000018,
        Format32bppRGB = 0x000209,      // without alpha channel, not premultiplied
        Format32bppARGB = 0x26200A,     // with alfa, not premultiplied
        Format32bppPARGB = 0x32200B,    // with alfa premultiplied
        Format48bppRGB = 0x1001100,
        Format64bppARGB = 0x2001A0,
        Format64bppPARGB = 0x3001B0
    }

    public enum InterpolationMode : int
    {
        Invalid = -1,
        Default = 0,
        LowQuality = 1,
        HighQuality = 2,
        Bilinear = 3,
        NearestNeighbor = 4,
        HighQualityBilinear = 5,
        HighQualityBicubic = 6
    }
}
