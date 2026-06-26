using System;

[Serializable]
public class WheelWedge
{
    public WedgeType type;
    public float weight; // relative probability weight (default 1.0 per wedge)

    public WheelWedge(WedgeType type, float weight = 1f)
    {
        this.type   = type;
        this.weight = weight;
    }
}
