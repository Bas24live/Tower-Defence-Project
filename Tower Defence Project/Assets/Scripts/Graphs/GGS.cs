using System.Collections.Generic;
using UnityEngine;

public class GGS {
    Graph graph;
    List<SingleProduciton> singleProductions;
    List<Production> matchedProds;

    public void Start() {
        Init();
    }

    public GGS() {
        Init();
    }

    private void Init() {
        graph = new Graph ();
        singleProductions = new List<SingleProduciton>();
        matchedProds = new List<Production>();
    }

    /* Check if production matches an subgraphs of the host graph.
     * Return true if at least one match is found.
     */
    public bool HasCandidates(Graph host, Production production) {
        List<List<Node>> matchedNodes = FindNodes(host.Nodes, production.LeftSide.Nodes);
        List<List<Node>> combonations = new List<List<Node>>();
        GenCombonations(matchedNodes, new List<Node>(), combonations, 0);

        //Remove combonations that don't have the same amount of nodes as in the left hand side
        for(int i = 0; i < combonations.Count; ++i) {
            List<Node> combonation = combonations[i];

            if (combonation.Count != production.LeftSide.Nodes.Count) {
                combonations.RemoveAt(i);
                --i;
            }
            else 
                if(!ValidCombonation(production.LeftSide, host, combonation)) {
                    combonations.RemoveAt(i);
                    --i;
                }
        }

        if (combonations.Count == 0)
            return false;
        else {
            production.Candidates = combonations;
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

    //Check that all edges in the produciton are contained in the combonation else combonation is not usable
    private bool ValidCombonation(Graph leftSide, Graph host, List<Node> combination) {
        List<Node> leftNodes = leftSide.Nodes;

        for (int i = 0; i < leftNodes.Count - 1; ++i)
            for (int j = i + 1; j < leftNodes.Count; ++j) {
                if (leftSide.DoesEdgeExist(leftNodes[i], leftNodes[j]) || leftSide.DoesEdgeExist(leftNodes[j], leftNodes[i]))
                    if (host.DoesEdgeExist(combination[i], combination[j]) || host.DoesEdgeExist(combination[j], combination[i]))
                        continue;
                    else
                        return false;
                        
            }

        return true;
    }
}
