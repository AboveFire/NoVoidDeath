using GameNetcodeStuff;
using HarmonyLib;

namespace NoVoidDeath
{
    internal class OutOfBoundsTrigger_Patch
    {
        private static StartOfRound playersManager;

        [HarmonyPatch(typeof(OutOfBoundsTrigger), "Start")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        internal static void Start_Patch()
        {
            playersManager = UnityEngine.Object.FindObjectOfType<StartOfRound>();
        }

        [HarmonyPatch(typeof(OutOfBoundsTrigger), "OnTriggerEnter")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        internal static bool OnTriggerEnter_Prefix(OutOfBoundsTrigger __instance, ref UnityEngine.Collider other)
        {
            if ((__instance.disableWhenRoundStarts && !playersManager.inShipPhase) || other.tag != "Player")
            {
                return true;
            }
            PlayerControllerB component = other.GetComponent<PlayerControllerB>();
            if (component == null || GameNetworkManager.Instance.localPlayerController != component || !playersManager.shipDoorsEnabled)
            {
                return true;
            }
            if (component.isInsideFactory)
            {
                if (!StartOfRound.Instance.isChallengeFile)
                {
                    // Here that's when a player get's killed
                    component.ResetFallGravity();
                    switch (Plugin.TpLocationConfig)
                    {
                        case Plugin.LocationType.Ship:
                            component.isInsideFactory = false;
                            component.TeleportPlayer(UnityEngine.Object.FindObjectOfType<StartOfRound>().outsideShipSpawnPosition.position);
                            break;
                        case Plugin.LocationType.Entrance:
                            component.TeleportPlayer(UnityEngine.Object.FindObjectOfType<EntranceTeleport>().entrancePoint.position);
                            break;
                        default:
                            // Dunno what we're doing here if default, I guess we just let everything play out normally
                            return true;
                    }
                    Plugin.Log("Saved from falling out of bounds!");

                    return false;
                }
            }
            return true;
        }
    }
}
