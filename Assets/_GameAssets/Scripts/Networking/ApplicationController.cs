using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientSingletonPrefab;
    [SerializeField] private HostSingleton _hostSingletonPrefab;
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }
    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            // dedicated server
        }
        else
        {
            // host client

            HostSingleton hostSingletonInstance = Instantiate(_hostSingletonPrefab);
            hostSingletonInstance.CreateHost();
            
            ClientSingleton clientSingletonInstance = Instantiate(_clientSingletonPrefab);
            await clientSingletonInstance.CreateClient();
        }
    }
}
