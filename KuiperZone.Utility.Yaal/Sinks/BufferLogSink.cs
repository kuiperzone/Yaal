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
public sealed class BufferLogSink : ILogSink
{
    private readonly object _syncObj = new();
    private readonly BufferSinkOptions _options;
    private readonly List<string> _history = new();

    /// <summary>
    /// Default constructor.
    /// </summary>
    public BufferLogSink()
    {
        _options = new BufferSinkOptions();
    }

    /// <summary>
    /// Constructor with options instance.
    /// </summary>
    public BufferLogSink(BufferSinkOptions opts)
    {
        // Take a copy
        _options = new BufferSinkOptions(opts);
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
    public void Write(LogMessage msg, IReadOnlyLogOptions opts)
    {
        if (msg.Severity.IsHigherOrEqualPriority(_options.Threshold))
        {
            lock (_syncObj)
            {
                if (_history.Count == _options.Capacity && _history.Count > 0)
                {
                    _history.RemoveAt(0);
                }

                _history.Add(msg.ToString(new MessageParams(_options, opts)));
            }
        }
    }

}