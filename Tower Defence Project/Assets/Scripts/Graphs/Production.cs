using System.Collections.Generic;
using System;

public class Production {

    Graph leftSide, rightSide;
    List<Graph> candidateGraphs;
    string label;

    public Production (Graph leftSide, Graph rightSide, string label) {
        this.leftSide = leftSide;
        this.rightSide = rightSide;
        this.label = label;
    }

    public bool ApplyToRandom () {
        Random random = new Random();
        int i = random.Next(candidateGraphs.Count);
        return Apply(candidateGraphs[i]);

    }

    private bool Apply (Graph candidate) {
        for (int i = 0; i < leftSide.Nodes.Count; ++i) {
            Node lNode = leftSide.Nodes[i];
            Node cNode = candidate.Nodes[i];

            foreach (Edge edge in candidate.Edges) {
                if (edge.Source == cNode)
                    edge.Source = lNode;
                else if (edge.Target == cNode)
                    edge.Target = lNode;
            }
        }

        return true;
    }

    //--------------------------Accessors Methods--------------------------//

    public Graph LeftSide{
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
}
    