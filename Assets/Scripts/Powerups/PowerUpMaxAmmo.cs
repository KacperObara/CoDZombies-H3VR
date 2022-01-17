using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Gamemode;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups
{
    public class PowerUpMaxAmmo : PowerUp
    {
        private Animator animator;
        public MeshRenderer Renderer;

        private void Awake()
        {
            animator = transform.GetComponent<Animator>();
        }

        public override void Spawn(Vector3 pos)
        {
            transform.position = pos;
            Renderer.enabled = true;
            animator.Play("Rotating");
            StartCoroutine(DespawnDelay());
        }

        public override void ApplyModifier()
        {
            foreach (FVRQuickBeltSlot slot in GM.CurrentPlayerBody.QuickbeltSlots)
            {
                MagazineWrapper magazineWrapper = null;
                FVRFireArmMagazine magazine = slot.CurObject as FVRFireArmMagazine;
                if (magazine)
                {
                    magazineWrapper = magazine.GetComponent<MagazineWrapper>();
                    if (magazineWrapper)
                        magazine.ReloadMagWithType(magazineWrapper.RoundClass);
                    else
                        magazine.ReloadMagWithType(magazine.DefaultLoadingPattern.Classes[0]);
                }

                FVRFireArmClip clip = slot.CurObject as FVRFireArmClip;
                if (clip)
                {
                    magazineWrapper = clip.GetComponent<MagazineWrapper>();
                    if (magazineWrapper)
                        clip.ReloadClipWithType(magazineWrapper.RoundClass);
                    else
                        clip.ReloadClipWithType(clip.DefaultLoadingPattern.Classes[0]);
                }

                Speedloader speedloader = slot.CurObject as Speedloader;
                if (speedloader)
                {
                    magazineWrapper = speedloader.GetComponent<MagazineWrapper>();
                    if (magazineWrapper)
                        speedloader.ReloadClipWithType(magazineWrapper.RoundClass);
                    else
                    {
                        try
                        {
                            List<FVRObject> compatibleRounds =
                                IM.OD[speedloader.ObjectWrapper.ItemID].CompatibleSingleRounds;
                            FVRFireArmRound round = compatibleRounds[0].GetGameObject().GetComponent<FVRFireArmRound>();
                            speedloader.ReloadClipWithType(round.RoundClass);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning("Raygun failed to reload it's ammo, tell Kodeman");
                            Debug.LogError(e);
                        }
                    }
                }
            }

            AudioManager.Instance.PowerUpMaxAmmoSound.Play();

            Despawn();
        }

        private void Despawn()
        {
            transform.position += new Vector3(0, -1000f, 0);
        }

        private IEnumerator DespawnDelay()
        {
            yield return new WaitForSeconds(15f);

            for (int i = 0; i < 5; i++)
            {
                Renderer.enabled = false;
                yield return new WaitForSeconds(.3f);
                Renderer.enabled = true;
                yield return new WaitForSeconds(.7f);
            }

            Despawn();
        }
    }
}