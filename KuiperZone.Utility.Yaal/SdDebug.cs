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
/// Structured RFC 5424 SD-ELEMENT. The class inherits <see cref="SdDictionary{T}"/>
/// where T is string, so as provide a sequence of SD-PARAM key-values.
/// </summary>
public sealed class SdDebug : SdElement
{
    private static readonly string ConstructorEntry = typeof(SdDebug).FullName + "..ctor";

    /// <summary>
    /// Contructor with SD-ID value. Note that locate the caller in the stack. The "entryMethod" should be the
    /// full name of the function which served as entry to the logging functionality. If not null,
    /// <see cref="SdDebug.Function"/> will be set to the method before it in the stack. If null,
    /// <see cref="SdDebug.Function"/> will be set to the caller of this constructor.
    /// </summary>
    public SdDebug(string id, string? entryMethod = null)
        : base(id)
	{
        Function = LocateCaller(entryMethod ?? ConstructorEntry, out int num);

        if (!string.IsNullOrEmpty(Function))
        {
            Add("FUNCTION", Function);

            if (num > 0)
            {
                LineNumber = num.ToString(CultureInfo.InvariantCulture);
                Add("LINE", LineNumber);
            }
        }
    }

    /// <summary>
    /// Gets the calling function name.
    /// </summary>
    public string? Function { get; }

    public string? LineNumber { get; }

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
                    caller = stack[n].Substring(start).Trim();
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