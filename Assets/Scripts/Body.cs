using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{

    public void OnExit()
    {
        transform.GetComponentInParent<Player>().ChangePosition();
    }
}
