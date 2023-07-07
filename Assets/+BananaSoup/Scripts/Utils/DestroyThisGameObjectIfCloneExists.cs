using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BananaSoup
{
    public class DestroyThisGameObjectIfCloneExists : MonoBehaviour
    {
        private string thisGameObjectsName;

        private void Awake()
        {
            thisGameObjectsName = name;

            string clone = GameObject.Find(thisGameObjectsName).name;
            if ( /* GameObject.Find(thisGameObjectsName).name == thisGameObjectsName && */ GameObject.Find(thisGameObjectsName) != gameObject )
            {
                Debug.Log($"{name} already exists. Destroying this one.");
                Destroy(gameObject);
            }
        }
    }
}
