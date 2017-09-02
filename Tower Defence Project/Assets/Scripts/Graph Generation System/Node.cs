using System;
using System.Runtime.Serialization;

[Serializable]
public class Node : ISerializable {
    public string id, label, type;
    public Vector3 position;

    public Node(string id, string label, string type, Vector3 position) {
        this.id = id;
        this.label = label;
        this.type = type;
        this.position = position;
    }

    public void Shift(Vector3 amount) { 
        position.X += amount.X;
        position.Y += amount.Y;
        position.Z += amount.Z;
    }

    public bool Compare (Node node) {
        return node.id == id && node.Type == type;
    }

    //------------------------------------------------------------Accessors Methods------------------------------------------------------------//
    public Vector3 Position {
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


    //------------------------------------------------------------Serialization Methods------------------------------------------------------------//
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("id", id);
        info.AddValue("label", label);
        info.AddValue("type", type);
        info.AddValue("position", position);
    }

    public Node(SerializationInfo info, StreamingContext context) {
        id = (string)info.GetValue("id", typeof(string));
        label = (string)info.GetValue("label", typeof(string));
        type = (string)info.GetValue("type", typeof(string));
        position = (Vector3)info.GetValue("position", typeof(Vector3));

    }
}
