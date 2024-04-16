using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScanning : MonoBehaviour
{
    private bool hasBeenScanned = false;

    public GameObject enemyObject;

    public void ScanEnemy()
    {
        if (hasBeenScanned) return;

        ErrorSceneManager.TriggerJumpscare();

        hasBeenScanned = true;

        enemyObject.SetActive(false);

        Destroy(gameObject, 2f);
    }
}
