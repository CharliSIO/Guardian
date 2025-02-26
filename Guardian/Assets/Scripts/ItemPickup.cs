using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private Collider2D PickupCollider;
    private bool bPlayerInPickupRadius = false;

    // Start is called before the first frame update
    void Start()
    {
        PickupCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            bPlayerInPickupRadius = true;
            Debug.Log("Player in pickup radius");
            collision.gameObject.GetComponentInParent<PlayerMovement>().InteractableObject = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            bPlayerInPickupRadius = false;
            Debug.Log("Player LEFT pickup radius");
        }
    }

    public virtual void Interact(PlayerMovement _InteractingCharacter)
    {

    }
}
