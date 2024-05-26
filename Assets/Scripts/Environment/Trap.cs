using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag.Equals(Tags.GetTag(TagName.Player)))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage();
        } 
    }
}
