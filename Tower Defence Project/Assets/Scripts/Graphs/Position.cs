public class Position {

    private float x, y, z;

    public Position (float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public float X{

        get {
            return x;
        }

        set {
            x = value;
        }
    }

    public float Y {

        get {
            return y;
        }

        set {
            y = value;
        }
    }

    public float Z {

        get {
            return z;
        }

        set {
            z = value;
        }
    }

}
