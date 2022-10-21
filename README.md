<img src="Media/yaal.png" style="width:50%;max-width:600px;margin-bottom:3em;"/>

# yaal #
**Yaal** is *yet another application logger* for dotnet. On Linux, it writes to syslog in RFC 5424 format by default and, on Windows, to EventLog. It supports other sinks and formats, including a file logger with rotation and auto-removal.

**[NUGET PACKAGE TBD](https://github.com/kuiperzone/AvantGarde/releases/latest)**

Note. Yaal is licensed under GPLv3. It has been tested on Linux and Windows, but not on other platforms.

## Features ##
Features include:

* Local syslog by default in RFC 5424 format on Linux
* Support for RFC 5424 structured data
* EventLog on Windows
* Supports BSD RFC 3164 format
* Log severity levels and a threshold setting
* Writes stack trace information in DEBUG build
* Supports file logging, with file rotation and old file removal
* Each thread can have own log file
* Other sinks include Console and memory

## Getting Started ##

See the AppDemo application in the source code. It provides examples of how to use most features of the Yaal `Logger` class.

Simply:

    using KuiperZone.Utility.Yaal;
    ...
    Logger.Global.Write($"Hello world");

Or for debug:

    Logger.Global.Debug($"Value: {value} = 0x{value:X8}");

The latter writes to the log only in a DEBUG build (it is omitted in RELEASE). Moreover, on Linux, it  writes RFC 5424 with stack trace as structured data:

    <15>1 2022-10-21T19:54:42.754408+01:00 vendetta DemoApp 17709 - [DGB@00000000 FUNC="KuiperZone.Utility.Yaal.DemoApp.Program.CallingMethod(Int32 value)" LINE="109" SEVERITY="debug" THREAD="17709-MAINTHREAD"] Value: 668 = 0x0000029C

Above, the calling method is recorded as SD-PARAMs "FUNC" and "LINE". Note that "THREAD" gives: "{pid}-{thread name or id}". This allows debug output to filtered on application PID and calling thread.

On Windows, it writes to EventLog:



## More Sinks ##

Yaal supports the concenpt of "sinks". Multiple sinks can be added to the logger. It is best to add sinks and change logging paramters at application start-up.

### Files ####

To add a file sink:

    Logger.Global.AddSink(new FileLogSink(new FileSinkOptions()));

The logger will now write to both Syslog (or EventLog), and files. The default file output directory is located under the user's home. By default, each thread writes to a separate file:

<img src="Media/file-out1.png" style="width:75%;max-width:1000px;"/>

Where log output is written using `Logger.Debug()`, the caller method and line information is prefxed to the output as follows:

<img src="Media/file-out2.png" style=""/>

Options can be set when the sink is created. Here, for example, we can auto remove old log files after 30 days:

    var opts = new FileSinkOptions();
    opts.StaleLife = TimeSpan.FromDays(30);
    Logger.Global.AddSink(new FileLogSink(opts));

The logging directory and filename can also be specified. Moreover, a number of placeholder variables can be used which are expanded at the time the file is opened.

Including the "{THD}" (thread name) placeholder in the filename will cause the sink to create a different file for each calling thread:

    var opts = new FileSinkOptions();
    opts.DirectoryPattern = "{DOCDIR}/Logs/{ASM}";
    opts.FilePattern = "{APP}-{PID}-{THD}-{[yyyyMMddTHHmmss]}.{PAG}.log";
    Logger.Global.AddSink(new FileLogSink(opts));

Above will cause a logging directory to be created under the user home directory. Each log file will be named after the application, PID, thread name (or ID), date-time, and a "page" counter. If "{THD}" is omitted, logging output from different threads will be written to the same file.

### Console ###

To add a Console sink:

    Logger.Global.AddSink(new ConsoleLogSink());

By default, output is colored according to severity.

<img src="Media/console.png" style="width:500%;max-width:400px;"/>

### Buffer ###



## RFC 5424 Structured Data ##

