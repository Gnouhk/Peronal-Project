using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lidar
{
    public class Scanned_Jumpscare : MonoBehaviour
    {
        public void Scanned(RaycastHit _hit)
        {
            Debug.Log("Is being scanned...");
            
            if (_hit.transform.tag == "Red")
            {
                Debug.Log("Scanned");
            }
        }
    }
}
