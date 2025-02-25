using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_LDSandbox : MonoBehaviour
{
    private int nbZones;
    private List<GameObject> zoneTemplates = new();
    // the lists below share the same indexing as `zoneTemplates`
    private List<string> zoneNames = new();
    private List<Transform> playerSpawnPositions = new();
    private List<Transform> playerSpawnOrientations = new();

    void Start()
    {
        // We have disabled the gameplay objects in the Editor
        // so that they don't clutter the view when blocking out.
        // Now, enable them.
        EnableChildByName("Gameplay");

        // Gather zone templates and their corresponding player spawn position+orientation.
        foreach (GameObject maybeZone in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (!maybeZone.name.StartsWith("Zone:")) { continue; }

            string zoneName = maybeZone.name.Substring(5).Trim();
            Transform playerSpawn = GetChildByName(maybeZone.transform, "PlayerSpawn");
            if(!playerSpawn)
            {
                Debug.LogWarningFormat("Sandbox: could not find player spawn for zone '{0}'. Will ignore this zone.", zoneName);
                continue;
            }
            Transform psPosition = playerSpawn.GetChild(0);
            Transform psOrientation = playerSpawn.GetChild(1);

            nbZones += 1;
            zoneNames.Add(zoneName);
            zoneTemplates.Add(maybeZone);
            playerSpawnPositions.Add(psPosition);
            playerSpawnOrientations.Add(psOrientation);
        }
        Debug.LogFormat("Sandbox: gathered {0} zones", nbZones);

        // We'll use the designer-defined zones as templates.
        // Disable them now, and we'll clone the relevant template at zone start.
        // TODO
    }

    private void EnableChildByName(string childName)
    {
        Transform[] trs = GetComponentsInChildren<Transform>(true); // true instructs to also include disabled objects
        foreach (Transform t in trs)
        {
            if (t.name == childName)
            {
                t.gameObject.SetActive(true);
                break;
            }
        }
    }

    private Transform GetChildByName(Transform reference, string childName)
    {
        Transform[] trs = reference.GetComponentsInChildren<Transform>();
        foreach (Transform child in trs)
        {
            if (child.name == childName)
            {
                return child;
            }
        }
        return null;
    }
}
