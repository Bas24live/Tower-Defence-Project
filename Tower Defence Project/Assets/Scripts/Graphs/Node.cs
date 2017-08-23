using System;
using System.Runtime.Serialization;

[Serializable]
public class Node : ISerializable {
    private string id, label, type;
    private Position position;

    public Node (string id, string label, string type, Position position) {
        this.id = id;
        this.label = label;
        this.type = type;
        this.position = position;
    }

    public Node (string label, string type, Position position) {
        this.label = label;
        this.type = type;
        this.position = position;
    }

    public void Shift (Position amount) {
        position.X += amount.X;
        position.Y += amount.Y;
        position.Z += amount.Z;
    }

    public bool Compare (Node node) {
        return node.id == id && node.Type == type;
    }

    //------------------------------Accessors Methods------------------------------//
    public Position Position {
        get {
            return position;
        }

        set {
            position = value;
        }
    }

    public string Label {
        get {
            return label;
        }

        set {
            label = value;
        }
    }

    public string Type {
        get {
            return type;
        }

        set {
            type = value;
        }
    }
    

    //-----------------------------Serialization Methods-----------------------------//
    public void GetObjectData (SerializationInfo info, StreamingContext context) {
        info.AddValue("label", label);
        info.AddValue("id", id);
        info.AddValue("type", type);
        info.AddValue("position", position);
    }

    public Node (SerializationInfo info, StreamingContext context) {
        label = (string)info.GetValue("label", typeof(string));
        id = (string)info.GetValue("id", typeof(string));
        type = (string)info.GetValue("type", typeof(string));
        position = (Position)info.GetValue("position", typeof(Position));

    }
}
