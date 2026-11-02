using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponScript))]
public class PlayerMeleeRange : Editor
{
    private void OnSceneGUI()
    {
        WeaponScript melee = (WeaponScript)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(melee.transform.position, melee.transform.forward, Vector2.up, 360, melee.meleeRange);

        Vector3 viewAngle01 = DirectionFromAngle(melee.transform.eulerAngles.y, -melee.meleeAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(melee.transform.eulerAngles.y ,melee.meleeAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(melee.transform.position, melee.transform.position + viewAngle01 * melee.meleeRange);
        Handles.DrawLine(melee.transform.position, melee.transform.position + viewAngle02 * melee.meleeRange);
    }

    private Vector2 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
