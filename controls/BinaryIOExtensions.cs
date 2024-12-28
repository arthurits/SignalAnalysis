
namespace System.IO;

public static class BinaryIOExtensions
{
    /// <summary>
    /// Reads a line of characters from the current binary stream and returns the data as a string.
    /// A line is defined as a sequence of characters followed by a line feed ("\n"), a carriage return ("\r"), or a carriage return immediately followed by a line feed ("\r\n").
    /// The string that is returned does not contain the terminating carriage return or line feed.
    /// The returned value is null if the end of the input stream is reached.
    /// </summary>
    /// <param name="reader">The reading binary stream.</param>
    /// <returns>The next line from the input stream, or null if the end of the input stream is reached.</returns>
    /// <seealso cref="https://www.codeproject.com/Articles/996254/BinaryReader-ReadLine-extension"/>
    public static string ReadLine(this BinaryReader reader)
    {
        var result = new System.Text.StringBuilder();
        bool foundEndOfLine = false;
        char ch;
        while (!foundEndOfLine)
        {
            try
            {
                ch = reader.ReadChar();
            }
            catch (EndOfStreamException)
            {
                if (result.Length == 0) return null;
                else break;
            }

            switch (ch)
            {
                case '\r':
                    if (reader.PeekChar() == '\n') reader.ReadChar();
                    foundEndOfLine = true;
                    break;
                case '\n':
                    foundEndOfLine = true;
                    break;
                default:
                    result.Append(ch);
                    break;
            }
        }
        return result.ToString();
    }

    /// <summary>
    /// Writes a string to the stream, followed by a line terminator.
    /// </summary>
    /// <param name="reader">The writing binary stream</param>
    /// <param name="line">The string to be written.</param>
    public static void WriteLine(this BinaryWriter reader, string line = null)
    {
        reader.Write(line + Environment.NewLine);
    }

    /// <summary>
    /// Writes a DataTime object into a binary stream
    /// </summary>
    /// <param name="writer">The writing binary stream</param>
    /// <param name="dateTime">DateTime object to write into the stream</param>
    public static void Write(this BinaryWriter writer, DateTime dateTime)
    {
        writer.Write(dateTime.ToBinary());
    }

    /// <summary>
    /// Writes a DataTime object from a binary stream
    /// </summary>
    /// <param name="reader">The reading binary stream</param>
    /// <returns>DateTime object read from the stream</returns>
    public static DateTime ReadDateTime(this BinaryReader reader)
    {
        return DateTime.FromBinary(reader.ReadInt64());
    }

    /// <summary>
    /// Custom function to write a string into a binay stream.
    /// It's usually recommended to use the default BinaryWriter Write overloaded method.
    /// </summary>
    /// <param name="writer">The writing binary stream</param>
    /// <param name="text">String to be written</param>
    /// <param name="length">Number of bytes to to written</param>
    public static void Write(this BinaryWriter writer, string text, int length, System.Text.Encoding encoder = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            writer.Write(new byte[length]);
            return;
        }

        var buffer = new byte[length];
        var strbytes = encoder == null ? System.Text.Encoding.Default.GetBytes(text) : encoder.GetBytes(text);

        Array.Copy(strbytes, buffer, length > strbytes.Length ? strbytes.Length : length);

        writer.Write(buffer);
    }

    /// <summary>
    /// Custom function to read a string from a binay stream.
    /// It's usually recommended to use the default BinaryWriter ReadString method.
    /// </summary>
    /// <param name="reader">The reading stream</param>
    /// <param name="size">Number of bytes to be read</param>
    /// <returns>The string from the stream</returns>
    public static string ReadString(this BinaryReader reader, int size, System.Text.Encoding encoder = null)
    {
        var bytes = reader.ReadBytes(size);
        string str = encoder == null ? System.Text.Encoding.Default.GetString(bytes) : encoder.GetString(bytes);
        return str.Replace((char)0, ' ').Trim();
    }
}