using System.Runtime.Serialization;
using System;

[Serializable]
public class Vector3 : ISerializable{

    private float x, y, z;

    public Vector3 (float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    //----------------------------Accessor Methods----------------------------//

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

    //----------------------------Serialization Methods----------------------------//

    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("x", x);
        info.AddValue("y", y);
        info.AddValue("z", z);
    }

    public Vector3(SerializationInfo info, StreamingContext context) {
        x = (float)info.GetValue("x", typeof(float));
        y = (float)info.GetValue("y", typeof(float));
        z = (float)info.GetValue("z", typeof(float));
    }
}
