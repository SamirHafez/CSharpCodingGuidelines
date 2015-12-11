using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using Microsoft.CodeAnalysis.Diagnostics;
using DiagnosticAnalyzerAndCodeFix.Maintainability;

namespace CodingGuidelines.Test.Maintainability
{
    [TestClass]
    public class AV1570Tests : CodeFixVerifier
    {
        [TestMethod]
        public void AccessWithoutChecking()
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
                                    var obj = new Object();
                                    
                                    var s = obj as String;

                                    var toString = s.ToString();
                                }
                            }
                        }
                        ";

            var expected = new DiagnosticResult
            {
                Id = "AV1570",
                Message = "Always check the result of an as operation",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 52)
                        }
            };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void AccessCheckingWithStatement()
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
                                    var obj = new Object();
                                    
                                    var s = obj as String;

                                    if(s != null)
                                        var toString = s.ToString();
                                }
                            }
                        }
                        ";

            VerifyCSharpDiagnostic(code);
        }

        [TestMethod]
        public void AccessCheckingWithBlock()
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
                                    var obj = new Object();
                                    
                                    var s = obj as String;

                                    if(s != null)
                                    {
                                        var toString = s.ToString();
                                    }
                                }
                            }
                        }
                        ";

            VerifyCSharpDiagnostic(code);
        }

        [TestMethod]
        public void AccessCheckingWithTernary()
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
                                    var obj = new Object();
                                    
                                    var s = obj as String;

                                    var toString = s != null ? s.ToString() : null;
                                }
                            }
                        }
                        ";

            VerifyCSharpDiagnostic(code);
        }

        //[TestMethod]
        //public void AccessCheckingWithReturnCut()
        //{
        //    var code = @"
        //                using System;
        //                using System.Linq;

        //                namespace ConsoleApp1
        //                {
        //                    public class Test0
        //                    {
        //                        public void Tested()
        //                        {
        //                            var obj = new Object();
                                    
        //                            var s = obj as String;

        //                            if(s == null)
        //                                return;
                                    
        //                            var toString = s.ToString();
        //                        }
        //                    }
        //                }
        //                ";

        //    VerifyCSharpDiagnostic(code);
        //}

        //[TestMethod]
        //public void AccessCheckingWithBreakCut()
        //{
        //    var code = @"
        //                using System;
        //                using System.Linq;

        //                namespace ConsoleApp1
        //                {
        //                    public class Test0
        //                    {
        //                        public void Tested()
        //                        {
        //                            var obj = new Object();

        //                            while(true)
        //                            {
        //                                var s = obj as String;

        //                                if(s == null)
        //                                    break;
                                        
        //                                var toString = s.ToString();
        //                            } 
        //                        }
        //                    }
        //                }
        //                ";

        //    VerifyCSharpDiagnostic(code);
        //}

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AV1570();
        }
    }
}