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

using System.Globalization;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// </summary>
public sealed class DebugInfo
{
    private static readonly string ConstructorEntry = typeof(DebugInfo).FullName + "..ctor";

    /// <summary>
    /// Contructor which will locate the caller in the stack. The "method" should be
    /// the full name of the method which called the logging functionality. If null,
    /// <see cref="DebugInfo.Function"/> will be set to the method name which invoked
    /// this constructor. See also: MethodBase.GetCurrentMethod()
    /// </summary>
    public DebugInfo(string? method = null)
    {
        var name = method ?? ConstructorEntry;
        Function = LocateCaller(name, out int temp);
        LineNumber = temp;
    }

    /// <summary>
    /// Constructor with function name and line number.
    /// </summary>
    public DebugInfo(string? func, int line)
    {
        Function = func;
        LineNumber = line;
    }

    /// <summary>
    /// Gets the calling function name, i.e. "ConsoleApp1.Program.Main(String[] args)".
    /// The value is null if unknown.
    /// </summary>
    public string? Function { get; }

    /// <summary>
    /// Gets the caller line number. The value is 0 if unknown.
    /// </summary>
    public int LineNumber { get; }

    /// <summary>
    /// Overrides. Example: "ConsoleApp1.Program.Main(String[] args) #47"
    /// </summary>
    public override string? ToString()
    {
        if (Function != null && LineNumber > 0)
        {
            return Function + " #" + LineNumber;
        }

        return Function;
    }

    private static string GetThreadName()
    {
        // Combination of pid and thread-name
        var temp = Thread.CurrentThread.Name;

        if (string.IsNullOrEmpty(temp))
        {
            temp = Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
        }

        return "0000-" + temp;
    }

    private static string? LocateCaller(string entryMethod, out int num)
    {
        // Get the root caller method name and line.
        // For example, the stack trace will look something like this:

        // at System.Environment.get_StackTrace()
        // at Tral.Oss.Logging.CallLogger.GetCallerName(Int32 stackDepth) in F:\TRAL\SOURCE\Tral.Oss.Logging\CallLogger.cs:line 280
        // at Tral.Oss.Logging.CallLogger.Write(Object val, String cat) in F:\TRAL\SOURCE\Tral.Oss.Logging\CallLogger.cs:line 216
        // at Tral.Oss.Logging.CallLogger.Debug(Object val) in F:\TRAL\SOURCE\Tral.Oss.Logging\CallLogger.cs:line 162
        // at ConsoleApp1.Program.Main(String[] args) in F:\TRAL\SOURCE\ConsoleApp1\Program.cs:line 20

        // Here, we would want to locate line after: Tral.Oss.Logging.CallLogger.Debug

        // Note, constructors look like:
        // Tral.MarketRobots.Archiver.Internal.ArchiverSink`1..ctor(ISeriesDBFactory dbFactory, String exchange)

        // Return null if fails.
        num = 0;
        string? caller = null;

        if (entryMethod.Length != 0)
        {
            var stack = Environment.StackTrace.Split('\n');

            for (int n = 0; n < stack.Length - 1; ++n)
            {
                int start = stack[n].IndexOf(entryMethod);

                if (start > -1)
                {
                    // Located, but keep going until no more
                    // found as may encounter overloaded names.
                    caller = stack[n+1].Substring(start).Trim();
                }
                else
                if (caller != null)
                {
                    break;
                }
            }
        }

        if (caller != null)
        {
            // Line number
            // Get last item and see if it convert to and integer.
            int pos = caller.LastIndexOf(' ');

            if (pos > -1)
            {
                int.TryParse(caller.Substring(pos + 1), out num);
            }

            // Locate end of method name
            pos = caller.IndexOf(" in ");
            if (pos < 0) pos = caller.IndexOf(")") + 1;

            // Remove file part if found, otherwise leave as is.
            if (pos > 0) // <- not accept start
            {
                caller = caller.Substring(0, pos).Trim();
            }
        }

        return caller;
    }

}