using System;

namespace GraphBuilder.Test
{
    internal interface INode
    {
    }

    internal class NameNode : INode
    {
        public string Text { get; set; }
    }

    internal class PartNode : INode
    {
        public string Part { get; set; }

        public override bool Equals(object obj)
        {
            var pn = obj as PartNode;
            return pn != null && Equals(pn.Part, Part);
        }

        public override int GetHashCode()
        {
            return Part != null ? Part.GetHashCode() : 0;
        }

        internal static string Strip(string text)
        {
            var idx = text.LastIndexOf('.');
            if (idx < 0)
                throw new ArgumentException("No parts.");
            return text.Substring(0, idx);
        }
    }

    internal class RootNode : INode
    {
        public static RootNode Instance = new RootNode();
    }
}