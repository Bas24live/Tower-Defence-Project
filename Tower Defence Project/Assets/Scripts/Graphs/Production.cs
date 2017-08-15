using System.Collections.Generic;

public class Production {

    Graph leftSide, rightSide;
    List<Graph> candidateGraphs;
    string label;

    public Production(Graph leftSide, Graph rightSide, string label) {
        this.leftSide = leftSide;
        this.rightSide = rightSide;
        this.label = label;
    }

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
    