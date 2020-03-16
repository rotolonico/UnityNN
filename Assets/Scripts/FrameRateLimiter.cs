using UnityEngine;

public class FrameRateLimiter : MonoBehaviour 
{
    public int targetFrameRate = 60;
 
    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
}