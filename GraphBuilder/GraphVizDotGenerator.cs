using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphBuilder
{
    public class GraphVizDotGenerator<TNode> where TNode: class
    {
        private static readonly string NL = Environment.NewLine;

        private readonly Graph<TNode> _graph;
        private readonly Func<TNode, string> _nodeIdGetter;
        private readonly Func<TNode, string> _nodeDescriber;

        public GraphVizDotGenerator(Graph<TNode> graph, Func<TNode, string> nodeIdGetter, Func<TNode, string> nodeDescriber)
        {
            _graph = graph;
            _nodeIdGetter = nodeIdGetter;
            _nodeDescriber = nodeDescriber;
        }

        public string CreateDot(string graphName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("digraph \"{0}\" {{{1}", graphName, NL);

            var edgeSet = new HashSet<Edge>();

            // Create an entry for each node. Collect unique edges while we're at it.
            foreach (var node in _graph.Walk())
            {
                var id = _nodeIdGetter(node);
                sb.AppendLine(NodeEntry(node));

                foreach (var edge in _graph.HeadsFor(node).Select(head => new Edge(id, _nodeIdGetter(head))))
                {
                    edgeSet.Add(edge);
                }
            }

            // Create edge entries
            foreach (var edge in edgeSet)
            {
                sb.AppendLine(EdgeEntry(edge));
            }

            // Finally create a start node and an edge from that to the graph root
            sb.AppendLine(NodeEntry("_START", "START"));
            sb.AppendLine(EdgeEntry(new Edge("_START", _nodeIdGetter(_graph.Root))));

            sb.AppendLine("}");
            return sb.ToString();
        }

        private string NodeEntry(TNode node)
        {
            return NodeEntry(_nodeIdGetter(node), _nodeDescriber(node));
        }

        private static string NodeEntry(string id, string label)
        {
            return string.Format("{0} [label=\"{1}\"];", id, label.Replace("\"", "\\\""));
        }

        private static string EdgeEntry(Edge edge)
        {
            return string.Format("{0} -> {1};", edge.SourceId, edge.TargetId);
        }

        private struct Edge
        {
            internal readonly string SourceId;
            internal readonly string TargetId;

            internal Edge(string sid, string tid) : this()
            {
                SourceId = sid;
                TargetId = tid;
            }
        }
    }
}
