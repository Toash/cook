namespace Assets.Scripts.Characters.NPC
{
    public class NPCEatFood : NPCState
    {

        public override string Name => "NPCEatFood";

        public override void OnEnter(NPCBrain brain)
        {
            brain.Agent.SetDestination(Brain.CurrentFoodTruck.WaitingSpot.transform.position);
            brain.NPC.Hand.DestroyHeldItem(5);

        }

        public override void OnExit(NPCBrain brain)
        {
        }

        public override void OnFixedUpdate(NPCBrain brain)
        {
        }

        public override void OnUpdate(NPCBrain brain)
        {
        }

    }
}