using System;
using System.Collections;
using System.Collections.Generic;
using CODZombies.Scripts.Common;
using CODZombies.Scripts.Managers.Sound;
using CODZombies.Scripts.Player;
using FistVR;
using UnityEngine;

namespace CODZombies.Scripts.Powerups
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
            TryToLoadAmmoInQuickbelt();
            TryToLoadAmmoInHand(PlayerData.Instance.LeftHand);
            TryToLoadAmmoInHand(PlayerData.Instance.RightHand);

            AudioManager.Instance.Play(ApplyAudio, .5f);
            Despawn();
        }

        private void TryToLoadAmmoInHand(FVRViveHand hand)
        {
            FVRInteractiveObject heldObject = hand.CurrentInteractable;
            if (heldObject && heldObject as FVRFireArm)
            {
                FVRFireArmMagazine mag = heldObject.GetComponentInChildren<FVRFireArmMagazine>();
                if (mag)
                {
                    MagazineWrapper magazineWrapper = mag.GetComponent<MagazineWrapper>();
                    if (magazineWrapper)
                        mag.ReloadMagWithType(magazineWrapper.RoundClass);
                    else if (mag.DefaultLoadingPattern.Classes.Length > 0)
                        mag.ReloadMagWithType(mag.DefaultLoadingPattern.Classes[0]);
                    // else
                    // {
                    //     List<FVRFireArmChamber> chambers = heldObject.GetComponent<FVRFireArm>().GetChambers();
                    //     foreach (var chamber in chambers)
                    //     {
                    //         FVRFireArmRound round = _SpawnRound()
                    //         chamber.SetRound(round);
                    //     }
                    // }
                }
            }
        }

        // public FVRPhysicalObject SpawnRound(string roundId)
        // {
        //     FVRObject obj = null;
        //     if (!IM.OD.TryGetValue(roundId, out obj))
        //     {
        //         Debug.LogError("Failed to spawn round with id: " + roundId);
        //         return null;
        //     }
        //
        //     var callback = obj.GetGameObject();
        //
        //     GameObject gun = Instantiate(callback, transform.position + Vector3.up, transform.rotation);
        //     gun.SetActive(true);
        //     return gun.GetComponent<FVRPhysicalObject>();
        // }

        private void TryToLoadAmmoInQuickbelt()
        {
            foreach (FVRQuickBeltSlot slot in GM.CurrentPlayerBody.QBSlots_Internal)
            {
                MagazineWrapper magazineWrapper = null;
                FVRFireArmMagazine magazine = slot.CurObject as FVRFireArmMagazine;
                if (magazine)
                {
                    magazineWrapper = magazine.GetComponent<MagazineWrapper>();
                    if (magazineWrapper)
                        magazine.ReloadMagWithType(magazineWrapper.RoundClass);
                    else if (magazine.DefaultLoadingPattern.Classes.Length > 0)
                        magazine.ReloadMagWithType(magazine.DefaultLoadingPattern.Classes[0]);
                }

                FVRFireArmClip clip = slot.CurObject as FVRFireArmClip;
                if (clip)
                {
                    magazineWrapper = clip.GetComponent<MagazineWrapper>();
                    if (magazineWrapper)
                        clip.ReloadClipWithType(magazineWrapper.RoundClass);
                    else if (clip.DefaultLoadingPattern.Classes.Length > 0)
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

                // Reload magazines inside the weapons (Internal mags included, like in tube-fed shotgun)
                FVRFireArm weapon = slot.CurObject as FVRFireArm;
                if (weapon)
                {
                    FVRFireArmMagazine mag = weapon.GetComponentInChildren<FVRFireArmMagazine>();
                    if (mag)
                    {
                        magazineWrapper = mag.GetComponent<MagazineWrapper>();
                        if (magazineWrapper)
                            mag.ReloadMagWithType(magazineWrapper.RoundClass);
                        else if (mag.DefaultLoadingPattern.Classes.Length > 0)
                            mag.ReloadMagWithType(mag.DefaultLoadingPattern.Classes[0]);
                        else if (AM.SRoundDisplayDataDic[mag.RoundType].Classes.Length > 0)
                        {
                            FireArmRoundClass roundClass = AM.SRoundDisplayDataDic[mag.RoundType].Classes[0].Class;
                            mag.ReloadMagWithType(roundClass);
                        }
                    }
                }
            }
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