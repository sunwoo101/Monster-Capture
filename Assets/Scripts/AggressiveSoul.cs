using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveSoul : Interactable
{
    public override void Interact(Player player)
    {
        player.iAggressiveSoulsCollected++;
        Destroy(gameObject);
    }
}
