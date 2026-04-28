using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Updated Network Runner Handler with input handling for the ported prototype.
/// Integrates multiplayer support for both props and hunters.
/// </summary>
public class UpdatedNetworkRunnerHandler : MonoBehaviour, INetworkInput
{
    private NetworkRunner runner;

    async void Start()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "NairobiPropHunt",
            Scene = SceneManager.GetActiveScene().buildIndex,
            PlayerCount = 10
        });
    }

    /// <summary>
    /// Provides input data for networked players each frame.
    /// </summary>
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        // Collect input
        data.movement = GetMovementInput();
        data.changeDisguise = Input.GetKeyDown(KeyCode.C);
        data.toggleHide = Input.GetKeyDown(KeyCode.H);

        input.Set(data);
    }

    /// <summary>
    /// Gets the current movement input from WASD keys.
    /// </summary>
    private Vector2 GetMovementInput()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            movement.y += 1;
        if (Input.GetKey(KeyCode.S))
            movement.y -= 1;
        if (Input.GetKey(KeyCode.A))
            movement.x -= 1;
        if (Input.GetKey(KeyCode.D))
            movement.x += 1;

        return movement.normalized;
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server!");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from server!");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress address, NetConnectFailedReason reason)
    {
        Debug.LogError($"Connection failed: {reason}");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"Network shutdown: {shutdownReason}");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene load complete!");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Starting scene load...");
    }
}

/// <summary>
/// Network input data structure for Photon Fusion.
/// Encapsulates player input for networked gameplay.
/// </summary>
public struct NetworkInputData : INetworkInput
{
    public Vector2 movement;
    public bool changeDisguise;
    public bool toggleHide;
}
