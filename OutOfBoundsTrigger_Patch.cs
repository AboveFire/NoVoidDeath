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
                    component.isInsideFactory = false;
                    component.TeleportPlayer(UnityEngine.Object.FindObjectOfType<StartOfRound>().outsideShipSpawnPosition.position);
                    return false;
                }
            }
            return true;
        }
    }
}
