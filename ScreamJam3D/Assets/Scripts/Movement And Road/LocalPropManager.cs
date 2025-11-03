using UnityEngine;

public class LocalPropManager : MonoBehaviour
{
    [Header("Directions:")]
    [Header("Red line points towards the road.")]
    [Header("Blue line points parallel to it.")]
    [Header("Link to your list here:")]
    [SerializeField] private PropList propPrefabs;
    [SerializeField, Range(0f, 100f)] private float chanceFail;
    private GameObject _prop;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float roll = Random.Range(0f, 100f);

        if (roll <= chanceFail)
        {
            //Destroy(gameObject);
        }
        else
        {
            _prop = Instantiate(propPrefabs.GetRandom, transform.position, transform.rotation, transform);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.right * 2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }

    void OnDestroy()
    {
        if (_prop)
            Destroy(_prop);
    }
}
