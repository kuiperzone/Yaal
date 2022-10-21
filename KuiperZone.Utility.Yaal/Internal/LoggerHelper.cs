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

using KuiperZone.Utility.Yaal.Sinks;

namespace KuiperZone.Utility.Yaal.Internal;

/// <summary>
/// Helps facilite alock-free design in Logger class. It holds everything that would otherwise
/// be volatile in one class which, therefore, can be accessed via one volatile reference.
/// Public for unit testing only.
/// </summary>
public class LoggerHelper
{
    private volatile Exception? v_error;

    public LoggerHelper(SeverityLevel threshold, IEnumerable<ILogSink>? sinks = null)
    {
        Options = new LogOptions();
        Threshold = threshold;

        var temp = new List<ILogSink>();

        if (sinks == null)
        {
            try
            {
                temp.Add(new SyslogLogSink());
            }
            catch (Exception e)
            {
                v_error = e;
            }
        }
        else
        {
            temp.AddRange(sinks);
        }

        Sinks = temp;
    }

    private LoggerHelper(IReadOnlyLogOptions opts, SeverityLevel severity,
        IReadOnlyCollection<ILogSink> sinks, Exception? lastError)
    {
        Options = opts;
        Threshold = severity;
        Sinks = sinks;

        v_error = lastError;
    }

    public readonly IReadOnlyLogOptions Options;

    public readonly SeverityLevel Threshold;

    public readonly IReadOnlyCollection<ILogSink> Sinks;

    public Exception? Error
    {
        get { return v_error; }
    }

    public void ResetError()
    {
        v_error = null;
    }

    public bool Allow(SeverityLevel severity)
    {
        return severity.IsHigherOrEqualPriority(Threshold);
    }

    public LoggerHelper NewOptions(IReadOnlyLogOptions opts)
    {
        return new LoggerHelper(opts, Threshold, Sinks, Error);
    }

    public LoggerHelper NewThreshold(SeverityLevel severity)
    {
        return new LoggerHelper(Options, severity, Sinks, Error);
    }

    public LoggerHelper NewSinks(IReadOnlyCollection<ILogSink> sinks)
    {
        return new LoggerHelper(Options, Threshold, sinks, Error);
    }

    public void Write(LogMessage msg)
    {
        foreach (var item in Sinks)
        {
            try
            {
                item.Write(msg, Options);
            }
            catch (Exception e)
            {
                v_error ??= e;
            }
        }
    }

    private static IReadOnlyCollection<string> CreateIds(string s)
    {
        var ids = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (ids.Length < 10)
        {
            return ids;
        }

        var hash = new HashSet<string>(ids.Length);

        foreach (var item in ids)
        {
            hash.Add(item);
        }

        return hash;
    }

}