using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using Microsoft.CodeAnalysis.Diagnostics;
using DiagnosticAnalyzerAndCodeFix.Maintainability;

namespace CodingGuidelines.Test.Maintainability
{
    [TestClass]
    public class AV1530Tests : CodeFixVerifier
    {
        [TestMethod]
        public void ForeachSimpleStatementAssignment()
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
                                    var list = new List<int>();
                                    foreach(var item in list)
                                        item = null;
                                }
                            }
                        }
                        ";

            var expected = new DiagnosticResult
            {
                Id = "AV1530",
                Message = "Don’t change a loop variable inside a for or foreach loop",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 41)
                        }
            };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void ForeachSimpleStatementNoAssignment()
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
                                    var list = new List<int>();
                                    foreach(var item in list)
                                        ;
                                }
                            }
                        }
                        ";

            VerifyCSharpDiagnostic(code);
        }

        [TestMethod]
        public void ForeachBlockStatementWithAssignment()
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
                                    var list = new List<int>();
                                    foreach(var item in list)
                                    {
                                        if(true)
                                            item = null;
                                    }
                                }
                            }
                        }
                        ";

            var expected = new DiagnosticResult
            {
                Id = "AV1530",
                Message = "Don’t change a loop variable inside a for or foreach loop",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 45)
                        }
            };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void ForeachBlockStatementNoAssignment()
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
                                    var list = new List<int>();
                                    foreach(var item in list)
                                    {
                                    }
                                }
                            }
                        }
                        ";

            VerifyCSharpDiagnostic(code);
        }

        [TestMethod]
        public void ForSimpleStatementAssignment()
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
                                    var list = new List<int>();
                                    for(int i = 0; i < 10; ++i)
                                        i = 10;
                                }
                            }
                        }
                        ";

            var expected = new DiagnosticResult
            {
                Id = "AV1530",
                Message = "Don’t change a loop variable inside a for or foreach loop",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 41)
                        }
            };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void ForSimpleStatementNoAssignment()
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
                                    var list = new List<int>();
                                    for(int i = 0; i < 10; ++i)
                                        ;
                                }
                            }
                        }
                        ";

            VerifyCSharpDiagnostic(code);
        }

        [TestMethod]
        public void ForBlockStatementWithAssignment()
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
                                    var list = new List<int>();
                                    for(int i = 0; i < 10; ++i)
                                    {
                                        if(true)
                                            i = 10;
                                    }
                                }
                            }
                        }
                        ";

            var expected = new DiagnosticResult
            {
                Id = "AV1530",
                Message = "Don’t change a loop variable inside a for or foreach loop",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 45)
                        }
            };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void ForBlockStatementNoAssignment()
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
                                    var list = new List<int>();
                                    for(int i = 0; i < 10; ++i
                                    {
                                    }
                                }
                            }
                        }
                        ";

            VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AV1530();
        }
    }
}