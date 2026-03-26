using UnityEngine;

namespace Assets.Scripts.Characters.NPC
{
    public class NPCHand : MonoBehaviour
    {
        public Transform HandRoot;

        private GameObject heldItem;

        public void Hold(GameObject item)
        {
            item.transform.SetParent(HandRoot);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;

            DisableHoldablesRecursive(item);

            heldItem = item;
        }

        public void DestroyHeldItem(float time)
        {
            Destroy(heldItem, time);
            heldItem = null;
        }

        private static void DisableHoldablesRecursive(GameObject root)
        {
            if (root.TryGetComponent<Snapper>(out var rootSnapper))
            {
                foreach (Snapper snapper in rootSnapper.GetSnapperChildrenRecursive())
                {
                    Holdable holdable = snapper.GetComponent<Holdable>();
                    if (holdable != null)
                    {
                        holdable.enabled = false;
                    }
                }
            }

            foreach (Holdable holdable in root.GetComponentsInChildren<Holdable>(true))
            {
                holdable.enabled = false;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (HandRoot != null)
            {
                Gizmos.DrawSphere(HandRoot.position, .2f);
            }
        }
#endif
    }
}