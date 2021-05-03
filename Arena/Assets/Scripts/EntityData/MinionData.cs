using Frictionless;
using System.Collections.Generic;
public class MinionData : EntityData
{
    public int maxHP;
    public int currentHP;
    public MinionType minionType;

    public void SetInitialValues()
    {
        currentHP = maxHP;
    }

}

