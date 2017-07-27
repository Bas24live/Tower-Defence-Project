using System.Collections.Generic;

public class Graph {

    List<Node> nodes;
    List<Edge> edges;

    public Graph() {
        nodes = new List<Node>();
        edges = new List<Edge>();
    }

    public void AddNode (Node node) {
        nodes.Add(node);
    }

    public void AddEdge (Edge edge) {
        edges.Add(edge);
    }

    public bool removeNode (string label) {
        foreach (Node node in nodes) {
            if (node.Label == label) {
                nodes.Remove(node);
                return true;
            }
        }

        return false;
    }

    public bool removeEdge (Node source, Node target) {
        foreach (Edge edge in edges) {
            if (edge.Source == source && edge.Target == target) {
                edges.Remove(edge);
                return true;
            }
        }

        return false;
    }

    public bool findMatch () {
        return false;
    }

    public bool applyProduciton () {
        return false;
    }
}
