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

using System.Reflection;
using KuiperZone.Utility.Yaal.Internal;
using KuiperZone.Utility.Yaal.Sinks;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// A logger class which writes messages to one or more logging sinks. By default, it will write to syslog
/// in RFC 5424 format on Linux, and EventLog on Windows. An instance is thread-safe, and the class
/// provides a global singleton.
/// </summary>
public sealed class Logger
{
    // Assumed to be faster than MethodBase.GetCurrentMethod()
    private readonly static string DebugMethodName = typeof(Logger).FullName + "." + nameof(Debug);
    private readonly static string DebugIfMethodName = typeof(Logger).FullName + "." + nameof(DebugIf);

    private volatile LoggerHelper v_helper;

    /// <summary>
    /// Default constructor. The <see cref="Threshold"/> property will be initialised to
    /// <see cref="SeverityLevel.Debug"/>, and <see cref="Sinks"/> will contain a single
    /// instance of <see cref="SyslogLogSink"/>.
    /// </summary>
    public Logger()
    {
        v_helper = new(SeverityLevel.Debug, null);
    }

    /// <summary>
    /// Constructor with initial value for <see cref="Threshold"/>. <see cref="Sinks"/>
    /// will contain a single instance of <see cref="SyslogLogSink"/>.
    /// </summary>
    public Logger(SeverityLevel threshold)
    {
        v_helper = new(threshold, null);
    }

    /// <summary>
    /// Constructor with one or more instances of <see cref="ILogSink"/>. <see cref="Threshold"/>
    /// will be initialised to <see cref="SeverityLevel.Debug"/>.
    /// </summary>
    public Logger(params ILogSink[] sinks)
    {
        v_helper = new(SeverityLevel.Debug, sinks);
    }

    /// <summary>
    /// Constructor with initial value for <see cref="Threshold"/> and one or more
    /// instances of <see cref="ILogSink"/>.
    /// </summary>
    public Logger(SeverityLevel threshold, params ILogSink[] sinks)
    {
        v_helper = new(threshold, sinks);
    }

    /// <summary>
    /// Gets a global singleton. By default, its <see cref="Sinks"/> property will contain
    /// a single instance of <see cref="SyslogLogSink"/>.
    /// </summary>
    public static readonly Logger Global = new();

    /// <summary>
    /// Gets or sets a collection of sinks. Although these can be changed in-flight, it is recommended that they
    /// are updated at the start of the program execution only. Assigning an empty container will be disable logging,
    /// although setting <see cref="Threshold"/> to <see cref="SeverityLevel.Disabled"/> should be the preferred way
    /// of disabling.
    /// </summary>
    public IReadOnlyCollection<ILogSink> Sinks
    {
        get { return v_helper.Sinks; }
        set { v_helper = v_helper.NewSinks(value); }
    }

    /// <summary>
    /// Gets or sets the logger options. These can be set using an instance of <see cref="LogOptions"/>.
    /// Although they can be changed in-flight, it is recommended that they are updated at the start of
    /// the program execution only.
    /// </summary>
    public IReadOnlyLogOptions Options
    {
        get { return v_helper.Options; }
        set { v_helper = v_helper.NewOptions(value); }
    }

    /// <summary>
    /// Gets or sets the logger severity threshold. If set to <see cref="SeverityLevel.Critical"/>, for
    /// example, only messages with this severity or higher will be logged. Those with lower priorities,
    /// such as <see cref="SeverityLevel.Info"/> will be ignored. Setting <see cref="Threshold"/>
    /// to <see cref="SeverityLevel.Disabled"/> will suspend all logging.
    /// </summary>
    public SeverityLevel Threshold
    {
        get { return v_helper.Threshold; }
        set { v_helper = v_helper.NewThreshold(value); }
    }

    /// <summary>
    /// Gets or sets a comma separated string contained message IDs to exclude (case sensitive).
    /// A message with a <see cref="LogMessage.MsgId"/> value contained within this string will be ignored.
    /// Example: "TCPIN,TCPOUT,LoopA".
    /// </summary>
    public string Exclusions
    {
        get { return v_helper.Exclusions; }
        set { v_helper = v_helper.NewExclusions(value); }
    }

    /// <summary>
    /// Gets the first Exception thrown internally. The approach of this <see cref="Logger"/> instance
    /// is not to throw exceptions, and this method is provided as means of diagnoising any problem that
    /// may be encountered logging data. The result is null if no internal error has been thrown (nominal case).
    /// </summary>
    public Exception? Error
    {
        get { return v_helper.Error; }
    }

    /// <summary>
    /// Sets <see cref="Error"/> to null.
    /// </summary>
    public void ResetError()
    {
        v_helper.ResetError();
    }

    /// <summary>
    /// Adds a new logging sink to the <see cref="Sinks"/> collection.
    /// </summary>
    /// <param name="sink"></param>
    public void AddSink(ILogSink sink)
    {
        var temp = new List<ILogSink>(v_helper.Sinks);
        temp.Add(sink);
        v_helper = v_helper.NewSinks(temp);
    }

    /// <summary>
    /// Writes the message to <see cref="Sinks"/> provided <see cref="LogMessage.Severity"/>
    /// equals or exceeds <see cref="Threshold"/> in priority.
    /// </summary>
    public void Write(LogMessage msg)
    {
        var temp = v_helper;

        if (temp.Allow(msg))
        {
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Writes the message text to <see cref="Sinks"/> with <see cref="SeverityLevel.Info"/>
    /// severity provided this equals or exceeds <see cref="Threshold"/> in priority.
    /// </summary>
    public void Write(string? text)
    {
        const SeverityLevel MsgSeverity = SeverityLevel.Info;

        var temp = v_helper;

        if (temp.Allow(MsgSeverity))
        {
            temp.Write(new LogMessage(MsgSeverity, text));
        }
    }

    /// <summary>
    /// Writes the message text to <see cref="Sinks"/> provided the given severity
    /// equals or exceeds <see cref="Threshold"/> in priority.
    /// </summary>
    public void Write(SeverityLevel severity, string? text)
    {
        var temp = v_helper;

        if (temp.Allow(severity))
        {
            temp.Write(new LogMessage(severity, text));
        }
    }

    /// <summary>
    /// Writes the message text to <see cref="Sinks"/> provided the given severity
    /// equals or exceeds <see cref="Threshold"/> in priority. The message will have
    /// the supplied ID.
    /// </summary>
    public void Write(string? msgId, SeverityLevel severity, string? text)
    {
        var temp = v_helper;

        if (temp.Allow(msgId, severity))
        {
            temp.Write(new LogMessage(msgId, severity, text));
        }
    }

    /// <summary>
    /// Writes the exception message to <see cref="Sinks"/> with <see cref="SeverityLevel.Error"/>
    /// severity provided this equals or exceeds <see cref="Threshold"/> in priority.
    /// </summary>
    public void Write(Exception e)
    {
        const SeverityLevel MsgSeverity = SeverityLevel.Error;

        var temp = v_helper;

        if (temp.Allow(MsgSeverity))
        {
            // Exception.Message only
            temp.Write(new LogMessage(MsgSeverity, e.Message));
        }
    }

    /// <summary>
    /// Writes the exception message to <see cref="Sinks"/> with the given severity
    /// provided this equals or exceeds <see cref="Threshold"/> in priority.
    /// </summary>
    public void Write(SeverityLevel severity, Exception e)
    {
        var temp = v_helper;

        if (temp.Allow(severity))
        {
            // Exception.Message only
            temp.Write(new LogMessage(severity, e.Message));
        }
    }

    /// <summary>
    /// Writes the exception message to <see cref="Sinks"/> with the given severity
    /// provided this equals or exceeds <see cref="Threshold"/> in priority. The
    /// message will have the supplied ID.
    /// </summary>
    public void Write(string? msgId, SeverityLevel severity, Exception e)
    {
        var temp = v_helper;

        if (temp.Allow(msgId, severity))
        {
            // Exception.Message only
            temp.Write(new LogMessage(msgId, severity, e.Message));
        }
    }

    /// <summary>
    /// Conditional variant of <see cref="Write(LogMessage)"/>.
    /// </summary>
    public void WriteIf(bool condition, LogMessage msg)
    {
        var temp = v_helper;

        if (condition && temp.Allow(msg))
        {
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Conditional variant of <see cref="Write(string?)"/>.
    /// </summary>
    public void WriteIf(bool condition, string? text)
    {
        const SeverityLevel MsgSeverity = SeverityLevel.Info;

        var temp = v_helper;

        if (condition && temp.Allow(MsgSeverity))
        {
            temp.Write(new LogMessage(MsgSeverity, text));
        }
    }

    /// <summary>
    /// Conditional variant of <see cref="Write(SeverityLevel, string?)"/>.
    /// </summary>
    public void WriteIf(bool condition, SeverityLevel severity, string? text)
    {
        var temp = v_helper;

        if (condition && temp.Allow(severity))
        {
            temp.Write(new LogMessage(severity, text));
        }
    }

    /// <summary>
    /// Conditional variant of <see cref="Write(string?, SeverityLevel, string?)"/>.
    /// </summary>
    public void WriteIf(bool condition, string? msgId, SeverityLevel severity, string? text)
    {
        var temp = v_helper;

        if (condition && temp.Allow(msgId, severity))
        {
            temp.Write(new LogMessage(msgId, severity, text));
        }
    }

    /// <summary>
    /// Writes the message to <see cref="Sinks"/> provided <see cref="LogMessage.Severity"/>
    /// equals or exceeds <see cref="Threshold"/> in priority. Debug variants trace the caller of
    /// the method, but note that debug methods are ignored and do nothing in Release builds.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(LogMessage msg)
    {
        var temp = v_helper;

        if (temp.Allow(msg))
        {
            MethodBase.GetCurrentMethod();
            msg.Debug ??= new(DebugMethodName);
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Writes the message text to <see cref="Sinks"/> with <see cref="SeverityLevel.Debug"/>
    /// severity provided this equals or exceeds <see cref="Threshold"/> in priority. Debug
    /// variants trace the caller of the method, but note that debug methods are ignored and
    /// do nothing in Release builds.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(string? text)
    {
        const SeverityLevel MsgSeverity = SeverityLevel.Debug;

        var temp = v_helper;

        if (temp.Allow(MsgSeverity))
        {
            var msg = new LogMessage(MsgSeverity, text);
            msg.Debug = new(DebugMethodName);
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Writes the message text to <see cref="Sinks"/> provided the given severity equals or
    /// exceeds <see cref="Threshold"/> in priority. Debug variants trace the caller of the
    /// method, but note that debug methods are ignored and do nothing in Release builds.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(SeverityLevel severity, string? text)
    {
        var temp = v_helper;

        if (temp.Allow(severity))
        {
            var msg = new LogMessage(severity, text);
            msg.Debug = new(DebugMethodName);
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Writes the message text to <see cref="Sinks"/> provided the given severity
    /// equals or exceeds <see cref="Threshold"/> in priority. The message will have
    /// the supplied ID. Debug variants trace the caller of the method, but note that
    /// debug methods are ignored and do nothing in Release builds.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(string? msgId, SeverityLevel severity, string? text)
    {
        var temp = v_helper;

        if (temp.Allow(msgId, severity))
        {
            var msg = new LogMessage(msgId, severity, text);
            msg.Debug = new(DebugMethodName);
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Writes the exception to <see cref="Sinks"/> with <see cref="SeverityLevel.Error"/>
    /// severity provided this equals or exceeds <see cref="Threshold"/> in priority. Debug variants
    /// trace the caller of the method, but note that debug methods are ignored and do nothing in
    /// Release builds.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(Exception e)
    {
        const SeverityLevel MsgSeverity = SeverityLevel.Error;

        var temp = v_helper;

        if (temp.Allow(MsgSeverity))
        {
            var msg = new LogMessage(MsgSeverity, e.ToString());
            msg.Debug = new(DebugMethodName);
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Writes the exception message to <see cref="Sinks"/> with the given severity provided this
    /// equals or exceeds <see cref="Threshold"/> in priority. Debug variants trace the caller of
    /// the method, but note that debug methods are ignored and do nothing in Release builds.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(SeverityLevel severity, Exception e)
    {
        var temp = v_helper;

        if (temp.Allow(severity))
        {
            var msg = new LogMessage(severity, e.ToString());
            msg.Debug = new(DebugMethodName);
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Writes the exception message to <see cref="Sinks"/> with the given severity provided this
    /// equals or exceeds <see cref="Threshold"/> in priority. The message will have the supplied
    /// ID. Debug variants trace the caller of the method, but note that debug methods are ignored
    /// and do nothing in Release builds.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(string? msgId, SeverityLevel severity, Exception e)
    {
        var temp = v_helper;

        if (temp.Allow(msgId, severity))
        {
            var msg = new LogMessage(msgId, severity, e.ToString());
            msg.Debug = new(DebugMethodName);
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Conditional variant of <see cref="Debug(LogMessage)"/>. Debug variants trace the caller of
    /// the method, but note that debug methods are ignored and do nothing in Release builds.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public void DebugIf(bool condition, LogMessage msg)
    {
        var temp = v_helper;

        if (condition && temp.Allow(msg))
        {
            msg.Debug ??= new(DebugIfMethodName);
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Conditional variant of <see cref="Debug(string?)"/>. Debug variants trace the caller of
    /// the method, but note that debug methods are ignored and do nothing in Release builds.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public void DebugIf(bool condition, string? text)
    {
        const SeverityLevel MsgSeverity = SeverityLevel.Debug;

        var temp = v_helper;

        if (condition && temp.Allow(MsgSeverity))
        {
            var msg = new LogMessage(MsgSeverity, text);
            msg.Debug = new(DebugIfMethodName);
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Conditional variant of <see cref="Debug(SeverityLevel, string?)"/>. Debug variants trace the
    /// caller of the method, but note that debug methods are ignored and do nothing in Release builds.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public void DebugIf(bool condition, SeverityLevel severity, string? text)
    {
        var temp = v_helper;

        if (condition && temp.Allow(severity))
        {
            var msg = new LogMessage(severity, text);
            msg.Debug = new(DebugIfMethodName);
            temp.Write(msg);
        }
    }

    /// <summary>
    /// Conditional variant of <see cref="Debug(string?, SeverityLevel, string?)"/>. Debug variants trace
    /// the caller of the method, but note that debug methods are ignored and do nothing in Release builds.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public void DebugIf(bool condition, string? msgId, SeverityLevel severity, string? text)
    {
        var temp = v_helper;

        if (condition && temp.Allow(msgId, severity))
        {
             var msg = new LogMessage(msgId, severity, text);
            msg.Debug = new(DebugIfMethodName);
            temp.Write(msg);
       }
    }

}