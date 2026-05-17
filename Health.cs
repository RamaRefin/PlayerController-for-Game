using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int health = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   public void Damage(int amount)
   {

    if(amount <0)
    {
        throw new System.ArgumentOutOfRangeException("Cannot have negative damage");
    }

    this.health -= amount;
   }

    public void Heal(int amount)
   {

    if(amount <0)
    {
        throw new System.ArgumentOutOfRangeException("Cannot have negative heal");
    }

    this.health += amount;
   }
}
