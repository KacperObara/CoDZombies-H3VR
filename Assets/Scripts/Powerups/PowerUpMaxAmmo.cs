using System.Collections;
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
            if (Renderer == null) // for error debugging
            {
                Debug.LogWarning("MaxAmmoPowerUp spawn failed! renderer == null Tell Kodeman");
                return;
            }

            if (animator == null)
            {
                Debug.LogWarning("MaxAmmoPowerUp spawn failed! animator == null Tell Kodeman");
                return;
            }

            transform.position = pos;
            Renderer.enabled = true;
            animator.Play("Rotating");
            StartCoroutine(DespawnDelay());
        }

        public override void ApplyModifier()
        {
            foreach (FVRQuickBeltSlot slot in GM.CurrentPlayerBody.QuickbeltSlots)
            {
                FVRFireArmMagazine magazine = slot.CurObject as FVRFireArmMagazine;
                if (magazine)
                {
                    magazine.ReloadMagWithType(magazine.DefaultLoadingPattern.Classes[0]);
                }

                FVRFireArmClip clip = slot.CurObject as FVRFireArmClip;
                if (clip)
                {
                    clip.ReloadClipWithType(clip.DefaultLoadingPattern.Classes[0]);
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