namespace System;

/// <summary>
/// Provides functions to validate object's properties as numbers
/// </summary>
class Validation
{
    /// <summary>
    /// Checks whether the object passed is of type double and whether its value is within the lower and upper limits
    /// </summary>
    /// <param name="obj">Object cointaining a double value</param>
    /// <param name="lowerBound">Lower limit to check</param>
    /// <param name="upperBound">Upper limit to check</param>
    /// <returns>The original value if it's within the limits. The lower or upper limits otherwise if it's off-limits, or 0 if it's not a valid double and there's no lower limit</returns>
    public static double ValidateNumber<T>(object obj, double? lowerBound = null, double? upperBound = null)
    {
        if (typeof(T) == typeof(int))
        {
            if (!IsValidInteger(obj)) return lowerBound ?? 0.0;
        }
        else
        {
            if (!IsValidDouble(obj)) return lowerBound ?? 0.0;
        }

        // Try converting it to double
        double value;
        if (!double.TryParse(obj.ToString(), out value))
            return 0.0;

        if (lowerBound.HasValue && value < lowerBound.Value) return lowerBound.Value;
        if (upperBound.HasValue && value > upperBound.Value) return upperBound.Value;

        return value;
    }

    /// <summary>
    /// Checks whether the object passed is of type double and whether its value is within the lower and upper limits
    /// </summary>
    /// <param name="obj">Object cointaining a double value</param>
    /// <param name="lowerBound">Lower limit to check</param>
    /// <param name="upperBound">Upper limit to check</param>
    /// <param name="showMsgBox"><see langword="True"/> if a MessageBox is to be shown</param>
    /// <param name="parent">Parent <see cref="Form"/></param>
    /// <returns><see langword="True"/> if <paramref name="obj"/> is within the bound limits, <see langword="false"/> otherwise</returns>
    public static bool IsValidRange<T>(object obj, double? lowerBound = null, double? upperBound = null, bool? showMsgBox = false, System.Windows.Forms.Form parent = null)
    {
        bool IsValid;

        switch (typeof(T))
        {
            case Type sbyteType when sbyteType == typeof(sbyte):
                IsValid = sbyte.TryParse(Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out sbyte dummySByte);
                break;
            case Type byteType when byteType == typeof(byte):
                IsValid = byte.TryParse(Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out byte dummyByte);
                break;
            case Type shortType when shortType == typeof(short):
                IsValid = short.TryParse(Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out short dummyShort);
                break;
            case Type ushortType when ushortType == typeof(ushort):
                IsValid = ushort.TryParse(Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out ushort dummyUShort);
                break;
            case Type intType when intType == typeof(int):
                IsValid = int.TryParse(Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out int dummyInt);
                break;
            case Type uintType when uintType == typeof(uint):
                IsValid = uint.TryParse(Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out uint dummyUInt);
                break;
            case Type longType when longType == typeof(long):
                IsValid = long.TryParse(Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out long dummyLong);
                break;
            case Type ulongType when ulongType == typeof(ulong):
                IsValid = ulong.TryParse(Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out ulong dummyULong);
                break;
            case Type floatType when floatType == typeof(float):
                IsValid = float.TryParse(Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out float dummyFloat);
                break;
            case Type doubleType when doubleType == typeof(double):
                IsValid = double.TryParse(Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out double dummyDouble);
                break;
            case Type decimalType when decimalType == typeof(decimal):
                IsValid = decimal.TryParse(Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out decimal dummyDecimal);
                break;
            default:
                IsValid = IsValidDouble(obj);
                break;
        }

        
        //if (typeof(T) == typeof(int))
        //    IsValid = IsValidInteger(obj);
        //else
        //    IsValid = IsValidDouble(obj);

        if (!IsValid)
        {
            if (showMsgBox.HasValue && showMsgBox.Value)
            {
                using (new System.Windows.Forms.CenterWinDialog(parent))
                {
                    System.Windows.Forms.MessageBox.Show(
                      $"The input data could not be converted to a number{Environment.NewLine}Please, check and modify the highlighted field.",
                      $"Data error",
                      System.Windows.Forms.MessageBoxButtons.OK,
                      System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            return false;
        }

        double value = Convert.ToDouble(obj.ToString());
        //double value;
        //if (!double.TryParse(obj.ToString(), out value))
        //    return false;

        if (lowerBound.HasValue && value < lowerBound.Value)
        {
            if (showMsgBox.HasValue && showMsgBox.Value)
            {
                System.Windows.Forms.MessageBox.Show(
                  $"The input data is off-limits. Please, check the highlighted field{Environment.NewLine}and make sure it is equal or above {lowerBound}.",
                  $"Data error",
                  System.Windows.Forms.MessageBoxButtons.OK,
                  System.Windows.Forms.MessageBoxIcon.Error);
            }
            return false;
        }
        if (upperBound.HasValue && value > upperBound.Value)
        {
            if (showMsgBox.HasValue && showMsgBox.Value)
            {
                System.Windows.Forms.MessageBox.Show(
                  $"The input data is off-limits. Please, check the highlighted field{Environment.NewLine}and make sure it is equal or below {upperBound}.",
                  $"Data error",
                  System.Windows.Forms.MessageBoxButtons.OK,
                  System.Windows.Forms.MessageBoxIcon.Error);
            }
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if the string format corresponds to a decimal number
    /// </summary>
    /// <param name="str"><see cref="object"/> (property) constaining the value to check</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    /// <seealso href="https://stackoverflow.com/questions/894263/identify-if-a-string-is-a-number"/>
    /// <seealso href="https://stackoverflow.com/questions/33939770/regex-for-decimal-number-validation-in-c-sharp"/>
    public static bool IsValidDouble(object str)
    {
        if (str is null) return false;

        var m = System.Text.RegularExpressions.Regex.Match(str.ToString() ?? string.Empty, @"^-?\+?[0-9]*\.?\,?[0-9]+$");
        
        var result = double.TryParse(Convert.ToString(str, Globalization.CultureInfo.InvariantCulture),
                                    System.Globalization.NumberStyles.Any,
                                    Globalization.NumberFormatInfo.InvariantInfo,
                                    out _);

        return m.Success && m.Value != string.Empty && result;
    }

    /// <summary>
    /// Checks if the string format corresponds to a non-decimal number
    /// </summary>
    /// <param name="str"><see cref="object"/> (property) containing the value to check</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    /// <seealso href="https://stackoverflow.com/questions/1130698/checking-if-an-object-is-a-number-in-c-sharp/1130748"/>
    public static bool IsValidInteger(object str)
    {
        if (str is null) return false;

        var m = System.Text.RegularExpressions.Regex.Match(str.ToString() ?? string.Empty, @"^-?\+?[0-9]+$");
        
        var result = int.TryParse(Convert.ToString(str, Globalization.CultureInfo.InvariantCulture),
                                System.Globalization.NumberStyles.Any,
                                Globalization.NumberFormatInfo.InvariantInfo,
                                out _);

        return m.Success && m.Value != string.Empty && result;
    }
}
