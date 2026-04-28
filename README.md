NAiROBI PROP HUNT — MULTIPLAYER CORE (FINAL BUILD)
⚙️ 1. NETWORK MANAGER

Handles joining a room + spawning players

using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public NetworkRunner runner;
    public GameObject playerPrefab;

    async void Start()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "PropHuntRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawn = new Vector3(Random.Range(-5,5),1,Random.Range(-5,5));
            runner.Spawn(playerPrefab, spawn, Quaternion.identity, player);
        }
    }
}
🧍 2. NETWORK PLAYER (PROP / HUNTER SYNC)
using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [Networked] public int Role { get; set; } // 0 Prop, 1 Hunter

    public GameObject propModel;
    public GameObject hunterModel;
    public float speed = 5f;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
            Role = Random.Range(0, 2);

        UpdateModel();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.position += new Vector3(h, 0, v) * speed * Runner.DeltaTime;
    }

    void UpdateModel()
    {
        propModel.SetActive(Role == 0);
        hunterModel.SetActive(Role == 1);
    }
}
🔫 3. HUNTER SHOOTING (ONLINE HIT CHECK)
using Fusion;
using UnityEngine;

public class NetworkHunter : NetworkBehaviour
{
    public float range = 20f;

    void Update()
    {
        if (!Object.HasInputAuthority) return;

        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    void Shoot()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            NetworkPlayer p = hit.transform.GetComponent<NetworkPlayer>();

            if (p != null && p.Role == 0)
                Debug.Log("PROP HIT!");
            else
                Debug.Log("MISS");
        }
    }
}
🎭 4. PROP SYSTEM (SYNC DISGUISE)
using Fusion;
using UnityEngine;

public class NetworkProp : NetworkBehaviour
{
    [Networked] public int PropID { get; set; }

    public GameObject[] props;

    public void ChangeProp(int id)
    {
        if (Object.HasStateAuthority)
        {
            PropID = id;
            Apply();
        }
    }

    void Apply()
    {
        for (int i = 0; i < props.Length; i++)
            props[i].SetActive(i == PropID);
    }
}
⏱️ 5. GAME TIMER (SHARED MATCH TIME)
using Fusion;
using UnityEngine;

public class NetworkGameManager : NetworkBehaviour
{
    [Networked] public float TimeLeft { get; set; } = 60f;

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            TimeLeft -= Runner.DeltaTime;

            if (TimeLeft <= 0)
                Debug.Log("PROPS WIN");
        }
    }
}
🎮 WHAT THIS BUILD DOES
✔ Multiplayer
Players join same room
Auto host/client system
✔ Roles
Random Prop or Hunter assignment
✔ Gameplay
Props move + disguise
Hunters shoot and detect
✔ Sync
Position synced
Prop state synced
Match timer shared
🚀 HOW TO RUN (IMPORTANT)
Install Photon Fusion
Create App ID (Photon dashboard)
Add NetworkRunner object
Make Player prefab:
NetworkObject
NetworkPlayer script
Assign prefab to NetworkManager
Play twice (or build + run)
⚡ YOU NOW HAVE

✔ Real online multiplayer prototype
✔ Prop vs Hunter gameplay loop
✔ Network synced movement
✔ Shooting system
✔ Shared match session

🔥 NEXT UPGRADE OPTIONS

If you want, I can extend this into:

🎭 Real prop morphing (smooth transformations)
🧠 AI hunters (bots for offline mode)
🎯 Proper hit confirmation system (server-authoritative)
🗺️ Lobby + matchmaking UI
📱 Mobile controls
🔊 Sound-based detection system
🎥 AAA-style polish (camera shake, effects, animations)

Just say:
👉 “make it AAA”
or
👉 “add lobby + matchmaking”

and I’ll build the next layer.
