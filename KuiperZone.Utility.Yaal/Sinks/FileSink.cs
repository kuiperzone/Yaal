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
    public FileSink(LogFormat format = LogFormat.Clean, SeverityLevel threshold = SeverityLevel.Lowest)
        : this(new FileConfig(format, threshold))
    {
    }

    /// <summary>
    /// Constructor variant with <see cref="IReadOnlyFileConfig.DirectoryPattern"/> value.
    /// </summary>
    public FileSink(string directory, LogFormat format = LogFormat.Clean, SeverityLevel threshold = SeverityLevel.Lowest)
        : this(new FileConfig(directory, format, threshold))
    {
    }

    /// <summary>
    /// Constructor with configuration instance.
    /// </summary>
    public FileSink(IReadOnlyFileConfig config)
    {
        // Take a copy
        Config = new FileConfig(config);
        DirectoryName = FileSinkWriter.Assert(Config);

        if (Config.FilePattern.Contains(IReadOnlyFileConfig.ThreadTag))
        {
            _local = new(() => {return new FileSinkWriter(Config);}, true);
        }
    }

    /// <summary>
    /// Gets a clone of the configuration instance supplied on construction.
    /// </summary>
    public IReadOnlyFileConfig Config { get; }

    /// <summary>
    /// Implements <see cref="ILogSink.Config"/>.
    /// </summary>
    IReadOnlySinkConfig ILogSink.Config
    {
        get { return Config; }
    }

    /// <summary>
    /// Gets the directory containing the log files.
    /// </summary>
    public string DirectoryName { get; }

    /// <summary>
    /// Implements <see cref="ILogSink.Write"/>.
    /// </summary>
    public void Write(LogMessage msg, IReadOnlyLoggerConfig config)
    {
        var opts = new MessageStringOptions(Config.Format, config);
        opts.IndentClean = Config.IndentClean;

        var text = msg.ToString(opts);

        if (_local != null)
        {
            _local.Value?.Write(text);
        }
        else
        {
            lock (_syncObj)
            {
                _global ??= new FileSinkWriter(Config);
                _global.Write(text);
            }
        }
    }

}