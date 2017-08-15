using System.Collections.Generic;
using UnityEngine;

public class GGS {
    Graph host;
    List<Production> productions;
    List<Production> matchedProds;

    public void Start() {
        Init();
    }

    public GGS() {
        Init();
    }

    private void Init() {
        host = new Graph ();    
        productions = new List<Production>();
        matchedProds = new List<Production>();

        for (int i = 0; i < productions.Count; ++i)
            if (!HasCandidates(host, productions[i])) {
                productions.RemoveAt(i);
                --i;
            }
    }

    /* Check if production matches an subgraphs of the host graph.
     * Return true if at least one match is found.
     */
    public bool HasCandidates(Graph host, Production production) {
        List<List<Node>> matchedNodes = FindNodes(host.Nodes, production.LeftSide.Nodes);
        List<List<Node>> combonations = new List<List<Node>>();
        List<Graph> candidateGraphs = new List<Graph>();
        GenCombonations(matchedNodes, new List<Node>(), combonations, 0);

        //Remove combonations that don't have the same amount of nodes as in the left hand side
        for(int i = 0; i < combonations.Count; ++i) {
            List<Node> combonation = combonations[i];

            if (combonation.Count != production.LeftSide.Nodes.Count) {
                combonations.RemoveAt(i);
                --i;
            }
            else {
                List<Edge> connectedEdges = GetConnectedEdges();
                if (!ValidCombonation(production.LeftSide, combonation, connectedEdges)) {
                    combonations.RemoveAt(i);
                    --i;
                } else {
                    candidateGraphs.Add(new Graph(combonations[i], connectedEdges));
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

    /* Find all nodes in host graph that match with nodes in the productions
     * left hand side and add them to a list. 
     */
    private List<List<Node>> FindNodes(List<Node> hostNodes, List<Node> leftSide) {
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
    private void GenCombonations(List<List<Node>> matchedNodes, List<Node> curNodes, List<List<Node>> combonations, int iCount) {
        for (int i = 0; i < matchedNodes[iCount].Count; ++i) {

            if (!Contains(curNodes, matchedNodes[iCount][i])) {
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

    //Check if the list contains the given node
    private bool Contains(List<Node> list, Node newNode) {
        foreach (Node node in list)
            if (newNode == node)
                return true;

        return false;
    }

    private List<Edge> GetConnectedEdges() {
        List<Edge> connectedEdges = new List<Edge>();

        //Add all edges that are connected to the nodes in the combination list
        foreach (Node node in host.Nodes) {
            foreach (Edge edge in host.GetEdges(node)) {
                bool exists = false;
                foreach (Edge existingEdge in connectedEdges) {
                    if (edge == existingEdge) {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                    connectedEdges.Add(edge);
            }
        }

        return connectedEdges;
    }

    //Check that all edges in the produciton are contained in the combonation else combonation is not usable
    private bool ValidCombonation(Graph leftSide, List<Node> combination, List<Edge> connectedEdges) {
        List<Node> leftNodes = leftSide.Nodes;        

        //Check whether a combination is valid by comparing the edges in the left hand side and those in the subgraph
        for (int i = 0; i < leftNodes.Count - 1; ++i) {
            for (int j = i + 1; j < leftNodes.Count; ++j) {
                if (leftSide.DoesEdgeExist(leftNodes[i], leftNodes[j]) || leftSide.DoesEdgeExist(leftNodes[j], leftNodes[i])) {
                    bool edgeExists = false;
                    for (int k = 0; k < connectedEdges.Count; ++k) {
                        if (DoesEdgeExist(connectedEdges[k], combination[i], combination[j])) {
                            connectedEdges.RemoveAt(k);
                            edgeExists = true;
                            break;
                        }
                    }
                    if (!edgeExists)
                        return false;
                }
            }
        }


        return true;
    }

    //Check if the given edge is connected between the two given nodes.
    private bool DoesEdgeExist (Edge edge, Node node1, Node node2) {
    if (edge.Source == node1 && edge.Target == node2 || edge.Source == node2 && edge.Target == node1)
        return true;

    return false;
    }
}
