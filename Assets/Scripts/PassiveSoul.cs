using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSoul : Interactable
{
    public override void Interact(Player player)
    {
        player.iPassiveSoulsCollected++;
        Destroy(gameObject);
    }
}
