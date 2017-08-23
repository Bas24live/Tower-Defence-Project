using System;
using System.Collections.Generic;

public class GGS {

    private static GGS instance = null;

    private Graph host;
    private List<Production> productions;
    private List<Production> matchedProductions;

    protected GGS () {

    }

    public static GGS GetInstance () {
        if (instance == null)
            instance = new GGS();
        
        return instance;
    }

    //Generate and/or read in required variables
    private void Init () {
        host = new Graph ();    
        productions = new List<Production>();
        matchedProductions = new List<Production>();
    }

    public bool Run () {
        Random random = new Random();

        while (ValidProducations()) {
            int i = random.Next(matchedProductions.Count);
            matchedProductions[i].ApplyToRandom();
        }

        return true;
    }

    /* Check if there are any valid productions that can be applied to the host graph*/
    private bool ValidProducations () {
        //Release any left over elements and reallocate memory to save space 
        matchedProductions.Clear();
        matchedProductions.TrimExcess();

        for (int i = 0; i < productions.Count; ++i) {
            if (!HasCandidates(host, productions[i])) {
                matchedProductions.Add(productions[i]);
            }
        }

        if (matchedProductions.Count == 0)
            return false;
        else
            return true;
    }

    /* Check if production matches any subgraphs of the host graph.
     * Return true if at least one match is found.
     */
    public bool HasCandidates (Graph host, Production production) {
        List<List<Node>> matchedNodes = FindMatchedNodes(host.Nodes, production.LeftSide.Nodes);
        List<List<Node>> combonations = new List<List<Node>>();
        
        GenCombonations(matchedNodes, new List<Node>(), combonations, 0);

        return GenCandidateGraphs(production, combonations); 
    }

    /* Find all nodes in host graph that match with nodes in the productions
     * left hand side and add them to a list. 
     * */
    private List<List<Node>> FindMatchedNodes (List<Node> hostNodes, List<Node> leftSide)  {
        List<List<Node>> matchedNodes = new List<List<Node>>();
        int iCount = 0;

        foreach (Node leftNode in leftSide) {
            foreach (Node hostNode in hostNodes) { 
                if (leftNode.Type == hostNode.Type)
                    matchedNodes[iCount].Add(hostNode);
            }
            ++iCount;
        }
     
        return matchedNodes;
    }

    // Generate a list of all possible combonations of nodes without repeating existing combinations.
    private void GenCombonations (List<List<Node>> matchedNodes, List<Node> curNodes, List<List<Node>> combonations, int iCount) {
        for (int i = 0; i < matchedNodes[iCount].Count; ++i) {

            if (!DoesNodeExist(matchedNodes[iCount][i], curNodes)) {
                curNodes.Add(matchedNodes[iCount][i]);

                if (iCount < matchedNodes.Count - 1) {
                    GenCombonations(matchedNodes, curNodes, matchedNodes, ++iCount);
                    curNodes.RemoveAt(curNodes.Count - 1);
                    --iCount;
                }
                else {
                    List<Node> clone = new List<Node>(curNodes);
                    combonations.Add(clone);
                    curNodes.RemoveAt(curNodes.Count - 1);
                }
            }
        }
    }

    //Remove combonations that don't have the same amount of nodes as in the left hand side
    private bool GenCandidateGraphs (Production production, List<List<Node>> combonations) {
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

        if (candidateGraphs.Count == 0)
            return false;
        else {
            production.CandidateGraphs = candidateGraphs;
            return true;
        }
    }



    /* Check whether a combination is valid by comparing the edges in the left hand side and those in the subgraph
     * If all edges are connected in the same manner then the combination is valid and return true else return false
     * */
    private bool ValidCombonation (Graph productionLeftSide, List<Node> combination, List<Edge> internalEdges) {
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

    //Check if the given node is contained in the given list of nodes
    private bool DoesNodeExist(Node node, List<Node> nodes) {
        foreach (Node lNode in nodes)
            if (node.Compare(lNode))
                return true;

        return false;
    }

    //Check if the given edge is connected between the two given nodes.
    private bool DoesEdgeExist (Node node1, Node node2, Edge edge) {
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

    //--------------------------Read in XML data--------------------------//

}
