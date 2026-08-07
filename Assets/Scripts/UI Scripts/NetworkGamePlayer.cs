using Mirror;

public class NetworkGamePlayer : NetworkBehaviour
{
    [SyncVar]
    private string displayName = "Loading...";

    private NM_GC room;
    private NM_GC Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NM_GC;
        }
    }

    public override void OnStartClient()
    {
        //DontDestroyOnLoad(gameObject);

        //Room.NetworkGamePlayers.Add(this);
    }

    public override void OnStopClient()
    {
        //Room.NetworkGamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
}