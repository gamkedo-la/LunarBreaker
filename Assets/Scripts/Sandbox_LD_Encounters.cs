using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_LDSandbox : MonoBehaviour
{
    private int nbZones;
    private List<GameObject> zoneTemplates = new();
    // the lists below share the same indexing as `zoneTemplates`
    private List<string> zoneNames = new();
    private List<Vector3> playerSpawnPositions = new();
    private List<Quaternion> playerSpawnRotations = new();

    private int curZoneIdx;
    private GameObject curZone;

    [SerializeField]
    private List<GameObject> enableTheseOnStart;
    [SerializeField]
    private List<GameObject> disableTheseOnStart;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GameObject startAtZone;

    void Start()
    {
        foreach (var gameObject in enableTheseOnStart)
        {
            gameObject.SetActive(true);
        }
        foreach (var gameObject in disableTheseOnStart)
        {
            gameObject.SetActive(false);
        }

        // Gather zone templates and their corresponding player spawn position+orientation.
        foreach (GameObject maybeZone in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (!maybeZone.name.StartsWith("Zone:")) { continue; }

            string zoneName = maybeZone.name.Substring(5).Trim();
            Transform playerSpawn = GetChildByName(maybeZone.transform, "PlayerSpawn");
            if (!playerSpawn)
            {
                Debug.LogWarningFormat("Sandbox: could not find player spawn for zone '{0}'. Will ignore this zone.", zoneName);
                continue;
            }
            Vector3 psPosition = playerSpawn.transform.position;
            Quaternion psOrientation = playerSpawn.transform.rotation;
            playerSpawn.gameObject.SetActive(false); // hide the player spawn

            nbZones += 1;
            zoneNames.Add(zoneName);
            zoneTemplates.Add(maybeZone);
            playerSpawnPositions.Add(psPosition);
            playerSpawnRotations.Add(psOrientation);
        }
        Debug.LogFormat("Sandbox: gathered {0} zones", nbZones);

        // We'll use the designer-defined zones as templates.
        // Disable them now, and we'll clone the relevant template when switching to a given zone.
        foreach (GameObject zone in zoneTemplates)
        {
            zone.SetActive(false);
        }

        // Start on the specified zone, or the first one if none is specified.
        curZoneIdx = startAtZone != null ? zoneTemplates.IndexOf(startAtZone) : 0;
        SwitchToZone(curZoneIdx);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SwitchToZone((curZoneIdx + 1) % nbZones);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            SwitchToZone((curZoneIdx + nbZones - 1) % nbZones);
        }
    }

    private void SwitchToZone(int zoneIdx)
    {
        if (curZone)
        {
            Destroy(curZone);
        }

        // Create a new zone instance using the relevant template.
        GameObject template = zoneTemplates[zoneIdx];
        curZone = Instantiate(template);
        curZone.SetActive(true);
        curZoneIdx = zoneIdx;

        // Move the player at the spawn point.
        playerController.Teleport(playerSpawnPositions[zoneIdx], playerSpawnRotations[zoneIdx]);

        // Reset the player controller (HP etc).
        // TODO

        Debug.LogFormat("Sandbox: switched to zone '{0}'", zoneNames[zoneIdx]);
    }

    private Transform GetChildByName(Transform reference, string childName)
    {
        return reference.GetComponentsInChildren<Transform>().FirstOrDefault(child => child.name == childName);
    }
}
