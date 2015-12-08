using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using Microsoft.CodeAnalysis.Diagnostics;
using DiagnosticAnalyzerAndCodeFix.Maintainability;
using System.Linq;

namespace CodingGuidelines.Test.Maintainability
{
    [TestClass]
    public class AV1510Tests : CodeFixVerifier
    {
        [TestMethod]
        public void QualifiedName()
        {
            var code = @"
                        using System;
                        using System.Linq;

                        namespace ConsoleApp1
                        {
                            public class Test0
                            {
                                public void Tested()
                                {
                                    var list = new System.Collections.Generic.List<string>();
                                }
                            }
                        }
                        ";

            var expected = new DiagnosticResult
            {
                Id = "AV1510",
                Message = "Use using statements instead of fully qualified type name",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                        new[] {
                                new DiagnosticResultLocation("Test0.cs", 11, 52)
                            }
            };

            // 3 times for: 'System.Collections.Generic.List<>'; 'System.Collections.Generic'; 'System.Collections'
            VerifyCSharpDiagnostic(code, Enumerable.Range(0, 3).Select(i => expected).ToArray());
        }

        [TestMethod]
        public void MemberAccess()
        {
            var code = @"
                        using System;
                        using System.Linq;

                        namespace ConsoleApp1
                        {
                            public class Test0
                            {
                                public void Tested()
                                {
                                    var memberAccess = String.Empty;
                                }
                            }
                        }
                        ";

            VerifyCSharpDiagnostic(code);
        }
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AV1510();
        }
    }
}