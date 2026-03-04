
using UnityEngine;
public class PlacementRaycastInfo
{
    public bool WorldRaycastValid;
    public RaycastHit WorldRaycastHit;

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

    public bool TryGetWorldPlacementPos(out Vector3 pos)
    {
        if (!WorldRaycastValid)
        {
            pos = Vector3.zero;
            return false;
        }
        else
        {
            pos = WorldRaycastHit.point;
            return true;
        }

    }

}