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
    private readonly bool _isThreadLocal;

    [ThreadStatic]
    private FileWriter? _threadWriter;
    private FileWriter? _globalWriter;

    /// <summary>
    /// Constructor.
    /// </summary>
    public FileSink(IReadOnlyFileSinkOptions options, SeverityLevel? threshold = null)
    {
        Threshold = threshold;
        Options = new FileSinkOptions(options);
        _isThreadLocal = Options.IsThreadLocal();

        DirectoryName = FileWriter.Assert(Options);
    }

    /// <summary>
    /// Implements <see cref="ILogSink.Threshold"/>.
    /// </summary>
    public SeverityLevel? Threshold { get; }

    /// <summary>
    /// Gets a clone of the options instance supplied on construction.
    /// </summary>
    public IReadOnlyFileSinkOptions Options { get; }

    /// <summary>
    /// Gets the directory containing the log files.
    /// </summary>
    public string DirectoryName { get; }

    /// <summary>
    /// Implements <see cref="ILogSink.Write"/>.
    /// </summary>
    public void Write(SeverityLevel severity, string message)
    {
        if (_isThreadLocal)
        {
            // Thread static reference
            _threadWriter ??= new FileWriter(Options);
            _threadWriter.Write(message);
        }
        else
        {
            lock (_syncObj)
            {
                _globalWriter ??= new FileWriter(Options);
                _globalWriter.Write(message);
            }
        }
    }

}