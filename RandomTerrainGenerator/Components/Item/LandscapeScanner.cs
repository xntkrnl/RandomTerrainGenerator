using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode.Components;
using UnityEngine;

namespace RandomTerrainGenerator.Components.Item
{
    internal class LandscapeScanner : GrabbableObject
    {
        [Space(10f)]
        [Header("Landscape Scanner")]
        public Animator animator;

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            ChangeStateToActivated();

            if (base.IsOwner)
                playerHeldBy.DiscardHeldObject();
        }

        public override void GrabItem()
        {
            ChangeStateToDeactivated(); 
        }

        private void ChangeStateToActivated()
        {
            Plugin.Log("activated");
            animator.SetBool("BatteryEmpty", insertedBattery.empty);
            animator.SetTrigger("OpenTrigger");
            //sound, etc
            //MOVE ANIMATOR ONE UP
        }

        private void ChangeStateToDeactivated()
        {
            Plugin.Log("deactivated");
            animator.SetTrigger("CloseTrigger");
            //sound, etc
        }
    }
}
