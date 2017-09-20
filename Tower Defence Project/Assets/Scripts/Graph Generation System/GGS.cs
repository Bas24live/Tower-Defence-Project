using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

public class GGS {
    private static GGS instance = null;

    private Graph host;
    private List<Production> productions;
    private List<Production> matchedProductions;

    private int iterations = 2;

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

        Random random = new Random();

        for (int i = 0; i < iterations; ++i) {
            if (ValidProducations()) {
                int randPos = random.Next(matchedProductions.Count);
                matchedProductions[randPos].ApplyToRandom(host);
            } else {
                break;
            }
        }

        return true;
    }

    //------------------------------------------------------------Graph Gen Methods------------------------------------------------------------//
    /* Check if there are any valid productions that can be applied to the host graph*/
    private bool ValidProducations() {
        //Release any left over elements and reallocate memory to save space 
        matchedProductions.Clear();
        matchedProductions.TrimExcess();

        for (int i = 0; i < productions.Count; ++i) {
            if (HasCandidates(host, productions[i])) {
                matchedProductions.Add(productions[i]);
            }
        }

        if (matchedProductions.Count == 0) {
            return false;
        }
        else {
            return true;
        }
    }

    /* Check if production matches any subgraphs of the host graph.
     * Return true if at least one match is found.
     * */
    public bool HasCandidates(Graph host, Production production) {
        List<List<Node>> matchedNodes = FindMatchedNodes(host.Nodes, production.LeftSide.Nodes);
        List<List<Node>> combonations = new List<List<Node>>();

        combonations = GenCombonations(matchedNodes, new List<Node>(), combonations, 0);

        bool result = GenCandidateGraphs(production, combonations);

        return result;
    }

    /* Find all nodes in host graph that match with nodes in the productions
     * left hand side and add them to a list. 
     * */
    private List<List<Node>> FindMatchedNodes(List<Node> hostNodes, List<Node> leftSide) {
        List<List<Node>> matchedNodes = new List<List<Node>>();

        foreach (Node leftNode in leftSide) {
            List<Node> temp = new List<Node>();
            foreach (Node hostNode in hostNodes) {
                if (leftNode.Type == hostNode.Type)
                    temp.Add(hostNode);
            }
            matchedNodes.Add(temp);
        }

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

    // GenCandidateGraphs Remove combonations that don't have the same amount of nodes as in the left hand side
    private bool GenCandidateGraphs(Production production, List<List<Node>> combonations) {
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
                    candidateGraphs.Add(new Graph(combonation, internalEdges));
                }
            }
        }

        if (candidateGraphs.Count == 0) {
            return false;
        }
        else {
            production.CandidateGraphs = candidateGraphs;
            return true;
        }
    }

    /* Check whether a combination is valid by comparing the edges in the left hand side and those in the subgraph
     * If all edges are connected in the same manner then the combination is valid and return true else return false
     * */
    private bool ValidCombonation(Graph productionLeftSide, List<Node> combination, List<Edge> internalEdges) {
        List<Node> nodes = productionLeftSide.Nodes;

        if (productionLeftSide.Edges.Count != internalEdges.Count)
            return false;

        for (int i = 0; i < nodes.Count - 1; ++i) {
            for (int j = i + 1; j < nodes.Count; ++j) {
                if (productionLeftSide.DoesEdgeExist(nodes[i], nodes[j]) || productionLeftSide.DoesEdgeExist(nodes[j], nodes[i])) {
                    if (!DoesEdgeExist(combination[i], combination[j], internalEdges)) {
                        return false;
                    }
                }
            }
        } 

        return true;
    }

    //------------------------------------------------------------Helper Methods------------------------------------------------------------//
    //Check if the given node is contained in the given list of nodes
    private bool DoesNodeExist(Node node, List<Node> nodes) {
        foreach (Node lNode in nodes)
            if (node.CompareType(lNode))
                return true;

        return false;
    }

    //Check if the given edge is connected between the two given nodes.
    private bool DoesEdgeExist(Node node1, Node node2, Edge edge) {
        if (edge.Source.CompareType(node1) && edge.Target.CompareType(node2) || edge.Source.CompareType(node2) && edge.Target.CompareType(node1))
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
        Node a = new Node("g00", "a", "start", new Vector3(0, 0, 0));
        Node b = new Node("g01", "b", "end", new Vector3(5, 0, 0));

        host.AddNode(a);
        host.AddNode(b);

        host.AddEdge(new Edge(a, b, "e01"));
    }

    private void GenProds() {
        productions.Add(Prod_1());
        productions.Add(Prod_2());
    }


    private Production Prod_1() {
        Node p00 = new Node("p00", "p00", "start", new Vector3(0, 0, 0));
        Node p01 = new Node("p01", "p01", "standard", new Vector3(2.5f, 0, 5));
        Node p02 = new Node("p02", "p02", "end", new Vector3(5, 0, 0));

        Production prod_1 = new Production("Produciton 1");

        //Left side of production
        prod_1.LeftSide.AddNode(p00);
        prod_1.LeftSide.AddNode(p02);
        prod_1.LeftSide.AddEdge(new Edge(p00, p02, "e00"));

        //Right side of production
        prod_1.RightSide.AddNode(p00);
        prod_1.RightSide.AddNode(p01);
        prod_1.RightSide.AddNode(p02);

        prod_1.RightSide.AddEdge(new Edge(p00, p02, "e00"));
        prod_1.RightSide.AddEdge(new Edge(p00, p01, "e01"));
        prod_1.RightSide.AddEdge(new Edge(p01, p02, "e02"));

        return prod_1;
    }

    private Production Prod_2() {
        Node p10 = new Node("p10", "p10", "standard", new Vector3(2.5f, 0, 5));
        Node p11 = new Node("p11", "p11", "standard", new Vector3(5, 0, -5));
        Node p12 = new Node("p12", "p12", "end", new Vector3(7.5f, 0, 0));

        Production prod_2 = new Production("Produciton 2");

        //Left side of production
        prod_2.LeftSide.AddNode(p10);
        prod_2.LeftSide.AddNode(p12);
        prod_2.LeftSide.AddEdge(new Edge(p10, p12, "e10"));

        //Right side of production
        prod_2.RightSide.AddNode(p10);
        prod_2.RightSide.AddNode(p11);
        prod_2.RightSide.AddNode(p12);

        prod_2.RightSide.AddEdge(new Edge(p10, p12, "e11"));
        prod_2.RightSide.AddEdge(new Edge(p11, p12, "e12"));

        return prod_2;
    }
}
