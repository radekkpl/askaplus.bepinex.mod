using Il2CppSystem;
using SSSGame;
using UnityEngine;

namespace askaplus.bepinex.mod
{
    internal class AskaPlusSpawner : SubcomponentSpawner
    {
        public HarvestInteraction harvestInteraction;
        private Action onFullyHarvestedDelegate;
        private Action onHarvestedDamageTakenDelegate;
        public bool UseFullyHarvested;

        public void Start()
        {
            Plugin.Log.LogMessage($"bonusSpawner. START {gameObject.GetInstanceID()}");

            if (harvestInteraction == null)
            {
                Plugin.Log.LogError("HarvestInteraction not assigned!");
                return;
            }


            if (UseFullyHarvested)
            {
                Plugin.Log.LogMessage($"Adding delegate to onFullyHarvested");
                onFullyHarvestedDelegate = new System.Action(OnFullyHarvested);
                harvestInteraction.add_OnFullyHarvested(onFullyHarvestedDelegate);
            }
            else 
            {
                Plugin.Log.LogMessage($"Adding delegate to onHarvestDamageTaken");
                onHarvestedDamageTakenDelegate = new System.Action(OnHarvestDamageTaken);
                harvestInteraction.add_OnHarvestDamageTaken(onHarvestedDamageTakenDelegate);
            }
        }

        private void OnFullyHarvested()
        {
            Plugin.Log.LogMessage($"Running AskaPlusSpawner with amount = {amount} on game object {gameObject.name}({gameObject.GetInstanceID()})");

            if (onFullyHarvestedDelegate != null)
            {
                Plugin.Log.LogMessage($"Removing onFullyHarvestedDelegate");
                harvestInteraction.remove_OnFullyHarvested(onFullyHarvestedDelegate);
            }
            Run();
            Plugin.Log.LogInfo("Deleting bonusspawner - fully harvested");
            // Pozdější zničení sebe sama
            MonoBehaviour.Destroy(this,2f);
        }
        private void OnHarvestDamageTaken()
        {
            var health = harvestInteraction._healthModifier?.GetHealth() ?? 0;
            Plugin.Log.LogMessage($"Harvest damage taken on game object {gameObject.name}({gameObject.GetInstanceID()}) with health: {health}");

            if (health <= 0) 
            {
                Plugin.Log.LogMessage($"Removing onHarvestDamageTakenDelegate");
                harvestInteraction.remove_OnHarvestDamageTaken(onHarvestedDamageTakenDelegate);
                Run();
                Plugin.Log.LogInfo("Deleting bonusspawner -  On HarvestedDamageTaken with remaining healt <= 0)");
                // Pozdější zničení sebe sama
                MonoBehaviour.Destroy(this, 2f);
            }
        }

        private void OnDestroy()
        {
            Plugin.Log.LogMessage($"OnDestroy");
            onFullyHarvestedDelegate = null;
            onHarvestedDamageTakenDelegate = null;
        }
    }
}
