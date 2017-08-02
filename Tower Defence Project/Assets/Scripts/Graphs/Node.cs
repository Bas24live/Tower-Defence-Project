public class Node {

    private string ID, type;
    private Position position;
    private string label;

    public Node (string label, string type, Position position) {
        this.label = label;
        this.type = type;
        this.position = position;
    }

    public void Shift(Position amount) {
        position.X += amount.X;
        position.Y += amount.Y;
        position.Z += amount.Z;
    }

    //--------------------------Accessors Methods--------------------------//

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
}
