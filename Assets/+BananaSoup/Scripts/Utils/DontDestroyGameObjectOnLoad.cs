using UnityEngine;

namespace BananaSoup.Utils
{
    public class DontDestroyGameObjectOnLoad : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
