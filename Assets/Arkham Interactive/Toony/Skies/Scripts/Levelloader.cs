using UnityEngine;

public class Levelloader : MonoBehaviour
{
    public Camera _camera;

    public static Levelloader Inst;
    private void Awake()
    {
        Inst = this;
    }

    bool FirstInitialize = true;
    private void Update()
    {
        if (FirstInitialize && PlayerLobbyCanvas.Inst != null)
        {
            PlayerLobbyCanvas.Inst._canvas.worldCamera = _camera;
            FirstInitialize = false;
        }
    }
}
