using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
    public void DoRestart()
    {
        EventBus.Invoke<IRestart>(act => act.OnRestarted());
    }
}
