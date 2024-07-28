using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_Canvas : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Canvas_inactivate;
    void Start()
    {
        Canvas_inactivate.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
