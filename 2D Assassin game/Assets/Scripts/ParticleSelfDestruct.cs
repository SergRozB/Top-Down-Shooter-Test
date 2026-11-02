using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestruct : MonoBehaviour
{ 
    void Awake()
    {  
        if (gameObject.tag != "Bullet")
            StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
