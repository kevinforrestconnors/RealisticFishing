using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Quests;

namespace RealisticFishing
{
    public class RealisticFishingQuest : Quest
    {
        public string target = "Demetrius";
        public int numberToFish = 3;
        public int numberFished = 0;
        public int reward = 15000;
        public int whichFish;

        public StardewValley.Object fish;
        public List<DescriptionElement> parts = new List<DescriptionElement>();
        public List<DescriptionElement> dialogueparts = new List<DescriptionElement>();

        public DescriptionElement objective;
        public string targetMessage;

        // make a new quest
        public RealisticFishingQuest(int whichFish)
        {
            this.questType.Value = 7;
            this.whichFish = whichFish;
            this.fish = new StardewValley.Object(whichFish, 1, false, -1, 0);
        }

        // recover an existing quest
        public RealisticFishingQuest(int whichFish, int numberFished) : this(whichFish) {
            this.numberFished = numberFished;
        }

        public void loadQuestInfo()
        {
            this.questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingQuest.cs.13227");
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13228", (object)this.fish.Name, (object)this.numberToFish));
            this.dialogueparts.Clear();
            this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13231", (object)this.fish.Name, (object)("careless fishermen caught too many small " + this.fish.Name + ", which resulted in the population growing too large.")));
            this.objective = this.fish.Name.Equals("Octopus") ? new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13243", (object)0, (object)this.numberToFish) : new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13244", (object)0, (object)this.numberToFish, (object)this.fish.Name);
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13274", (object)this.reward));
            this.parts.Add((DescriptionElement)"Strings\\StringsFromCSFiles:FishingQuest.cs.13275");
        }

        public override void reloadDescription()
        {
            if (this._questDescription == "")
                this.loadQuestInfo();
            if (this.parts.Count == 0 || this.parts == null || (this.dialogueparts.Count == 0 || this.dialogueparts == null))
                return;
            string str1 = "";
            string str2 = "";
            foreach (DescriptionElement part in this.parts)
                str1 += part.loadDescriptionElement();
            foreach (DescriptionElement dialoguepart in this.dialogueparts)
                str2 += dialoguepart.loadDescriptionElement();
            this.questDescription = str1;
            this.targetMessage = str2;
        }

        public override void reloadObjective()
        {
            if ((int)this.numberFished < (int)this.numberToFish)
                this.objective = this.fish.Name.Equals("Octopus") ? new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13243", (object)this.numberFished, (object)this.numberToFish) : (this.fish.Name.Equals("Squid") ? new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13255", (object)this.numberFished, (object)this.numberToFish) : new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13244", (object)this.numberFished, (object)this.numberToFish, (object)this.fish.Name));
            if (this.objective == null)
                return;
            this.currentObjective = this.objective.loadDescriptionElement();
        }

        public override bool checkIfComplete(NPC n = null, int fishid = -1, int number2 = -1, Item item = null, string monsterName = null)
        {
            this.loadQuestInfo();
            if (n == null && fishid != -1 && (fishid == this.whichFish && this.numberFished < this.numberToFish))
            {
                this.numberFished = Math.Min(this.numberToFish, this.numberFished + 1);
                if (this.numberFished >= this.numberToFish)
                {
                    this.dailyQuest.Value = false;
                    this.objective = new DescriptionElement("Strings\\Quests:ObjectiveReturnToNPC", (object)Game1.getCharacterFromName(this.target), false);
                    Game1.playSound("jingle1");
                }
            }
            else if (n != null && this.numberFished >= this.numberToFish && (this.target != null && n.Name.Equals(this.target)) && (n.isVillager() && !this.completed))
            {
                n.CurrentDialogue.Push(new Dialogue(this.targetMessage, n));
                this.moneyReward.Value = this.reward;
                this.questComplete();
                Game1.drawDialogue(n);
                return true;
            }
            return false;
        }
    }
}
