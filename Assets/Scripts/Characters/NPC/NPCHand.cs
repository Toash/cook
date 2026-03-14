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

            heldItem = item;
        }

        public void DestroyHeldItem(float time)
        {
            Destroy(heldItem, time);
            heldItem = null;

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