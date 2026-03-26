using UnityEngine;

namespace Assets.Scripts.Characters.NPC
{
    public class NPCEatAtTable : NPCState
    {
        public override string Name => "NPCEatAtTable";

        private Transform targetSeat;
        private float eatTimer;
        private float eatDuration;

        public override void OnEnter(NPCBrain brain)
        {
            targetSeat = TableManager.I.GetAvailableSeat();

            if (targetSeat == null)
            {
                Brain.ChangeState("NPCLeaveRestaurant");
                return;
            }

            TableManager.I.ReserveSeat(targetSeat);

            eatDuration = 8;
            eatTimer = 0f;

            Brain.Agent.SetDestination(targetSeat.position);
        }

        public override void OnUpdate(NPCBrain brain)
        {
            // still walking to table
            if (!AgentUtils.HasAgentReachedDestination(Brain.Agent))
                return;

            eatTimer += Time.deltaTime;

            if (eatTimer >= eatDuration)
            {
                TableManager.I.ReleaseSeat(targetSeat);
                Brain.NPC.Hand.DestroyHeldItem(0);
                Brain.HasEatenToday = true;
                Brain.ChangeState("NPCLeaveRestaurant");
            }
        }

        public override void OnExit(NPCBrain brain)
        {
        }

        public override void OnFixedUpdate(NPCBrain brain)
        {
        }
    }
}