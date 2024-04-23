using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lidar
{
    public class Scanned_Jumpscare : MonoBehaviour
    {
        [SerializeField] private string jumpscareSceneName = "JumpscareScene_(WindowError)";

        private bool hasBeenScanned = false;

        public void Scanned(RaycastHit _hit)
        {
            if (hasBeenScanned) return;

            Debug.Log("Is being scanned...");
            
            if (_hit.transform.tag == "Red")
            {
                ErrorSceneManager.TriggerJumpscare();
                Destroy(_hit.collider.gameObject, 8f);
                hasBeenScanned = true;
            }
        }

        IEnumerator TriggerJumpscare()
        {
            // Optionally pause the game
            Time.timeScale = 0;

            // Load the jumpscare or error scene
            SceneManager.LoadScene(jumpscareSceneName, LoadSceneMode.Additive);

            // Wait
            yield return new WaitForSecondsRealtime(3);

            // Unload the jumpscare scene
            SceneManager.UnloadSceneAsync(jumpscareSceneName);

            // Resume game
            Time.timeScale = 1;
        }
    }
}
