
using UnityEngine;
/// <summary>
/// Information for the placement of held items;
/// </summary>
public class PlacementInfo
{
    public bool WorldRaycastValid;
    public RaycastHit WorldRaycastHit;
    public float WorldPlacementYaw; // the current yaw of the holdable, used for rotation while placing.


    public bool SnapRaycastValid;
    public RaycastHit SnapRaycastHit;




    public bool TryGetSnapArea(out SnapArea snapArea)
    {
        if (!SnapRaycastValid)
        {
            snapArea = null;
            return false;
        }

        if (SnapRaycastHit.collider.TryGetComponent<SnapArea>(out var point))
        {
            snapArea = point;
            return true;
        }
        else
        {
            snapArea = null;
            return false;
        }
    }

    /// <summary>
    /// tries to get the position and rotation of a world placement if it is valid
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <returns></returns>
    public bool TryGetWorldPlacementInfo(out Transform trans, out Vector3 pos, out Quaternion rot)
    {
        if (!WorldRaycastValid)
        {
            trans = null;
            pos = Vector3.zero;
            rot = Quaternion.identity;
            return false;
        }
        else
        {
            trans = WorldRaycastHit.transform;
            pos = WorldRaycastHit.point;
            rot = Quaternion.Euler(0, WorldPlacementYaw, 0);
            return true;
        }

    }



}