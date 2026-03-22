
using System.Linq;
using UnityEngine;
/// <summary>
/// Information for the placement of held items
/// </summary>
public class PlacementInfo
{
    public bool WorldRaycastValid;
    public RaycastHit WorldRaycastHit;
    public float WorldPlacementYaw; // the current yaw of the holdable, used for rotation while placing.


    public bool SnapRaycastValid;
    public RaycastHit[] SnapRaycastHits;

    // the raycast that is used to place for a snapper
    public RaycastHit ValidSnapRaycastHit;

    /// <summary>
    /// Tries to get the first valid snap area that the snapper can snap to, and interally sets the valid raycast hit.
    /// </summary>
    /// <param name="currentSnapper"></param>
    /// <param name="snapArea"></param>
    /// <returns></returns>
    public bool TryGetFirstValidSnapArea(Snapper currentSnapper, out SnapArea snapArea, out RaycastHit validHit)
    {
        if (!SnapRaycastValid)
        {
            snapArea = null;
            validHit = new RaycastHit();
            return false;
        }

        // first first snap area that can be snapped to
        var match = SnapRaycastHits
            .Select(hit => new
            {
                Hit = hit,
                SnapArea = hit.collider.GetComponent<SnapArea>()
            })
            .FirstOrDefault(x => x.SnapArea != null && currentSnapper.CanSnapToSnapArea(x.SnapArea));

        if (match != null)
        {
            snapArea = match.SnapArea;
            validHit = match.Hit;
            ValidSnapRaycastHit = validHit;
            return true;
        }

        snapArea = null;
        validHit = new RaycastHit();
        return false;

    }
    // public bool TryGetSnapArea(out SnapArea[] snapAreas)
    // {
    //     if (!SnapRaycastValid)
    //     {
    //         snapAreas = null;
    //         return false;
    //     }

    //     // assumes that snap areas are on the same layer.
    //     if (SnapRaycastHits.Any(h => h.collider.TryGetComponent<SnapArea>(out var snapArea)))
    //     {
    //         snapAreas = SnapRaycastHits.Select(h => h.collider.GetComponent<SnapArea>()).ToArray();
    //         return true;
    //     }
    //     else
    //     {
    //         snapAreas = null;
    //         return false;
    //     }
    // }

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