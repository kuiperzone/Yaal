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

using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using KuiperZone.Utility.Yaal.Sinks;

namespace KuiperZone.Utility.Yaal.Internal;

/// <summary>
/// Internal class used by <see cref="FileSink"/>.
/// </summary>
public sealed class FileSinkWriter : IDisposable
{
    private StreamWriter? _stream;
    private int _pageCounter;

    /// <summary>
    /// Extension for all log files.
    /// </summary>
    public const string LogExt = ".log";

    /// <summary>
    /// Extension for old log files.
    /// </summary>
    public const string OldExt = ".old";

    /// <summary>
    /// Constructor with options.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid path</exception>
    /// <exception cref="DirectoryNotFoundException">Directory not found</exception>
    /// <exception cref="IOException">File IO error</exception>
    public FileSinkWriter(IReadOnlyFileSinkOptions options)
    {
        Options = options;
        _stream = NewFile();
    }

    /// <summary>
    /// Options.
    /// </summary>
    public readonly IReadOnlyFileSinkOptions Options;

    /// <summary>
    /// Gets the line count.
    /// </summary>
    public long LineCount { get; private set; }

    /// <summary>
    /// Assert options. Returns directory.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid DirectoryPattern</exception>
    /// <exception cref="ArgumentException">Invalid FilePattern</exception>
    public static string Assert(IReadOnlyFileSinkOptions options)
    {
        // Call this to ensure name OK
        GetFileName(options.FilePattern, 0);
        return GetDirectoryName(options.DirectoryPattern);
    }

    /// <summary>
    /// Gets the directory formed by the expansion of <see cref="IReadOnlyFileSinkOptions.DirectoryPattern"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid DirectoryPattern</exception>
    public static string GetDirectoryName(string pattern)
    {
        pattern = SubstituteCommon(pattern);

        if (!string.IsNullOrEmpty(pattern))
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                pattern = pattern.Replace('\\', '/');
            }

            pattern = SubstituteDirectory(pattern, IReadOnlyFileSinkOptions.TempTag);
            pattern = SubstituteDirectory(pattern, IReadOnlyFileSinkOptions.DocTag);
        }

        return pattern;
    }

    /// <summary>
    /// Gets the filename formed by the expansion of <see cref="IReadOnlyFileSinkOptions.FilePattern"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid FilePattern</exception>
    public static string GetFileName(string pattern, int page)
    {
        pattern = SubstituteCommon(pattern);

        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException($"{nameof(pattern)} is empty");
        }

        const int ZeroPad = 3;
        pattern = pattern.Replace(IReadOnlyFileSinkOptions.ProcIdTag, AppInfo.Pid.PadLeft(ZeroPad, '0'), StringComparison.OrdinalIgnoreCase);
        pattern = pattern.Replace(IReadOnlyFileSinkOptions.ThreadTag, LogUtil.ThreadName, StringComparison.OrdinalIgnoreCase);
        pattern = pattern.Replace(IReadOnlyFileSinkOptions.PageTag, page.ToString(CultureInfo.InvariantCulture).PadLeft(ZeroPad, '0'), StringComparison.OrdinalIgnoreCase);

        pattern = SubstituteDateTime(pattern);

        if (!pattern.EndsWith(LogExt, StringComparison.OrdinalIgnoreCase))
        {
            pattern += LogExt;
        }

        return pattern;
    }

    /// <summary>
    /// Writes the message.
    /// </summary>
    public void Write(string message)
    {
        _stream ??= NewFile();
        _stream.WriteLine(message);
        _stream.Flush();

        LineCount += 1;

        if (Options.MaxLines > 0 && LineCount >= Options.MaxLines)
        {
            _stream.Dispose();
            _stream = null;
            LineCount = 0;
        }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        _stream?.Dispose();
    }

    private static string AssertDirectory(IReadOnlyFileSinkOptions options)
    {
        var dir = GetDirectoryName(options.DirectoryPattern);

        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            if (options.CreateDirectory)
            {
                Directory.CreateDirectory(dir);
            }
            else
            {
                throw new DirectoryNotFoundException("Directory not found " + dir);
            }
        }

        return dir;
    }

    private static string SubstituteCommon(string pattern)
    {
        pattern = pattern.Trim();
        pattern = pattern.Replace(IReadOnlyFileSinkOptions.AppTag, AppInfo.AppName, StringComparison.OrdinalIgnoreCase);
        pattern = pattern.Replace(IReadOnlyFileSinkOptions.AsmTag, AppInfo.AssemblyName, StringComparison.OrdinalIgnoreCase);
        pattern = pattern.Replace(IReadOnlyFileSinkOptions.BuildTag, AppInfo.IsDebug ? "Dbg" : "Rel", StringComparison.OrdinalIgnoreCase);

        return pattern;
    }

    private static string SubstituteDateTime(string pattern)
    {
        // Data substitution
        // I.e. {APP}-{[yyyyMMddTHHmmssZ]}-{THD}-{PAG}.log
        const string Date1 = "{[";
        const string Date2 = "]}";

        while(true)
        {
            int p1 = pattern.IndexOf(Date1);

            if (p1 < 0)
            {
                return pattern;
            }

            // Date substitution.
            int p2 = pattern.IndexOf(Date2);

            if (p2 <= p1)
            {
                throw new FormatException("File pattern contains '{[' without corresponding closure");
            }

            int len = p2 - p1 - Date1.Length;
            string tm = pattern.Substring(p1 + Date1.Length, len).Trim();

            if (tm.EndsWith("Z"))
            {
                tm = DateTime.UtcNow.ToString(tm);
            }
            else
            {
                tm = DateTime.Now.ToString(tm);
            }

            pattern = pattern.Substring(0, p1) + tm + pattern.Substring(p2 + Date2.Length);
        }
    }

    private static string SubstituteDirectory(string pattern, string tag)
    {
        if (pattern.Contains(tag, StringComparison.OrdinalIgnoreCase))
        {
            if (!pattern.StartsWith(tag, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"{tag} must only be used at start of the file pattern");
            }

            var value = Path.GetTempPath();

            if (tag == IReadOnlyFileSinkOptions.DocTag)
            {
                try
                {
                    value = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                catch
                {
                    // Fall to temporary
                }
            }

            return pattern.Replace(tag, value.TrimEnd('/', '\\'), StringComparison.OrdinalIgnoreCase);
        }

        return pattern;
    }

    private static int RemoveStalefiles(string directory, TimeSpan life)
    {
        int rslt = 0;

        if (life > TimeSpan.Zero)
        {
            List<string> files = new List<string>();
            files.AddRange(System.IO.Directory.GetFiles(directory, "*" + LogExt, SearchOption.TopDirectoryOnly));
            files.AddRange(System.IO.Directory.GetFiles(directory, "*" + LogExt + OldExt, SearchOption.TopDirectoryOnly));

            var mark = DateTime.UtcNow.Subtract(life);
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            foreach (string fn in files)
            {
                try
                {
                    FileInfo fi = new FileInfo(fn);

                    if (fi.LastWriteTimeUtc < mark)
                    {
                        if (!isWindows)
                        {
                            // Check we can lock the file
                            File.Open(fi.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None).Dispose();
                        }

                        File.Delete(fi.FullName);
                        rslt += 1;
                    }
                }
                catch (IOException)
                {
                }
            }
        }

        return rslt;
    }

    private StreamWriter NewFile()
    {
        var dir = AssertDirectory(Options);
        var path = Path.Combine(dir, GetFileName(Options.FilePattern, ++_pageCounter));

        if (File.Exists(path))
        {
            try
            {
                var oldPath = path + OldExt;

                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }

                File.Move(path, oldPath);
            }
            catch(IOException)
            {
                File.Delete(path);
            }
        }

        LineCount = 0;
        RemoveStalefiles(dir, Options.StaleLife);

        var file = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        return new StreamWriter(file, new UTF8Encoding(false));
    }

}