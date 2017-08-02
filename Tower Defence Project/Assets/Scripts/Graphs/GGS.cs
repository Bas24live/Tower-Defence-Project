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
        graph = new Graph();
        singleProductions = new List<SingleProduciton>();
        matchedProds = new List<Production>();
    }

    public bool IsMatch(Graph host, Production production) {
        List<Graph> possibleMatches = new List<Graph>();
        Graph lhSide = production.LhSide;
        List<Node> matchedNodes = FindNodes(host, lhSide);

        if (matchedNodes.Count <= production.LhSide.Nodes.Count)
            return false;

        Graph matchedSubGraph = new Graph();

        foreach (Node lNode in lhSide.Nodes) {
            List<Node> lLinks = lNode
        }


        return true;
    }

    private List<List<Node>> FindNodes(Graph host, Graph lhSide) {
        List<List<Node>> matchedNodes = new List<List<Node>>();
        int iCount = 0;
        
        foreach (Node node in lhSide.Nodes)
            foreach (Node hNode in host.Nodes) {
                if (node.Type == hNode.Type) 
                    matchedNodes[iCount].Add(hNode);
                ++iCount;
                }

        for (int i = 0; i < matchedNodes.Count; ++i) {
            List<Node> nodes = matchedNodes[i];

            for (int k = 0; k < nodes.Count; ++k) {
                for
            }
            iCount = 0;

        }
     
        return matchedNodes;
    }

    private bool Contains(List<Node> nodes, Node node) {
        foreach (Node sNode in nodes) {
            if (node == sNode)
                return true;
        }

        return false;
    }

    private bool Contains(List<List<Node>> existingCombos, List<Node> newCombo) {
        
    }

}
