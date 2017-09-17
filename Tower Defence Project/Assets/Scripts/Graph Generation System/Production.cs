using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnityEngine;

[Serializable]
public class Production : MonoBehaviour {
    Graph leftSide, rightSide;
    List<Graph> candidateGraphs;
    string label;

    //------------------------------------------------------------Constructor Methods------------------------------------------------------------//
    public Production(string label) {
        this.label = label;
        leftSide = new Graph();
        rightSide = new Graph();
        candidateGraphs = new List<Graph>();
    }

    public Production(string label, Graph leftSide, Graph rightSide) {
        this.leftSide = leftSide;
        this.rightSide = rightSide;
        this.label = label;
        candidateGraphs = new List<Graph>();
    }

    //------------------------------------------------------------Mutator Methods------------------------------------------------------------//
    public bool ApplyToRandom(Graph host) {
        System.Random random = new System.Random();
        int i = random.Next(candidateGraphs.Count);
        return Apply(host, candidateGraphs[i]);
    }

    private bool Apply(Graph host, Graph candidate) {
        List<Node> cNodes = candidate.Nodes;
        List<Edge> cEdges = candidate.Edges;
        List<Edge> cExternalEdges = candidate.GetExternalEdges(cNodes);
        
        //Remove host subgraph internal nodes
        host.RemoveEdges(cEdges);
        for (int i = 0; i < cNodes.Count; ++i) {
            Node lNode = leftSide.Nodes[i];
            Node cNode = cNodes[i];
            if (rightSide.DoesNodeExist(lNode)) {
                host.Replace(cNode, rightSide.GetNode(lNode));
            } else {
                host.RemoveNode(cNode);
                host.CleanEdges(cNode);
            }
        }

        foreach (Node rNode in rightSide.Nodes) {
            if (!leftSide.DoesNodeExist(rNode)) {
                host.AddNode(rNode);
            }
        }

        //Add new internal nodes from production
        host.AddEdges(rightSide.Edges);

        return true;
    }    

    //------------------------------------------------------------Accessors Methods------------------------------------------------------------//
    public Graph LeftSide {
        get {
            return leftSide;
        }
    }

    public Graph RightSide {
        get {
            return rightSide;
        }
    }

    public string Label {
        get {
            return label;
        }
    }

    public List<Graph> CandidateGraphs {
        get {
            return candidateGraphs;
        }

        set {
            candidateGraphs = value;
        }
    }

    //------------------------------------------------------------Serialization Methods------------------------------------------------------------//
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("label", label);
        info.AddValue("left side", leftSide);
        info.AddValue("right side", rightSide);
        info.AddValue("candidate graphs", candidateGraphs);
    }

    public Production(SerializationInfo info, StreamingContext context) {
        label = (string)info.GetValue("label", typeof(string));
        leftSide = (Graph)info.GetValue("left side", typeof(Graph));
        rightSide = (Graph)info.GetValue("right side", typeof(Graph));
        candidateGraphs = (List<Graph>)info.GetValue("candidate graphs", typeof(List<Graph>));
    }
}
    