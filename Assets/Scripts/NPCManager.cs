using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public GameObject refugeePrefab;
    public GameObject refugeeMaskPrefab;

    public Transform spawnRight;
    public Transform standPoint;
    public Transform exitLeft;

    private Refugee currentRefugee;
    public MaskData[] possibleMasks;


    void Start()
    {
        SpawnRefugee();
    }

    void SpawnRefugee()
    {
        GameObject ref_obj = Instantiate(refugeePrefab, spawnRight.position, Quaternion.identity);
        GameObject ref_mask = GetComponent<SpriteRandomizerLibrary>().InstantiateRandomMask(refugeeMaskPrefab, ref_obj, (spawnRight.position + new Vector3(0,.50f,0)));
        currentRefugee = ref_obj.GetComponent<Refugee>();

        currentRefugee.MoveTo(standPoint.position);


        /* DEPRECATED / OLD
        GameObject obj = Instantiate(refugeePrefab,spawnRight.position,Quaternion.identity);
        currentRefugee = obj.GetComponent<Refugee>();
        GetComponent<SpriteRandomizerLibrary>().InstantiateRandomMask(refugeeMaskPrefab, obj,new Vector3(0,0,0));
        refugeeMaskPrefab.gameObject.transform.position = new Vector3(0,0,0);
        currentRefugee.MoveTo(standPoint.position);
        */
        /* DEPRECATED / OLD
        // Assign random mask
        MaskData randomMask = possibleMasks[Random.Range(0, possibleMasks.Length)];
        currentRefugee.SetMask(randomMask);

        currentRefugee.MoveTo(standPoint.position);
        */
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
