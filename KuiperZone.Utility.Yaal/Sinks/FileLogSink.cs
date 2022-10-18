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
public sealed class FileLogSink : ILogSink
{
    private object _syncObj = new();
    private readonly ThreadLocal<FileSinkWriter>? _local;
    private readonly FileSinkOptions _options;
    private FileSinkWriter? _global;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public FileLogSink()
        : this(new FileSinkOptions())
    {
    }

    /// <summary>
    /// Constructor with options instance.
    /// </summary>
    public FileLogSink(FileSinkOptions opts)
    {
        // Take a copy
        _options = new FileSinkOptions(opts);
        DirectoryName = FileSinkWriter.Assert(_options);

        if (_options.FilePattern.Contains(FileSinkOptions.ThreadTag))
        {
            _local = new(() => {return new FileSinkWriter(_options);}, true);
        }
    }

    /// <summary>
    /// Gets the directory containing the log files.
    /// </summary>
    public string DirectoryName { get; }

    /// <summary>
    /// Implements <see cref="ILogSink.Write"/>.
    /// </summary>
    public void Write(LogMessage msg, IReadOnlyLoggerOptions opts)
    {
        if (msg.Severity.IsHigherOrEqualPriority(_options.Threshold))
        {
            var mo = new MessageStringOptions(_options, opts);
            mo.IndentClean = _options.IndentClean;

            var text = msg.ToString(mo);

            if (_local != null)
            {
                _local.Value?.Write(text);
            }
            else
            {
                lock (_syncObj)
                {
                    _global ??= new FileSinkWriter(_options);
                    _global.Write(text);
                }
            }
        }
    }

}