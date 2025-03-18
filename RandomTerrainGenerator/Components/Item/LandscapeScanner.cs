using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode.Components;
using UnityEngine;

namespace RandomTerrainGenerator.Components.Item
{
    internal class LandscapeScanner : GrabbableObject
    {
        private bool activated;
        private int antennaAngle;
        private System.Random antennaAngleRandom;

        public Animator animator;
        public InteractTrigger interactTriggerScript;

        public override void Start()
        {
            base.Start();
            antennaAngleRandom = new System.Random(StartOfRound.Instance.randomMapSeed);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);

            if (base.IsOwner)
            {
                playerHeldBy.DiscardHeldObject();
            }
        }

        public override void DiscardItem()
        {
            base.DiscardItem();
        }

        public override void EquipItem()
        {
            base.EquipItem();

            ChangeStateToDeactivated();
        }

        public override void DiscardItemFromEnemy()
        {
            base.DiscardItemFromEnemy();
        }

        public override void Update()
        {
            base.Update();
            if (playerHeldBy == null && !isHeld && !isHeldByEnemy && reachedFloorTarget && !activated && !insertedBattery.empty)
                ChangeStateToActivated();
        }

        private void ChangeStateToActivated()
        {
            activated = true;
            antennaAngle = antennaAngleRandom.Next(0, 360);
            //animator, sound, etc
        }

        private void ChangeStateToDeactivated()
        {
            activated = false;
            //animator, sound, etc
        }
    }
}
