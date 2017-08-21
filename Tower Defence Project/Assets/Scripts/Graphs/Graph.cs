using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class Graph {
    private string label;
    private List<Node> nodes;
    private List<Edge> edges;

    public Graph () {
        nodes = new List<Node>();
        edges = new List<Edge>();
    }

    public Graph (string label) {
        nodes = new List<Node>();
        edges = new List<Edge>();
    }

    public Graph (List<Node> nodes, List<Edge> edges) {
        this.nodes = nodes;
        this.edges = edges;
    }

    public Graph (List<Node> nodes, List<Edge> edges, string label) {
        this.nodes = nodes;
        this.edges = edges;
        this.label = label;
    }

    //Adds a node to the list of nodes contained in this graph
    public void AddNode (Node node) {
        nodes.Add(node);
    }

    /* Removes a given node from the list of nodes in the graph
     * returns true if the node was found and removed
     * returns false if the node was not found and removed
     */
    public bool RemoveNode (Node node) {
        foreach (Node gNode in nodes) {
            if (gNode.Compare(node)) {
                nodes.Remove(node);
                return true;
            }
        }

        return false;
    }

    //Adds an edge to the list of edges contained in this graph
    public void AddEdge (Edge edge) {
        edges.Add(edge);
    }
    /* Removes a given edge from the list of edges in the graph
     * returns true if the edge was found and removed
     * returns false if the edge was not found and removed
     */
    public bool RemoveEdge (Node source, Node target) {
        foreach (Edge edge in edges) {
            if (edge.Source.Compare(source) && edge.Target.Compare(target)) {
                edges.Remove(edge);
                return true;
            }
        }

        return false;
    }

    //Remove all nodes from the graph
    public void ClearNodes () {
        nodes.Clear();
    }

    //Remove all edges from the graph
    public void ClearEdges () {
        edges.Clear();
    }

    /* Finds all edges that are connected to the given node
     * returns a list of edges that have the given node as either a source or target node
     */
    public List<Edge> GetEdges (Node node) {
        List<Edge> existingEdges = new List<Edge>();

        foreach (Edge edge in edges) {
            if (edge.Source.Compare(node) || edge.Target.Compare(node))
                existingEdges.Add(edge);
        }

        return existingEdges;
    }

    /* Checks whether there is an edge linking the given source and target nodes
     * returns the result
     */
    public bool DoesEdgeExist (Node source, Node target) {
        foreach(Edge edge in edges) {
            if (edge.Source.Compare(source) && edge.Target.Compare(target))
                return true;
        }

        return false;
    }

    //--------------------------Accessors Methods--------------------------//

    public List<Node> Nodes {
        get {
            return nodes;
        }

        set {
            nodes = value;
        }
    }

    public List<Edge> Edges {
        get {
            return edges;
        }

        set {
            edges = value;
        }
    }

    public string Label {
        get {
            return label;
        }

        set {
            label = value;
        }
    }

    //-----------------------------Serialization Methods-----------------------------//
    public void GetObjectData (SerializationInfo info, StreamingContext context) {
        info.AddValue("label", label);
        info.AddValue("nodes", nodes);
        info.AddValue("edges", edges);
    }

    public Graph (SerializationInfo info, StreamingContext context) {
        label = (string)info.GetValue("label", typeof(string));
        nodes = (List<Node>)info.GetValue("nodes", typeof(List<Node>));
        edges = (List<Edge>)info.GetValue("edges", typeof(List<Edge>));
    }
}
