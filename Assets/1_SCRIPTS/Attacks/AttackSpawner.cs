using UnityEngine;

public class AttackSpawner : MonoBehaviour
{
    public GameObject attackToSpawn;
    public GameObject projectileHolder;
    
    public void SpawnAttack(Vector3 direction)
    {
        GameObject attack = Instantiate(attackToSpawn, this.transform);

        attack.gameObject.transform.position += direction * 1.5f;
        if (projectileHolder != null) attack.gameObject.transform.SetParent(projectileHolder.transform, true);
        
        attack.GetComponent<Attack>().TriggerAttack(direction); 
    }
}
