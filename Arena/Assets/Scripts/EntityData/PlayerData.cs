using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{

    public bool isAlive;
    public int health = 1;
    public float attackDamage;
    public int currentGold;
    public PlayerRoles currentRole;
    public int _entityID;
    

    public void OnChangeRole(PlayerRoles role, int entityID)
    {
        if(_entityID != entityID)
        {
            return;
        }
        
        //do a check to make sure you arent already that role

        currentRole = role;

    }




}
