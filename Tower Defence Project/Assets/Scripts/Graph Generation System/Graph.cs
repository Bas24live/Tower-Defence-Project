using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class Graph {
    private string label;
    private List<Node> nodes;
    private List<Edge> edges;

    //------------------------------------------------------------Constructors------------------------------------------------------------//
    public Graph() {
        nodes = new List<Node>();
        edges = new List<Edge>();
    }

    public Graph(string label) {
        nodes = new List<Node>();
        edges = new List<Edge>();
    }

    public Graph(List<Node> nodes, List<Edge> edges) {
        this.nodes = nodes;
        this.edges = edges;
    }

    public Graph(List<Node> nodes, List<Edge> edges, string label) {
        this.nodes = nodes;
        this.edges = edges;
        this.label = label;
    }

    //--------------------------------------------------------------Mutators--------------------------------------------------------------//
    //Adds the given node to the list of nodes
    public void AddNode(Node node) {
        nodes.Add(node);
    }

    /* Removes a given node from the list of nodes in the graph
     * Returns true if the node was found and removed
     * Returns false if the node was not found and removed
     * */
    public bool RemoveNode(Node node) {
        foreach (Node gNode in nodes) {
            if (gNode.CompareType(node)) {
                nodes.Remove(node);
                return true;
            }
        }

        return false;
    }

    //Adds the given edge to the list of edges
    public void AddEdge(Edge edge) {
        edges.Add(edge);
    }

    /* Removes a given edge from the list of edges in the graph
     * Returns true if the edge was found and removed
     * Returns false if the edge was not found and removed
     * */
    public bool RemoveEdge(Node source, Node target) {
        foreach (Edge edge in edges) {
            if (edge.Source.CompareType(source) && edge.Target.CompareType(target)) {
                edges.Remove(edge);
                return true;
            }
        }

        return false;
    }

    /* Finds the old node in the list of existing nodes in the graph
     * once the old node is found it is replaced with the new node.
     * Returns a bool value indicating wether the replacement took place or not.
     */
    public bool Replace(Node oldNode, Node newNode) {
        foreach (Edge edge in edges) {
            if (edge.Source.CompareExact(oldNode)) {
                edge.Source = newNode;
            }
            else if (edge.Target.CompareExact(oldNode)) {
                edge.Target = newNode;
            }
        }

        for (int i = 0; i < nodes.Count; ++i) {
            if (nodes[i].CompareExact(oldNode)) {
                nodes.RemoveAt(i);
                nodes.Insert(i, newNode);
                return true;
            }
        }

        return false;
    }

    /* Finds the old edge in the list of existing edges in the graph
     * once the old edge is found it is replaced with the new edge.
     * Returns a bool value indicating wether the replacement took place or not.
     */
    public bool Replace(Edge oldEdge, Edge newEdge) {
        for (int i = 0; i < edges.Count; ++i) {
            if (edges[i].CompareExact(oldEdge)) {
                edges.RemoveAt(i);
                edges.Insert(i, newEdge);
                return true;
            }
        }

        return false;
    }

    /* Finds all old edges in the list of existing edges in the graph
     * and replaces them with the new edges.
     * Returns a bool value indicating wether the replacement took place or not.
     */
    public bool Replace(List<Edge> oldEdges, List<Edge> newEdges) {
        for (int i = 0; i < edges.Count; ++i) {
            for (int j = 0; j < oldEdges.Count; ++j) {
                if (edges[i].CompareExact(oldEdges[j])) {
                    edges.RemoveAt(i);
                    edges.Insert(i, newEdges[j]);
                    break;
                }
            }
        }
        return true;
    }

    /* Finds all old edges in the list of existing edges in the graph and removes them.
     */
    public void RemoveEdges(List<Edge> edges) {
        for (int i = 0; i < edges.Count; ++i) {
            for (int j = 0; j < this.edges.Count; ++j) {
                if (edges[i].CompareExact(this.edges[j])) {
                    this.edges.RemoveAt(j);
                    --j;
                    break;
                }
            }
        }
    }

    //Add the given list of edges to the hosts current list of edges
    public void AddEdges(List<Edge> edges) {
        foreach (Edge edge in edges) {
            AddEdge(edge);
        }
    }

    //Remove all nodes from the graph
    public void ClearNodes() {
        nodes.Clear();
        nodes.TrimExcess();
    }

    //Remove all edges from the graph
    public void ClearEdges() {
        edges.Clear();
        edges.TrimExcess();
    }

    /* Finds all edges that are connected to the given node
     * and then removes them from the graphs list of edges
     */
    public void CleanEdges(Node node) {
        for(int i = 0; i < edges.Count; ++i) {
            if (IsConnected(node, edges[i])) {
                edges.RemoveAt(i);
                --i;
            }
        }
    }

    //-----------------------------------------------------------------Helpers-----------------------------------------------------------------//
    /* Finds all edges that are connected to the given node
     * Returns a list of edges that have the given node as either a source or target node
     * */
    public List<Edge> GetConnectedEdges(Node node) {
        List<Edge> existingEdges = new List<Edge>();

        foreach (Edge edge in edges) {
            if (IsConnected(node, edge))
                existingEdges.Add(edge);
        }

        return existingEdges;
    }

    /* Find all edges that are connected within the list of given nodes
     * Return the resultant list
     * */
    public List<Edge> GetInternalEdges(List<Node> nodes) {
        List<Edge> internalEdges = new List<Edge>();

        foreach (Edge edge in edges) {
            for (int i = 0; i < nodes.Count - 1; ++i) {
                for (int j = i + 1; j < nodes.Count; ++j) {
                    if (IsConnected(nodes[i], edge) && IsConnected(nodes[j], edge)) { 

                        internalEdges.Add(edge);
                    }
                }
            }
        }

        return internalEdges;
    }

    /* Find all edges that are connected eternally of the list of given nodes
    * Return the resultant list
    * */
    public List<Edge> GetExternalEdges(List<Node> nodes) {
        List<Edge> internalEdges = GetInternalEdges(nodes);
        List<Edge> externalEdges = new List<Edge>();

        foreach (Edge edge in edges) {
            foreach (Node node in nodes) {
                if (IsConnected(node, edge) && !DoesEdgeExist(edge, internalEdges)) {
                    externalEdges.Add(edge);
                }
            }
        }

        return externalEdges;
    }

    //Checks if a given edge is attached to the given node
    public bool IsConnected(Node node, Edge edge) {
        if (edge.Source.CompareType(node) || edge.Target.CompareType(node))
            return true;
        else
            return false;
    }

    //Checks if a given edge is attached to any of the nodes in the given list
    public bool IsConnected(List<Node> nodes, Edge edge) {
        foreach (Node node in nodes) {
            if (edge.Source.CompareType(node) || edge.Target.CompareType(node))
                return true;
        }

        return false;
    }

    /* Checks whether there is an edge linking the given source and target nodes
     * Returns the result
     */
    public bool DoesEdgeExist(Node source, Node target) {
        foreach(Edge edge in edges) {
            if (edge.Source.CompareType(source) && edge.Target.CompareType(target))
                return true;
        }

        return false;
    }

    /* Checks whether the given edge exists in the list of given edges
    * Returns true if so : false otherwise
    */
    public bool DoesEdgeExist(Edge edge, List<Edge> edges) {
        foreach (Edge lEdge in edges) {
            if (edge.CompareType(lEdge))
                return true;
        }

        return false;
    }

    /* Checks whether the given node is contained in the graphs set of nodes.
     * Compares using the exact comparison.
     * Returns a boolean result of the findings.
     */
    public bool DoesNodeExist(Node node) {
        foreach (Node gNode in nodes) {
            if (node.CompareExact(gNode)) {
                return true;
            }
        }

        return false;
    }

    //--------------------------------------------------------------Accessors-------------------------------------------------------------//
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

    /* Compare the given edge to all edges in the graph and 
     * return the graphs edge upon finding it else return null.
     */
    public Edge GetEdge(Edge edge) {
        foreach (Edge gEdge in edges) {
            if (edge.CompareExact(gEdge)) {
                return gEdge;
            }
        }

        return null;
    }

    /* Compare the given nodes to all nodes in the graph and 
     * return the graphs node upon finding it else return null.
     */
    public Node GetNode(Node node) {
        foreach (Node gNode in nodes) {
            if (node.CompareExact(gNode)) {
                return gNode;
            }
        }

        return null;
    }

    //--------------------------------------------------------Serialization Methods-------------------------------------------------------//
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("label", label);
        info.AddValue("nodes", nodes);
        info.AddValue("edges", edges);
    }

    public Graph(SerializationInfo info, StreamingContext context) {
        label = (string)info.GetValue("label", typeof(string));
        nodes = (List<Node>)info.GetValue("nodes", typeof(List<Node>));
        edges = (List<Edge>)info.GetValue("edges", typeof(List<Edge>));
    }
}
