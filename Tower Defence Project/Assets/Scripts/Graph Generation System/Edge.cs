using System;
using System.Runtime.Serialization;

public enum EdgeType { UNI_DIRECTIONAL, BI_DIRECTIONAL };

[Serializable]
public class Edge : ISerializable {
    /*
     * An edge contains a refernece to both the source and target nodes that it connects.
     * An edge it of type BI_DIRECTIONAL meaning it goes both ways or that there is no direction.
     * Edges are matched on type, source and target nodes.
     */

    Node source, target;    //References to the source and target nodes
    EdgeType edgeType;      //A desrciption for the type of edge
    string id;              //Unique identifier

    public Edge(Node source, Node target, EdgeType edgeType, string id) {
        this.source = source;
        this.target = target;
        this.edgeType = edgeType;
        this.id = id;
    }


    public Edge(Node source, Node target, string id) {
        this.source = source;
        this.target = target;
        this.id = id;
        edgeType = EdgeType.BI_DIRECTIONAL;
    }

    public bool CompareType(Edge edge) {
        return edge.edgeType == edgeType;
    }

    public bool CompareExact(Edge edge) {
        return edge.edgeType == edgeType && edge.id == id;
    }

    //------------------------------------------------------------Accessors Methods------------------------------------------------------------//
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

    public string Id {
        get {
            return id;
        }
    }

    //------------------------------------------------------------Serialization Methods------------------------------------------------------------//
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("id", id);
        info.AddValue("source", source);
        info.AddValue("source", source);
        info.AddValue("type", edgeType);
    }

    public Edge(SerializationInfo info, StreamingContext context) {
        id = (string)info.GetValue("id", typeof(string));
        source = (Node)info.GetValue("source", typeof(Node));
        target = (Node)info.GetValue("terget", typeof(Node));
        edgeType = (EdgeType)info.GetValue("type", typeof(EdgeType));
    }
}
