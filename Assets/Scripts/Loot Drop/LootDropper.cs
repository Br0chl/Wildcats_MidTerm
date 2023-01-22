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

        float dropForce = 300f;
        Vector3 dropDirection = new Vector3(Random.Range(-3f, .3f), 0, Random.Range(-3f, .3f));
        drop.GetComponent<Rigidbody>().AddForce(dropDirection * dropForce, ForceMode.Impulse);
    }
}
