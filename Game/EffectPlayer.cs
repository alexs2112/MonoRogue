using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace MonoRogue {
    public enum EffectType {
        Alarm,
        Arrow,
        Death,
        Door,
        Eat,
        Equip,
        GoldenKey,
        HeartCrystal,
        Hit,
        MenuMove,
        MenuSelect,
        Spell,
        Victory
    }

    public class EffectPlayer {
        private static Dictionary<EffectType, SoundEffect[]> SoundEffects;

        public static void LoadSoundEffects(ContentManager content) {
            SoundEffects = new Dictionary<EffectType, SoundEffect[]>();

            SoundEffects.Add(
                EffectType.Alarm,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/Alarm0"),
                    content.Load<SoundEffect>("Audio/Effects/Alarm1")
                }
            );

            SoundEffects.Add(
                EffectType.Arrow,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/Arrow0"),
                    content.Load<SoundEffect>("Audio/Effects/Arrow1"),
                    content.Load<SoundEffect>("Audio/Effects/Arrow2")
                }
            );

            SoundEffects.Add(
                EffectType.Death,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/Death0"),
                    content.Load<SoundEffect>("Audio/Effects/Death1")
                }
            );

            SoundEffects.Add(
                EffectType.Door,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/Door0"),
                    content.Load<SoundEffect>("Audio/Effects/Door1")
                }
            );

            SoundEffects.Add(
                EffectType.Eat,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/Eat")
                }
            );

            SoundEffects.Add(
                EffectType.Equip,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/Equip")
                }
            );

            SoundEffects.Add(
                EffectType.GoldenKey,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/GoldenKey")
                }
            );

            SoundEffects.Add(
                EffectType.HeartCrystal,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/HeartCrystal")
                }
            );

            SoundEffects.Add(
                EffectType.Hit,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/Hit0"),
                    content.Load<SoundEffect>("Audio/Effects/Hit1"),
                    content.Load<SoundEffect>("Audio/Effects/Hit2"),
                    content.Load<SoundEffect>("Audio/Effects/Hit3"),
                    content.Load<SoundEffect>("Audio/Effects/Hit4")
                }
            );

            SoundEffects.Add(
                EffectType.MenuMove,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/MenuMove")
                }
            );

            SoundEffects.Add(
                EffectType.MenuSelect,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/MenuSelect")
                }
            );

            SoundEffects.Add(
                EffectType.Spell,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/Spell0"),
                    content.Load<SoundEffect>("Audio/Effects/Spell1"),
                    content.Load<SoundEffect>("Audio/Effects/Spell2")
                }
            );

            SoundEffects.Add(
                EffectType.Victory,
                new SoundEffect[] {
                    content.Load<SoundEffect>("Audio/Effects/Victory")
                }
            );
        }

        public static void PlaySoundEffect(EffectType e) {
            SoundEffect[] arr = SoundEffects[e];
            SoundEffect effect = arr[new System.Random().Next(arr.Length)];
            effect.Play(Constants.EffectVolume, 0.0f, 0.0f);
        }
    }
}
