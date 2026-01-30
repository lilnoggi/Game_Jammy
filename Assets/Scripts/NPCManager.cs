using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public GameObject refugeePrefab;

    public Transform spawnRight;
    public Transform standPoint;
    public Transform exitLeft;

    private Refugee currentRefugee;

    void Start()
    {
        SpawnRefugee();
    }

    void SpawnRefugee()
    {
        GameObject obj = Instantiate(
            refugeePrefab,
            spawnRight.position,
            Quaternion.identity
        );

        currentRefugee = obj.GetComponent<Refugee>();
        currentRefugee.MoveTo(standPoint.position);
    }

    public void ApproveRefugee()
    {
        if (currentRefugee == null) return;

        currentRefugee.MoveTo(exitLeft.position);
        Destroy(currentRefugee.gameObject, 2f);
        currentRefugee = null;

        Invoke(nameof(SpawnRefugee), 1.5f);
    }

    public void RejectRefugee()
    {
        if (currentRefugee == null) return;

        currentRefugee.MoveTo(exitLeft.position);
        Destroy(currentRefugee.gameObject, 2f);
        currentRefugee = null;

        Invoke(nameof(SpawnRefugee), 1.5f);
    }
}
