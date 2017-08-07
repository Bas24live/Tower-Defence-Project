using System.Collections.Generic;

public class Graph {

    List<Node> nodes;
    List<Edge> edges;

    public Graph() {
        nodes = new List<Node>();
        edges = new List<Edge>();
    }

    public void AddNode(Node node) {
        nodes.Add(node);
    }

    public void AddEdge(Edge edge) {
        edges.Add(edge);
    }

    public bool RemoveNode(string label, string type) {
        foreach (Node node in nodes) {
            if (node.Label == label && node.Type == type) {
                nodes.Remove(node);
                return true;
            }
        }

        return false;
    }

    public bool RemoveEdge(Node source, Node target) {
        foreach (Edge edge in edges) {
            if (edge.Source == source && edge.Target == target) {
                edges.Remove(edge);
                return true;
            }
        }

        return false;
    }

    public bool ApplyProduciton() {
        return false;
    }

    public void ClearNodes() {
        nodes = new List<Node>();
    }

    public void ClearEdges() {
        edges = new List<Edge>();
    }


    public List<Edge> GetEdges(Node source) {
        List<Edge> existingEdges = new List<Edge>();
        foreach (Edge edge in edges) {
            if (edge.Source == source)
                existingEdges.Add(edge);
        }

        return existingEdges;
    }

    public bool DoesEdgeExist(Node source, Node target) {
        foreach(Edge edge in edges) {
            if (edge.Source == source && edge.Target == target)
                return true;
        }

        return false;
    }

    //--------------------------Accessors Methods--------------------------//

    public List<Node> Nodes {
        get {
            return nodes;
        }
    }

    public List<Edge> Edges {
        get {
            return edges;
        }
    }
}
