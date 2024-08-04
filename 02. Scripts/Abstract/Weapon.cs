using UnityEngine;

public abstract class Weapon
{
    public int Damage { get; set; }

    public virtual void Use(Character target)
    {
        Debug.Log("Weapon used Damage : " + Damage);
    }
}
