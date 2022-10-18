using UnityEngine;
using Adverty;
public class InitializeAdvertySDK
{
    private static bool initilized;
    public static void Initialize(Camera camera)
    {
        if(!initilized)
        {
            //Create UserData and initialize Adverty SDK
            UserData userData = new UserData(AgeSegment.Unknown, Gender.Unknown);
            AdvertySDK.Init(userData);
            initilized = true;
        }
        
        Adverty.AdvertySettings.SetMainCamera(camera);
        Debug.Log("Adverty Initialized");
    }
    
}