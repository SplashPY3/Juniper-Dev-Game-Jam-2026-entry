using System;

[Serializable]
public class RelicInstance
{
    public Relic data;
    public int currentStacks;

    public bool CanStack => currentStacks < data.maxStacks;

    // Effect value scaled by the current number of stacks
    public int ScaledValue => data.effectValue * currentStacks;

    public RelicInstance(Relic relic)
    {
        data = relic;
        currentStacks = 1;
    }
}
