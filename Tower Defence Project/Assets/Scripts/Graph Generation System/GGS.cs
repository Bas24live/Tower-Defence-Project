using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class GGS : MonoBehaviour {

    private static GGS instance = null;

    private Graph host;
    private List<Production> productions;
    private List<Production> matchedProductions;

    private int iterations = 1;

    protected GGS() {

    }

    public static GGS GetInstance() {
        if (instance == null)
            instance = new GGS();

        return instance;
    }

    //Generate and/or read in required variables
    private void Init() {
        host = new Graph();
        productions = new List<Production>();
        matchedProductions = new List<Production>();

        GenHost();
        GenProds();
    }

    public bool Run() {
        Init();

        System.Random random = new System.Random();

        for (int i = 0; i < iterations; ++i) {
            if (ValidProducations()) {
                int randPos = random.Next(matchedProductions.Count);
                matchedProductions[randPos].ApplyToRandom();
            } else {
                break;
            }
        }

        return true;
    }

    //------------------------------------------------------------Graph Gen Methods------------------------------------------------------------//
    /* Check if there are any valid productions that can be applied to the host graph*/
    private bool ValidProducations() {
        Debug.Log("ValidProductions Started...");
        //Release any left over elements and reallocate memory to save space 
        matchedProductions.Clear();
        matchedProductions.TrimExcess();

        for (int i = 0; i < productions.Count; ++i) {
            if (HasCandidates(host, productions[i])) {
                matchedProductions.Add(productions[i]);
            }
        }

        if (matchedProductions.Count == 0) {
            Debug.Log("ValidProductions Returned False...");
            return false;
        }
        else {
            Debug.Log("ValidProductions Returned True...");
            return true;
        }
    }

    /* Check if production matches any subgraphs of the host graph.
     * Return true if at least one match is found.
     * */
    public bool HasCandidates(Graph host, Production production) {
        Debug.Log("HasCandidates Started...");
        List<List<Node>> matchedNodes = FindMatchedNodes(host.Nodes, production.LeftSide.Nodes);
        List<List<Node>> combonations = new List<List<Node>>();

        combonations = GenCombonations(matchedNodes, new List<Node>(), combonations, 0);

        Debug.Log("GenCombonations Returned " + combonations.Count);

        bool result = GenCandidateGraphs(production, combonations);

        Debug.Log("HasCandidates Returned " + result);
        return result;
    }

    /* Find all nodes in host graph that match with nodes in the productions
     * left hand side and add them to a list. 
     * */
    private List<List<Node>> FindMatchedNodes(List<Node> hostNodes, List<Node> leftSide) {
        Debug.Log("FindMatchedNodes Started...");

        List<List<Node>> matchedNodes = new List<List<Node>>();

        foreach (Node leftNode in leftSide) {
            List<Node> temp = new List<Node>();
            foreach (Node hostNode in hostNodes) {
                if (leftNode.Type == hostNode.Type)
                    temp.Add(hostNode);
            }
            matchedNodes.Add(temp);
        }
        Debug.Log("FindMatchedNodes Finished...");

        return matchedNodes;
    }

    // Generate a list of all possible combonations of nodes without repeating existing combinations.
    private List<List<Node>> GenCombonations(List<List<Node>> matchedNodes, List<Node> curNodes, List<List<Node>> combonations, int iCount) {
        for (int i = 0; i < matchedNodes[iCount].Count; ++i) {

            if (!DoesNodeExist(matchedNodes[iCount][i], curNodes)) {
                curNodes.Add(matchedNodes[iCount][i]);

                if (iCount < matchedNodes.Count - 1) {
                    combonations = GenCombonations(matchedNodes, curNodes, matchedNodes, ++iCount);
                    curNodes.RemoveAt(curNodes.Count - 1);
                    --iCount;
                }
                else {
                    List<Node> temp = new List<Node>();

                    foreach (Node node in curNodes) {
                        temp.Add(node);
                    }

                    combonations.Add(temp);
                    curNodes.RemoveAt(curNodes.Count - 1);
                }
            }
        } 
        return combonations;
    }

    //Remove combonations that don't have the same amount of nodes as in the left hand side
    private bool GenCandidateGraphs(Production production, List<List<Node>> combonations) {
        Debug.Log("GenCandidateGraphs Started...");
        for (int i = 0; i < combonations.Count; ++i) {
            string combo = "";
            foreach (Node node in combonations[i]) {
                combo += node.label + "|";
            }

            Debug.Log(combo);
        }

        List<Graph> candidateGraphs = new List<Graph>();

        for (int i = 0; i < combonations.Count; ++i) {
            List<Node> combonation = combonations[i];

            if (combonation.Count != production.LeftSide.Nodes.Count) {
                combonations.RemoveAt(i);
                --i;
            }
            else {
                List<Edge> internalEdges = host.GetInternalEdges(combonation);

                if (!ValidCombonation(production.LeftSide, combonation, internalEdges)) {
                    combonations.RemoveAt(i);
                    --i;
                }
                else {
                    candidateGraphs.Add(new Graph(combonations[i], internalEdges));
                }
            }
        }

        if (candidateGraphs.Count == 0) {
            Debug.Log("GenCandidateGraphs Returned False...");
            Debug.Log("HasCandidates Finished...");
            return false;
        }
        else {
            Debug.Log("GenCandidateGraphs Returned True...");
            Debug.Log("HasCandidates Finished...");
            production.CandidateGraphs = candidateGraphs;
            return true;
        }
    }

    /* Check whether a combination is valid by comparing the edges in the left hand side and those in the subgraph
     * If all edges are connected in the same manner then the combination is valid and return true else return false
     * */
    private bool ValidCombonation(Graph productionLeftSide, List<Node> combination, List<Edge> internalEdges) {
        Debug.Log("ValidCombonation Started...");

        List<Node> nodes = productionLeftSide.Nodes;

        if (productionLeftSide.Edges.Count != internalEdges.Count)
            return false;

        for (int i = 0; i < nodes.Count - 1; ++i) {
            for (int j = i + 1; j < nodes.Count; ++j) {
                if (productionLeftSide.DoesEdgeExist(nodes[i], nodes[j]) || productionLeftSide.DoesEdgeExist(nodes[j], nodes[i])) {
                    if (!DoesEdgeExist(combination[i], combination[j], internalEdges)) {
                        Debug.Log("ValidCombonation Returned False...");

                        return false;
                    }
                }
            }
        }
        Debug.Log("ValidCombonation Returned True...");

        return true;
    }

    //------------------------------------------------------------Helper Methods------------------------------------------------------------//
    //Check if the given node is contained in the given list of nodes
    private bool DoesNodeExist(Node node, List<Node> nodes) {
        foreach (Node lNode in nodes)
            if (node.Compare(lNode))
                return true;

        return false;
    }

    //Check if the given edge is connected between the two given nodes.
    private bool DoesEdgeExist(Node node1, Node node2, Edge edge) {
        if (edge.Source.Compare(node1) && edge.Target.Compare(node2) || edge.Source.Compare(node2) && edge.Target.Compare(node1))
            return true;

        return false;
    }

    //Check if there is an edge from the given list of edges that is connected between the two given nodes.
    private bool DoesEdgeExist(Node node1, Node node2, List<Edge> edges) {
        foreach (Edge edge in edges) {
            if (DoesEdgeExist(node1, node2, edge)) {
                return true;
            }
        }

        return false;
    }

    //------------------------------------------------------------Accessor Methods------------------------------------------------------------//
    public Graph Host {
        get {
            return host;
        }
        
        set {
            host = value;
        }
    }

    //------------------------------------------------------------Read in XML data------------------------------------------------------------//
    // Write the productions and host graph to the specified files
    public void WriteXML() {         
        using (Stream fs = new FileStream(@"C: \Users\bas24\Downloads\testG.graph", FileMode.Create, FileAccess.Write, FileShare.None)) {
            XmlSerializer serializerGraph = new XmlSerializer(typeof(Graph));
            serializerGraph.Serialize(fs, host);    
        }

        using (Stream fs = new FileStream(@"C: \Users\bas24\Downloads\testP.productions", FileMode.Create, FileAccess.Write, FileShare.None)) {
            XmlSerializer serializerProductions = new XmlSerializer(typeof(List<Production>));
            serializerProductions.Serialize(fs, host);
        }
    }

    // Read the productions and host graph from the specified files
    public void ReadXML() {

        using (FileStream fs = File.OpenRead(@"C:\Users\bas24\Downloads\testG.graph")) {
            XmlSerializer deserializerGraph = new XmlSerializer(typeof(Graph));
            host = (Graph)deserializerGraph.Deserialize(fs);
        }

        using (FileStream fs = File.OpenRead(@"C:\Users\bas24\Downloads\testP.productions")) {
            XmlSerializer deserializerProducitons = new XmlSerializer(typeof(List<Production>));
            productions = (List<Production>)deserializerProducitons.Deserialize(fs);
        }
    }

    //------------------------------------------------------------Gen Productions and Host------------------------------------------------------------//
    private void GenHost() {
        Node a = new Node("000", "a", "start", new Vector3(0, 0, 0));
        Node b = new Node("001", "b", "standard", new Vector3(5, 0, 0));
        Node c = new Node("002", "c", "end", new Vector3(0, 5, 0));

        host.AddNode(a);
        host.AddNode(b);
        host.AddNode(c);

        host.AddEdge(new Edge(a, b));
        host.AddEdge(new Edge(b, c));

    }

    private void GenProds() {
        Node a = new Node("000", "a", "start", new Vector3(0, 0, 0));
        Node b = new Node("001", "b", "standard", new Vector3(5, 0, 0));
        Node c = new Node("002", "c", "standard", new Vector3(0, -5, 0));

        Production prod = new Production("Test");

        //Left side of production
        prod.LeftSide.AddNode(a);
        prod.LeftSide.AddNode(b);
        prod.LeftSide.AddEdge(new Edge(a, b));

        //Right side of production
        prod.RightSide.AddNode(c);
        prod.RightSide.AddEdge(new Edge(b, c));

        productions.Add(prod);
    }

}
