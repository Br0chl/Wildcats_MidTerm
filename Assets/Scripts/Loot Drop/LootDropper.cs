using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LootDropper : MonoBehaviour
{
    [SerializeField] List<Loot> lootList = new List<Loot>();

    Loot GetDrop()
    {
        int randomNum = Random.Range(1, 101);
        List<Loot> possibleDrops = new List<Loot>();
        foreach (Loot item in lootList)
        {
            if (randomNum <= item.GetDropChance())
            {
                possibleDrops.Add(item);
            }
        }
        if (possibleDrops.Count > 0)
        {
            Loot drop = possibleDrops[Random.Range(0, possibleDrops.Count)];
            return drop;
        }
        // No loot
        return null;
    }

    List<Loot> GetDrops()
    {
        int randomNum = Random.Range(1, 101);
        List<Loot> possibleDrops = new List<Loot>();
        foreach (Loot item in lootList)
        {
            if (randomNum <= item.GetDropChance())
            {
                possibleDrops.Add(item);
            }
        }
        if (possibleDrops.Count > 0)
        {
            return possibleDrops;
        }
        // No loot
        return null;
    }

    public void DropLoot(Vector3 lootSpawn)
    {
        Loot itemToDrop = GetDrop();
        if (itemToDrop == null)
            return;

        GameObject drop = Instantiate(itemToDrop.gameObject, lootSpawn, Quaternion.identity);
    }

    public void GetMultipleDrops(Vector3 lootSpawn)
    {
        List<Loot> itemsToDrop = GetDrops();
        if(itemsToDrop == null) return;
        if (itemsToDrop.Count == 0)
            return;

        foreach (Loot lootDrop in itemsToDrop)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir += new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

            // Check that destination is on the navMesh
            NavMeshHit hit;
            NavMesh.SamplePosition(new Vector3(randomDir.x, 0, randomDir.z), out hit, 1, 1);

            if (hit.position != null)
            {
                GameObject drop = Instantiate(lootDrop.gameObject, lootSpawn + hit.position, Quaternion.identity);
            }
        }
    }
}
