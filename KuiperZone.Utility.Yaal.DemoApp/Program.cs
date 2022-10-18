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

namespace KuiperZone.Utility.Yaal.DemoApp;

class Program
{
    private static int s_threadCount;

    public static int Main(string[] args)
    {
        // SET UP LOGGER (use global instance)
        // By default, Logger has a SyslogSink which will write to syslog
        // on LINUX (RFC 5424), or EventLog on Windows (plain text).
        var log = Logger.Global;

        // Add file output. By default FileSink will
        // write to user's Document folder.
        var fcon = new FileConfig();
        fcon.IndentClean = 100;
        var files = new FileSink(fcon);
        log.AddSink(files);

        // We will see output on console
        log.AddSink(new ConsoleSink());

        // We will use BufferSink later.
        // It's primary use case is in unit testing.
        var buffer = new BufferSink();
        log.AddSink(buffer);


        // This is useful in tracking calling threads
        Thread.CurrentThread.Name = "MAINTHREAD";


        // INTRO
        log.Write($"YAAL HELLO WORLD: {AppInfo.Pid}");
        log.Write("NOTE. Log files also written to: " + files.DirectoryName);


        // SEVERITY LEVELS
        // The following Debug() will not be logged unless you
        // lower the threshold by uncommenting the line below.
        // log.Threshold = SeverityLevel.DebugL3;
        log.Write(SeverityLevel.DebugL3, $"This message has {SeverityLevel.DebugL3} severity");

        // Write a message for every severity, but note those
        // with lower priority than log.Threshold will be ignored.
        log.Write($"The following are severities down to {log.Threshold}");
        foreach (var item in Enum.GetValues<SeverityLevel>())
        {
            // We generate MsgId as upper-case severity value
            log.Write(item.ToString().ToUpper(), item, $"Message for {item} severity");
        }


        // STACK TRACE (CALLING METHOD NAME)
        log.Write("The following will be logged only in DEBUG build");
        log.Debug("This line should have the Main() method name nad line # associated with it");

        // Call a method to demonstrate stack trace
        CallingMethod(668);


        // MULTIPLE THREADS
        // The logger is thread-safe and on linux thread information is recorded
        // in structured data. Moreover, the FileSink has a special feature -- it
        // will write log output from different threads to separate files.
        new Thread(LogThread).Start();
        new Thread(LogThread).Start();
        new Thread(LogThread).Start();


        // BUFFER SINK
        // We can use BufferSink to query what was actually logged
        log.Debug("This is logged in DEBUG only");
        bool wasLogged = buffer.Contains("logged in DEBUG only");
        log.Write($"Statement was logged: {wasLogged}");

        return 0;
    }

    private static void CallingMethod(int value)
    {
        // Debug() queries the stack to determine the method name which called the logger.
        // In syslog, caller information will be included as structured data. In plain
        // Text format, it is prefixed to the message.
        Logger.Global.Debug($"This line should have the call name {nameof(CallingMethod)} and line # associated with it");

        // Example of how we can log values using interpolated string
        Logger.Global.Debug($"Value: {value} = 0x{value:X8}");
    }

    private static void LogThread(object? obj)
    {
        Thread.CurrentThread.Name = "LogThread-" + Interlocked.Increment(ref s_threadCount);

        Logger.Global.Write("THREAD NAME: " + Thread.CurrentThread.Name);
        Logger.Global.Debug("This is a debug only statement in thread: " + Thread.CurrentThread.Name);
    }
}

