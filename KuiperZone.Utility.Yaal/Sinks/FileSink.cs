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

using KuiperZone.Utility.Yaal.Internal;

namespace KuiperZone.Utility.Yaal.Sinks;

/// <summary>
/// Implements <see cref="ILogSink"/> for a file logger.
/// </summary>
public sealed class FileSink : ILogSink
{
    private object _syncObj = new();
    private readonly ThreadLocal<FileSinkWriter>? _local;
    private FileSinkWriter? _global;

    /// <summary>
    /// Constructor with option values. Serves as default constructor.
    /// </summary>
    public FileSink(FormatKind format = FormatKind.Text, SeverityLevel threshold = SeverityLevel.Lowest)
        : this(new FileSinkOptions(format, threshold))
    {
    }

    /// <summary>
    /// Constructor variant with <see cref="IReadOnlyFileSinkOptions.DirectoryPattern"/> value.
    /// </summary>
    public FileSink(string directory, FormatKind format = FormatKind.Text, SeverityLevel threshold = SeverityLevel.Lowest)
        : this(new FileSinkOptions(directory, format, threshold))
    {
    }

    /// <summary>
    /// Constructor with options instance.
    /// </summary>
    public FileSink(IReadOnlyFileSinkOptions options)
    {
        // Take a copy
        Options = new FileSinkOptions(options);
        DirectoryName = FileSinkWriter.Assert(Options);

        if (Options.FilePattern.Contains(IReadOnlyFileSinkOptions.ThreadTag))
        {
            _local = new(() => {return new FileSinkWriter(Options);}, true);
        }
    }

    /// <summary>
    /// Gets a clone of the options instance supplied on construction.
    /// </summary>
    public IReadOnlyFileSinkOptions Options { get; }

    /// <summary>
    /// Implements <see cref="ILogSink.Options"/>.
    /// </summary>
    IReadOnlySinkOptions ILogSink.Options
    {
        get { return Options; }
    }

    /// <summary>
    /// Gets the directory containing the log files.
    /// </summary>
    public string DirectoryName { get; }

    /// <summary>
    /// Implements <see cref="ILogSink.Write"/>.
    /// </summary>
    public void Write(LogMessage message, IReadOnlyLoggerOptions options)
    {
        var text = message.ToString(Options.Format, options);

        if (_local != null)
        {
            _local.Value?.Write(text);
        }
        else
        {
            lock (_syncObj)
            {
                _global ??= new FileSinkWriter(Options);
                _global.Write(text);
            }
        }
    }

}