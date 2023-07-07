using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaSoup
{
    public class GameManager : MonoBehaviour
    {
        private PlayerInputManager inputManager;

        void Start()
        {
            GetReferences();
            SetupInputManager();
        }

        private void GetReferences()
        {
            inputManager = GetComponent<PlayerInputManager>();
            if ( inputManager == null )
            {
                Debug.LogError($"{name} is missing a reference to PlayerInputManager!");
            }
        }

        private void SetupInputManager()
        {
            inputManager.DisableJoining();
        }
    }
}
