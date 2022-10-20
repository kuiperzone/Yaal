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

using System.Runtime.InteropServices;
using System.Text;
using KuiperZone.Utility.Yaal.Sinks;

namespace KuiperZone.Utility.Yaal.Internal;

/// <summary>
/// Internal class used by <see cref="FileLogSink"/>.
/// </summary>
public sealed class FileSinkWriter : IDisposable
{
    private readonly FileSinkOptions _options;
    private StreamWriter? _stream;
    private int _pageCounter;

    /// <summary>
    /// Constructor with options.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid path</exception>
    /// <exception cref="DirectoryNotFoundException">Directory not found</exception>
    /// <exception cref="IOException">File IO error</exception>
    public FileSinkWriter(FileSinkOptions opts)
    {
        _options = opts;
        _stream = NewFile();
    }

    /// <summary>
    /// Gets the line count.
    /// </summary>
    public long LineCount { get; private set; }

    /// <summary>
    /// Writes the message.
    /// </summary>
    public void Write(string msg)
    {
        _stream ??= NewFile();
        _stream.WriteLine(msg);
        _stream.Flush();

        LineCount += 1;

        if (_options.MaxLines > 0 && LineCount >= _options.MaxLines)
        {
            _stream.Dispose();
            _stream = null;
            LineCount = 0;
        }
    }

    /// <summary>
    /// Remove all log files.
    /// </summary>
    public static int RemoveStalefiles(string directory)
    {
        return RemoveStalefiles(directory, TimeSpan.Zero);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        _stream?.Dispose();
    }

    private static int RemoveStalefiles(string directory, TimeSpan life)
    {
        const string LogWild = "*" + FileSinkOptions.LogExt;
        const string OldWild = "*" + FileSinkOptions.LogExt + FileSinkOptions.OldExt;

        int rslt = 0;

        if (Directory.Exists(directory))
        {
            var files = new List<string>();
            files.AddRange(System.IO.Directory.GetFiles(directory, LogWild, SearchOption.TopDirectoryOnly));
            files.AddRange(System.IO.Directory.GetFiles(directory, OldWild, SearchOption.TopDirectoryOnly));

            var mark = DateTime.UtcNow.Subtract(life);
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            foreach (string fn in files)
            {
                try
                {
                    var fi = new FileInfo(fn);

                    if (life <= TimeSpan.Zero || fi.LastWriteTimeUtc < mark)
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

    private string EnsureDirectory()
    {
        var dir = _options.GetDirectoryName();

        if (Directory.Exists(dir))
        {
            return dir;
        }

        if (_options.CanCreateDirectory)
        {
            Directory.CreateDirectory(dir);
            return dir;
        }

        throw new DirectoryNotFoundException("Directory not found " + dir);
    }

    private StreamWriter NewFile()
    {
        var dir = EnsureDirectory();
        var path = Path.Combine(dir, _options.GetFileName(++_pageCounter));

        if (File.Exists(path))
        {
            try
            {
                var oldPath = path + FileSinkOptions.OldExt;

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

        if (_options.StaleLife > TimeSpan.Zero)
        {
            RemoveStalefiles(dir, _options.StaleLife);
        }

        var file = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        return new StreamWriter(file, new UTF8Encoding(false));
    }

}