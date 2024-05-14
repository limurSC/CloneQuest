using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Plate : MonoBehaviour
{
    [SerializeField] private UnityEvent OnActivate;
    [SerializeField] private UnityEvent OnDeactivateSet;
    private int _triggers = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggers == 0) { OnActivate.Invoke(); }
        _triggers += 1;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _triggers -= 1;
        if (_triggers == 0) { OnDeactivateSet.Invoke(); }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        var colliders = GetComponents<Collider2D>();
        if (colliders.Length > 0 && colliders.Any((collider) => collider.isTrigger)) { return; }
        Debug.LogWarning("At least one collider trigger is required: ", this);
    }
#endif
}
