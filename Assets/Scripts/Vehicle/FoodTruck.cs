using UnityEngine;

namespace Assets.Scripts.Vehicle
{
    public class FoodTruck : MonoBehaviour
    {

        /// <summary>
        /// Spots NPCS wait to order something
        /// </summary>
        public OrderLine OrderLine;

        /// <summary>
        /// Spots NPCS wait while an order is being made 
        /// </summary>
        public OrderWaitingSpot WaitingSpot;

        /// <summary>
        /// Spot Player submits Made orders, and where NPCS pick up their orders.
        /// </summary>
        public SingleOrderSubmissionArea OrderSubmissionArea;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}