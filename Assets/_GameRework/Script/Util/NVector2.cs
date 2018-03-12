using UnityEngine;

[System.Serializable]
public struct NVector2
{
    public int x, y;

    public NVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public NVector2(Vector2 v){
        this.x = (int) v.x;
        this.y = (int) v.y;
    }


    public static NVector2 operator -(NVector2 a, NVector2 b) { return new NVector2(a.x - b.x, a.y - b.y); }
    public static NVector2 operator +(NVector2 a, NVector2 b) { return new NVector2(a.x + b.x, a.y + b.y); }

    public static NVector2 operator *(NVector2 a, int b) { return new NVector2(a.x * b, a.y * b); }

    
    public static bool operator ==(NVector2 a, NVector2 b) { return a.Equals(b); }
    public static bool operator !=(NVector2 a, NVector2 b) { return !(a == b); }

    public bool Equals(NVector2 other) { return x == other.x && y == other.y; }
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is NVector2 && Equals((NVector2)obj);
    }

    public override int GetHashCode() { unchecked { return (x * 397) ^ y; } }

    public static implicit operator Vector2(NVector2 v) { return new Vector2(v.x, v.y); }
    public override string ToString() { return $" [{x}, {y}]"; }
}
