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
        private bool activated;
        private static LandscapeScanner activeScanner;
        public GameObject cameraGameObject;
        public GameObject vfxGameObject;

        public override void Start()
        {
            base.Start();

            cameraGameObject.SetActive(false);
            vfxGameObject.SetActive(false);
        }

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
            if (activated || isInFactory) return;

            activated = true;
            if (activeScanner == null)
            {
                activeScanner = this;
                cameraGameObject.SetActive(true);
                vfxGameObject.SetActive(true);
                animator.SetBool("IsMainScanner", true);
            }

            animator.SetTrigger("OpenTrigger");
            //sounds
        }

        private void ChangeStateToDeactivated()
        {
            if (!activated) return;

            activated = false;
            if (activeScanner == this)
                activeScanner = null!;

            cameraGameObject.SetActive(false);
            vfxGameObject.SetActive(false);
            animator.SetBool("IsMainScanner", false);
            animator.SetTrigger("CloseTrigger");
            //sounds
        }
    }
}
