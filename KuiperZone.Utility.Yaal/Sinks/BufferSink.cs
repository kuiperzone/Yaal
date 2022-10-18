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
/// Implements <see cref="ILogSink"/> using FIFO memory buffer. This is useful in unit testing,
/// where the caller holds a reference to the sink instance and can query whether certain code paths
/// have correctly executed based on their logging output. An instance of this is thread-safe.
/// </summary>
public sealed class BufferSink : ILogSink
{
    private readonly object _syncObj = new();
    private readonly List<string> _history = new();

    /// <summary>
    /// Constructor with option values. Serves as default constructor.
    /// </summary>
    public BufferSink(LogFormat format = LogFormat.Clean, SeverityLevel threshold = SeverityLevel.Lowest)
        : this(new BufferConfig(format, threshold))
    {
        Config = new BufferConfig(format, threshold);
    }

    /// <summary>
    /// Constructor variant with <see cref="IReadOnlyBufferConfig.Capacity"/> value.
    /// </summary>
    public BufferSink(int capacity, LogFormat format = LogFormat.Clean, SeverityLevel threshold = SeverityLevel.Lowest)
    {
        Config = new BufferConfig(capacity, format, threshold);
    }

    /// <summary>
    /// Constructor with config instance.
    /// </summary>
    public BufferSink(IReadOnlyBufferConfig config)
    {
        // Take a copy
        Config = new BufferConfig(config);
    }

    /// <summary>
    /// Gets a clone of the configuration instance supplied on construction.
    /// </summary>
    public IReadOnlyBufferConfig Config { get; }

    /// <summary>
    /// Implements <see cref="ILogSink.Config"/>.
    /// </summary>
    IReadOnlySinkConfig ILogSink.Config
    {
        get { return Config; }
    }

    /// <summary>
    /// Gets up to count recent log messages. The result is a new instance on each call.
    /// </summary>
    public string[] GetRecent(int count = int.MaxValue)
    {
        lock (_syncObj)
        {
            count = Math.Clamp(count, 0, _history.Count);

            if (count > 0)
            {
                int from = _history.Count - count;
                var array = new string[count];

                _history.CopyTo(from, array, 0, count);
                return array;
            }

            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Returns true if the string is contained with recent log messages.
    /// </summary>
    public bool Contains(string? s, StringComparison comp = StringComparison.Ordinal)
    {
        if (!string.IsNullOrEmpty(s))
        {
            foreach (var item in GetRecent())
            {
                if (item.Contains(s, comp))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Implements <see cref="ILogSink.Write"/>.
    /// </summary>
    public void Write(LogMessage msg, IReadOnlyLoggerConfig config)
    {
        lock(_syncObj)
        {
            if (_history.Count == Config.Capacity && _history.Count > 0)
            {
                _history.RemoveAt(0);
            }

            _history.Add(msg.ToString(new MessageStringOptions(Config.Format, config)));
        }
    }

}