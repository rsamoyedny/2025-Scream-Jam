using UnityEngine;

[CreateAssetMenu(fileName = "PropList", menuName = "Scriptable Objects/PropList")]
public class PropList : ScriptableObject
{
    [SerializeField] private GameObject[] propPrefabs;

    public GameObject GetRandom => propPrefabs[Random.Range(0, propPrefabs.Length)];
}