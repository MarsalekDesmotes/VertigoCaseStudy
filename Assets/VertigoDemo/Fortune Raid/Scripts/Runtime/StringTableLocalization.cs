using System.Collections.Generic;
using UnityEngine;

namespace VertigoDemo
{
    public sealed class StringTableLocalization : ILocalization
    {
        private readonly Dictionary<string, string> table;

        public StringTableLocalization(Dictionary<string, string> table)
        {
            this.table = table;
        }

        public static StringTableLocalization CreateDefaultEnglish()
        {
            return new StringTableLocalization(new Dictionary<string, string>
            {
                { LocalizationKeys.PopupRewardTitle, "REWARD SECURED!" },
                { LocalizationKeys.PopupRewardTitleSpecial, "SPECIAL REWARD!" },
                { LocalizationKeys.PopupRewardBody, "Push your luck in the next zone." },
                { LocalizationKeys.PopupRewardBodySpecial, "Limited reward added to your run." },
                { LocalizationKeys.PopupRewardCta, "CONTINUE" },
                { LocalizationKeys.PopupBombTitle, "OH NO, A BOMB EXPLODED RIGHT IN YOUR HANDS!" },
                { LocalizationKeys.PopupBombBody, "Revive yourself to keep your rewards." },
                { LocalizationKeys.PopupBombGiveUp, "GIVE UP" },
                { LocalizationKeys.PopupBombRevive, "REVIVE" },
                { LocalizationKeys.PopupCollectedTitle, "REWARDS COLLECTED" },
                { LocalizationKeys.PopupCollectedBody, "You safely left with your rewards." },
                { LocalizationKeys.PopupCollectedRewardKinds, "{0} REWARD TYPES" },
                { LocalizationKeys.PopupCollectedCta, "NEW RUN" },
                { LocalizationKeys.ZoneLabel, "ZONE {0}" },
                { LocalizationKeys.ZoneRisk, "RISK ZONE" },
                { LocalizationKeys.ZoneSafe, "SAFE ZONE" },
                { LocalizationKeys.ZoneGolden, "GOLDEN ZONE" },
                { LocalizationKeys.TransitionGoldenTitle, "GOLDEN ZONE {0}" },
                { LocalizationKeys.TransitionGoldenPunchline, "SPECIAL REWARDS. NO BOMB." },
            });
        }

        public string Get(string key)
        {
            if (table.TryGetValue(key, out string value))
            {
                return value;
            }

            Debug.LogWarning("Missing localization key: " + key);
            return key;
        }

        public string Format(string key, params object[] args)
        {
            return string.Format(Get(key), args);
        }
    }
}
