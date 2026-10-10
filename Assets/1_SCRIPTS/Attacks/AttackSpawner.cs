using UnityEngine;

public class AttackSpawner : MonoBehaviour
{
    public GameObject attackToSpawn;
    public GameObject projectileHolder;

    [Header("Config")]
    [SerializeField] private float spawnDistance = 1.5f;

    public GameEntity attackOriginator;
     
    public void SpawnAttack(Vector3 direction)
    {
        GameObject attack = Instantiate(attackToSpawn, this.transform);

        attack.gameObject.transform.position += direction * spawnDistance;
        if (projectileHolder != null) attack.gameObject.transform.SetParent(projectileHolder.transform, true);

        Attack atk = attack.GetComponentsInChildren<Attack>()[0];

        atk.thisAttacker = attackOriginator;

        if (atk.hasAdditionalAttack)
        {
            if (atk.staticAttack) atk.nextAttackDirection = direction;
            else atk.nextAttackDirection = new Vector3 (0, 0, 0);   

            atk.nextAttackSpawner.attackOriginator = attackOriginator;
        }

        atk.TriggerAttack(direction); 
    }
     
    public void SpawnAttack(Vector3 direction, int boost)
    {
        GameObject attack = Instantiate(attackToSpawn, this.transform);

        attack.gameObject.transform.position += direction * spawnDistance;
        if (projectileHolder != null) attack.gameObject.transform.SetParent(projectileHolder.transform, true);

        Attack atk = attack.GetComponentsInChildren<Attack>()[0];

        atk.thisAttacker = attackOriginator;

        if (atk.hasAdditionalAttack)
        {
            if (atk.staticAttack) atk.nextAttackDirection = direction;
            else atk.nextAttackDirection = new Vector3 (0, 0, 0);   

            atk.nextAttackSpawner.attackOriginator = attackOriginator;
        }

        atk.TriggerAttack(direction, boost); 
    }
}
