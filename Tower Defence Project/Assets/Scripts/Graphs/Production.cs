using System.Collections.Generic;

public class Production {

    Graph lhSide, rhSide, matchedSubGraph;
    string label;

    public Production(Graph lhSide, Graph rhSide, string label) {
        this.lhSide = lhSide;
        this.rhSide = rhSide;
        this.label = label;
    }

    public Graph MatchedSubGraph {
        get {
            return matchedSubGraph;
        }

        set {
            matchedSubGraph = value;
        }
    }

    public Graph LhSide{
        get {
            return lhSide;
        }
    }

    public Graph RhSide {
        get {
            return rhSide;
        }
    }

    public string Label {
        get {
            return label;
        }
    }


}
    