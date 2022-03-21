using TTP.Toys;
using UnityEngine;

public class ObjectsDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Toy toy))
        {
            if (!toy.IsInBusket)
            {
                Debug.Log($"Destroy {toy.name}");
                toy.IsDestroying = true;
                Destroy(toy.gameObject);
            }
        }
    }
}
