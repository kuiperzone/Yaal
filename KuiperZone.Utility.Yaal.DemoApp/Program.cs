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
        // This is useful in tracking calling threads below
        Thread.CurrentThread.Name = "MAINTHREAD";


        // SET UP LOGGER (use global instance)
        // By default, Logger has a SyslogSink which will write to syslog
        // on LINUX (RFC 5424), or EventLog on Windows (plain text).

        // Add file output. By default FileSink will
        // write to user's Document folder.
        var fopts = new FileSinkOptions();
        fopts.RemoveLogsOnStart = true;
        var files = new FileSink(fopts);
        Logger.Global.AddSink(files);

        // We will see output on console
        Logger.Global.AddSink(new ConsoleSink(new ConsoleSinkOptions(true)));

        // We will use BufferSink later.
        // It's primary use case is in unit testing.
        var buffer = new BufferSink();
        Logger.Global.AddSink(buffer);


        // INTRO
        Logger.Global.Write($"YAAL HELLO WORLD: {AppInfo.Pid}");
        Logger.Global.Write("NOTE. Log files also written to: " + files.DirectoryName);

        // SEVERITY LEVELS
        // The following Debug() will not be logged unless you
        // lower the threshold by uncommenting the line below.
        // log.Threshold = SeverityLevel.DebugL3;
        Logger.Global.Write(SeverityLevel.DebugL3, $"This message has {SeverityLevel.DebugL3} severity");

        // Write a message for every severity, but note those
        // with lower priority than log.Threshold will be ignored.
        var threshold = Logger.Global.Threshold;
        Logger.Global.Write($"The following are severities down to {threshold}");

        foreach (var item in Enum.GetValues<SeverityLevel>())
        {
            // We generate MsgId as upper-case severity value
            Logger.Global.Write(item.ToString().ToUpper(), item, $"Message for {item} severity");
        }


        // STACK TRACE (CALLING METHOD NAME)
        Logger.Global.Debug("This line will only be written in DEBUG and will have the Main() method name and line #");

        // Call a method to demonstrate stack trace
        CallingMethod(668);


        // LOG AN EXCEPTION
        try
        {
            throw new ArgumentException("Test exception");
        }
        catch(Exception e)
        {
            // Log Message only
            Logger.Global.Write(e);

            // Logs ToString
            Logger.Global.Debug(e);
        }


        // MULTIPLE THREADS
        // The logger is thread-safe and on linux thread information is recorded
        // in structured data. Moreover, the FileSink has a special feature -- it
        // will write log output from different threads to separate files.
        new Thread(LogThread).Start();
        new Thread(LogThread).Start();
        new Thread(LogThread).Start();


        // BUFFER SINK
        // We can use BufferSink to query what was actually logged
        Logger.Global.Debug("This is logged in DEBUG only");

        bool wasLogged = buffer.Contains("logged in DEBUG only");
        Logger.Global.Write($"Statement was logged: {wasLogged}");


        // STRUCTURED DATA
        var msg = new LogMessage(SeverityLevel.Notice, "Contains structured data");
        msg.Data.Add("exampleSDID@32473", new SdElement());
        msg.Data["exampleSDID@32473"].Add("iut", "9");
        msg.Data["exampleSDID@32473"].Add("eventSource", "rawr");
        msg.Data["exampleSDID@32473"].Add("eventID", "123");
        Logger.Global.Write(msg);

        Console.WriteLine("ERROR: " + Logger.Global.Error);

        return 0;
    }

    private static void CallingMethod(int value)
    {
        // Following will be logged on in DEBUG
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

