using UnityEngine;

public abstract class Sensor : MonoBehaviour
{
    public abstract RaycastHit2D Hit { get; }
}
