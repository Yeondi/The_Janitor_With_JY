using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayEffect(string address, Vector3 position)
    {
        Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject effectPrefab = handle.Result;
                Instantiate(effectPrefab, position, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Failed to load effect: " + address);
            }
        };
    }
}
