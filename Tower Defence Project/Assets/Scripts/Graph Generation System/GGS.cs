using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

public class GGS {
    private static GGS instance = null;

    private Graph host;
    private List<Production> productions;
    private List<Production> matchedProductions;

    private int iterations = 5;

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
        Node a = new Node("000", "a", "start", new Vector3(0, 0, 0));
        Node b = new Node("001", "b", "standard", new Vector3(5, 0, 0));
        Node c = new Node("002", "c", "end", new Vector3(0, 5, 0));

        host.AddNode(a);
        host.AddNode(b);
        host.AddNode(c);

        host.AddEdge(new Edge(a, b, "001"));
        host.AddEdge(new Edge(b, c, "002"));
    }

    private void GenProds() {
        Node a = new Node("000", "v", "start", new Vector3(0, 0, 0));
        Node b = new Node("001", "f", "standard", new Vector3(10, 0, 0));
        Node c = new Node("002", "k", "standard", new Vector3(0, 0, -1));

        Production prod = new Production("Test");

        //Left side of production
        prod.LeftSide.AddNode(a);
        prod.LeftSide.AddNode(b);
        prod.LeftSide.AddEdge(new Edge(a, b, "011"));

        //Right side of production
        prod.RightSide.AddNode(a);
        prod.RightSide.AddNode(b);
        prod.RightSide.AddNode(c);

        prod.RightSide.AddEdge(new Edge(a, b, "011"));
        prod.RightSide.AddEdge(new Edge(b, c, "012"));

        productions.Add(prod);

        Node f = new Node("000", "f", "standard", new Vector3(13, 0, 0));
        Node x = new Node("001", "x", "standard", new Vector3(12, 0, 4));
        Node z = new Node("002", "z", "end", new Vector3(12, 0, 10));

        Production prod2 = new Production("Test2");

        //Left side of production
        prod2.LeftSide.AddNode(f);
        prod2.LeftSide.AddNode(z);
        prod2.LeftSide.AddEdge(new Edge(f, z, "021"));

        //Right side of production
        prod2.RightSide.AddNode(x);
        prod2.RightSide.AddNode(f);
        prod2.RightSide.AddNode(z);
        prod2.RightSide.AddEdge(new Edge(x, z, "022"));
        prod2.RightSide.AddEdge(new Edge(x, f, "023"));
        prod2.RightSide.AddEdge(new Edge(z, f, "024"));

        productions.Add(prod2);
    }

}
