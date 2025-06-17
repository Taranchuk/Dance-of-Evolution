using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
namespace DanceOfEvolution
{
    [HotSwappable]
    public class Window_SkillsToSpecialize : Window
    {
        private List<SkillDef> skillsToSpecialize;
        private Action<SkillDef> action;
        public override Vector2 InitialSize => new Vector2(400f, 300f);

        public Window_SkillsToSpecialize(List<SkillDef> skillsToSpecialize, Action<SkillDef> action)
        {
            this.skillsToSpecialize = skillsToSpecialize;
            this.action = action;
            // Window settings
            doCloseButton = false;
            doCloseX = false;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
        }
        private bool doClose;
        public override void DoWindowContents(Rect inRect)
        {
            float y = 0f;
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, y, inRect.width, 40f), "DE_SelectSkillToSpecialize".Translate());
            y += 40f;

            Text.Font = GameFont.Small;

            foreach (var skill in skillsToSpecialize)
            {
                if (DrawSkillButton(skill, inRect.width, ref y))
                {
                    action(skill);
                    doClose = true;
                    Close();
                    return;
                }
            }
        }

        public override void Close(bool doCloseSound = true)
        {
            if (doClose)
            {
                base.Close(doCloseSound);
            }
        }

        private bool DrawSkillButton(SkillDef skill, float width, ref float y)
        {
            Rect buttonRect = new Rect(0f, y, width, 32);
            y += 32 + 6;
            return Widgets.ButtonText(buttonRect, skill.skillLabel.CapitalizeFirst());
        }
    }
}