// -----------------------------------------------------------------------------
// PROJECT   : Yaal
// COPYRIGHT : Andy Thomas (C) 2022
// LICENSE   : GPL-3.0-or-later
// HOMEPAGE  : https://github.com/kuiperzone/Yaal
//
// This file is part of Yaal.
//
// Yaal is free software: you can redistribute it and/or modify it under the terms of the GNU General
// Public License as published by the Free Software Foundation, either version 3 of the License, or at your option
// any later version.
//
// Yaal is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with Yaal.
// If not, see <https://www.gnu.org/licenses/>.
// -----------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KuiperZone.Utility.Yaal.Internal;

/// <summary>
/// Static internal utilities. Public for testing.
/// </summary>
public static class LogUtil
{
    /// <summary>
    /// Gets an ID number for the calling thread.
    /// </summary>
    public static string ThreadId
    {
        get { return Thread.CurrentThread.ManagedThreadId.ToString(); }
    }

    /// <summary>
    /// Get a name for the calling thread. If it has no name, one is formed from the thread-id.
    /// </summary>
    public static string ThreadName
    {
        get
        {
            const int MaxThreadLength = 60;

            var name = Thread.CurrentThread.Name;

            if (string.IsNullOrEmpty(name))
            {
                // Use integer ID instead
                name = "Thread" + ThreadId;
            }
            else
            if (name.Length > MaxThreadLength)
            {
                // Not too long
                name = name.Substring(0, MaxThreadLength).Trim();
            }

            return name;
        }
    }

    /// <summary>
	/// Static routine which returns true if id a valid SD-NAME or SD-ID. I.e. it is not empty, and
	/// all characters are within the printable ASCII range, and they do not contain: '=', SP, ']', '"'.
    /// </summary>
	public static bool IsValidId([NotNullWhen(true)] string? id, int maxLength = int.MaxValue)
    {
        if (string.IsNullOrEmpty(id) || id.Length > maxLength)
        {
            return false;
        }

        foreach (var c in id)
        {
            if (c <= ' ' || c > '~' || c == '=' || c == '"' || c == ']')
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Asserts <see cref="IsValidId"/> is true.
    /// </summary>
    /// <exception cref="ArgumentNullException">name</exception>
    /// <exception cref="ArgumentException">Invalid RFC 5424 ID</exception>
    public static void AssertId([NotNull] string? id, int maxLength = int.MaxValue)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (!IsValidId(id, maxLength))
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("RFC 5424 ID empty");
            }

            if (id.Length > maxLength)
            {
                throw new ArgumentException("RFC 5424 ID too long " + id.Substring(0, maxLength) + "...");
            }

            throw new ArgumentException("Invalid RFC 5424 ID " + id);
        }
    }

    /// <summary>
    /// Overload. Equivalent to: Escape(obj?.ToString(), false, chars).
    /// </summary>
    public static string EscapeRemoval(object? obj, string? chars = null)
    {
        return Escape(obj?.ToString(), false, chars);
    }

    /// <summary>
    /// Overload. Equivalent to: Escape(str, false, chars).
    /// </summary>
    public static string Escape(string? str, string? chars = null)
    {
        return Escape(str, false, chars);
    }

    /// <summary>
	/// Escapes the ASCII string, replacing non-printing characters with escaped numerical codes.
    /// If "escapeExt" is true, characters >= 0x7F are also escaped. The "chars" value provides a
    /// list of additional characters to escape, i.e. "\\]\"".
    /// </summary>
    public static string Escape(string? str, bool escapeExt, string? chars = null)
    {
        // https://www.rfc-editor.org/rfc/rfc5424#section-6.3.5
        // https://www.rfc-editor.org/rfc/rfc5424#section-6.4
        str ??= "";

        if (chars != null)
        {
            foreach (var c in chars)
            {
                str = str.Replace(c.ToString(), "\\" + c);
            }
        }

        StringBuilder? buffer = null;

        for (int n = 0; n < str.Length; ++n)
        {
            var c = str[n];

            if (c >= '\xFF' && escapeExt)
            {
                buffer ??= new StringBuilder(str.Substring(0, n), str.Length + 16);
                AppendHex(c, buffer);
            }
            else
            if (c >= 0 && c < 0x20)
            {
                buffer ??= new StringBuilder(str.Substring(0, n), str.Length + 16);

                switch (c)
                {
                    case '\x00': buffer.Append("\\0"); break;
                    case '\x07': buffer.Append("\\a"); break;
                    case '\x08': buffer.Append("\\b"); break;
                    case '\x09': buffer.Append("\\t"); break;
                    case '\x0A': buffer.Append("\\n"); break;
                    case '\x0B': buffer.Append("\\v"); break;
                    case '\x0C': buffer.Append("\\f"); break;
                    case '\x0D': buffer.Append("\\r"); break;
                    case '\x1B': buffer.Append("\\e"); break;
                    default: AppendHex(c, buffer); break;
                }
            }
            else
            {
                buffer?.Append(c);
            }
        }

        return buffer?.ToString() ?? str;
    }

    private static void AppendHex(UInt16 c, StringBuilder buffer)
    {
        if (c <= 0xFF)
        {
            buffer.Append("\\x");
            buffer.Append(c.ToString("X2"));
        }
        else
        {
            buffer.Append("\\u");
            buffer.Append(c.ToString("X4"));
        }
    }

}