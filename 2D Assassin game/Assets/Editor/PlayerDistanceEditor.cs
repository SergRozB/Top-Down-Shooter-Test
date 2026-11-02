using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CentralDataScript))]
public class PlayerDistanceEditor : Editor
{
    private void OnSceneGUI()
    {
        Player_Movement player = (Player_Movement)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(player.transform.position, Vector3.forward, Vector2.up, 360, CentralDataScript.exactPlayerPosLimit);
    }
}
