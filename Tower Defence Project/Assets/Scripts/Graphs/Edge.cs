public enum EdgeType { UNI_DIRECTIONAL, BI_DIRECTIONAL };

public class Edge {
    /*
     * An edge contains a refernece to both the source and target nodes that it connects.
     * An edge it of type BI_DIRECTIONAL meaning it goes both ways or that there is no direction.
     * Edges are matched on type, source and target nodes.
     */

    Node source, target; //References to the source and target nodes
    EdgeType edgeType;

    public Edge(Node source, Node target, EdgeType edgeType) {
        this.source = source;
        this.target = target;
        this.edgeType = edgeType;
    }


    public Edge(Node source, Node target) {
        this.source = source;
        this.target = target;
        edgeType = EdgeType.BI_DIRECTIONAL;
    }

    //--------------------------Accessors Methods--------------------------//

    public Node Source {
        get {
            return source;
        }

        set {
            source = value;
        }
    }

    public Node Target {
        get {
            return target;
        }

        set {
            target = value;
        }
    }
}
