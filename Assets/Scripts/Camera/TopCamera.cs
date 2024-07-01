using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopCamera : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHeight(float height)
    {   
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}
