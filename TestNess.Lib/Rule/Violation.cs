// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace TestNess.Lib.Rule
{
    public class Violation
    {
        public string Message { get; private set; }
        public IRule Rule { get; private set; }
        public TestCase TestCase { get; private set; }

        /// <summary>
        /// If non-<c>null</c>, contains the URL of the document (source file) that contains the
        /// code for which this violation was generated. This member can be <c>null</c> if debug 
        /// symbols could not be loaded.
        /// </summary>
        public string DocumentUrl { get; private set; }

        /// <summary>
        /// If non-<c>null</c>, contains line number information for the piece of code for which
        /// this violation was generated. This member can be <c>null</c> if debug symbols could
        /// not be loaded, or if the violation doesn't apply to a particular piece of code but
        /// to a method in general.
        /// </summary>
        public Coordinates Location { get; private set; }

        public Violation(IRule rule, TestCase testCase, string message = null) : this(rule, testCase, null, message)
        {
        }

        public Violation(IRule rule, TestCase testCase, Instruction instruction, string message = null)
        {
            Rule = rule;
            TestCase = testCase;
            Message = message ?? CreateDefaultMessage(rule);

            if (instruction != null)
            {
                InitLocation(instruction);
            }
            else
            {
                InitLocation(testCase.TestMethod);
            }
        }

        private static string CreateDefaultMessage(IRule rule)
        {
            return string.Format("violation of \"{0}\"", rule);
        }

        private void InitLocation(MethodDefinition method)
        {
            // Get the document URL from the first instruction. Source location for the method
            // definition (name, parameters, etc.) is apparently not stored in the PDB file.
            var instruction = method.Body.Instructions.FirstOrDefault(i => i.SequencePoint != null);
            if (instruction == null) return;
            var sp = instruction.SequencePoint;
            if (sp == null) return;
            DocumentUrl = sp.Document.Url;
        }

        private void InitLocation(Instruction instruction)
        {
            var sp = instruction.FindSequencePoint();
            if (sp == null) return;
            DocumentUrl = sp.Document.Url;
            Location = new Coordinates
                           {
                               StartLine = sp.StartLine,
                               EndLine = sp.EndLine,
                               StartColumn = sp.StartColumn,
                               EndColumn = sp.EndColumn
                           };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(DocumentUrl ?? TestCase.TestMethod.DeclaringType.FullName);
            sb.Append("::");
            sb.Append(TestCase.TestMethod.Name);
            sb.Append("(");
            sb.Append(Location != null ? StartCoordinates(Location) : TestCase.TestMethod.NameWithParameters());
            sb.Append("): ");
            sb.Append(Message);
            return sb.ToString();
        }

        private static string StartCoordinates(Coordinates location)
        {
            return string.Format("{0},{1}", location.StartLine, location.StartColumn);
        }

        public class Coordinates
        {
            public int StartLine { get; internal set; }
            public int EndLine { get; internal set; }
            public int StartColumn { get; internal set; }
            public int EndColumn { get; internal set; }
        }
    }
}
