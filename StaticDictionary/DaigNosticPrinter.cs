using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticDictionary.Interface
{
    public partial class DiagnosticPrinter
    {
        public static void PrintDiagnostics()
        {
            InternalPrintDiagnostics();
        }

        static partial void InternalPrintDiagnostics();
    }
}
