using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    [SerializeField] Canvas ThisCanvas;

    private void LateUpdate()
    {
        if (ThisCanvas.worldCamera != null)
            transform.LookAt(transform.position + (-ThisCanvas.worldCamera.transform.forward));
    }
}
