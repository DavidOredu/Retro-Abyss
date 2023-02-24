using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupController : SerializedMonoBehaviour
{
    //[SerializeField]
    //private float spawnTime;

    //public GameObject spawnPoint;
    //public GameObject powerupPrefab;

    //public List<Powerup> powerups;

    public Dictionary<string, float> activePowerups = new Dictionary<string, float>();

    public List<Powerup> keys { get; private set; } = new List<Powerup>();

    public Dictionary<string, Powerup> powerupIdentifier = new Dictionary<string, Powerup>();

    private void Update()
    {
        HandleActivePowerups();
    }
    public void HandleActivePowerups()
    {
        bool changed = false;

        if(activePowerups.Count > 0)
        {
            foreach(Powerup powerup in powerupIdentifier.Values)
            {
                if(activePowerups[powerup.name] > 0)
                {
                    powerup.Active();
                    if(powerup.isTimeBased)
                        activePowerups[powerup.name] -= Time.deltaTime;
                    else
                        activePowerups[powerup.name] = powerup.value;
                }
                else
                {
                    changed = true;

                    powerup.End();
                   
                    activePowerups.Remove(powerup.name);
                }
            }
        }

        if (changed)
        {
            List<string> keys = new List<string>(powerupIdentifier.Keys);

            foreach (var key in keys)
            {
                if(!activePowerups.ContainsKey(key))
                    powerupIdentifier.Remove(key);
            }
        }
    }

    public void ActivatePowerup(Powerup powerup)
    {
        if (!activePowerups.ContainsKey(powerup.name))
        {
            powerup.Start();
            powerup.value = powerup.powerupData.level;

            if(powerup.isTimeBased)
                activePowerups.Add(powerup.name, powerup.duration);
            else
                activePowerups.Add(powerup.name, powerup.value);

            powerupIdentifier.Add(powerup.name, powerup);
        }
        else
        {
            if (powerup.isTimeBased)
                activePowerups[powerup.name] += powerup.powerupData.duration;
            else
                powerupIdentifier[powerup.name].value = powerup.powerupData.level;
        }

        //keys = new List<Powerup>(activePowerups.Keys);
    }

    //public GameObject spawnPowerup(Powerup powerup, Vector3 position)
    //{
    //    GameObject powerupGameObject = Instantiate(powerupPrefab);

    //    var powerupBehaviour = powerupGameObject.GetComponent<PowerupBehaviour>();

    //    powerupBehaviour.powerupController = this;

    //    powerupBehaviour.SetPowerup(powerup);

    //    powerupGameObject.transform.position = position;
    //    return powerupGameObject;
    //}

    //IEnumerator SpawnRandomPoint()
    //{
        

    //    spawnPowerup(powerups[Random.Range(0, powerups.Count - 1)], spawnPoint.transform.position);

    //    yield return new WaitForSeconds(spawnTime);
    //}
}
