using System;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    private GameObject currentRoomGameObject;
    GameObject GetLevelGameObject() => LevelManager.Instance.GetCurrentRoomConfig().levelGameObject;

    public void InstantiateRoomGameObject()
    {
        if (currentRoomGameObject != null)
        {
           DestroyRoomGameObject(); 
        }
        currentRoomGameObject = GetLevelGameObject();
        currentRoomGameObject = Instantiate(currentRoomGameObject, transform.position, Quaternion.identity); 
    }

    public void DestroyRoomGameObject()
    {
        Destroy(currentRoomGameObject);

    }
}
