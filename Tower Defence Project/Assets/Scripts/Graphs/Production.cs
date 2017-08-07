using System.Collections.Generic;

public class Production {

    Graph leftSide, rightSide;
    List<Graph> matchedSubgraphs;
    List<List<Node>> candidates;
    string label;

    public Production(Graph leftSide, Graph rightSide, string label) {
        this.leftSide = leftSide;
        this.rightSide = rightSide;
        this.label = label;
    }

    public List<Graph> MatchedSubgraphs {
        get {
            return matchedSubgraphs;
        }

        set {
            matchedSubgraphs = value;
        }
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

    public List<List<Node>> Candidates {
        get {
            return candidates;
        }

        set {
            candidates = value;
        }
    }
}
    