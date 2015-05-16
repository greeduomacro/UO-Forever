#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CentralGump;
using Server.Items;
using Server.Mobiles;
using VitaNex;

#endregion

namespace Server.PvPTemplates
{
    public class TemplateProfile : PropertyObject, IEnumerable<Template>
	{
		private static readonly Layer[] _InvalidLayers = new[] {//
			Layer.Hair, Layer.FacialHair, Layer.Face, // 
			Layer.ShopBuy, Layer.ShopResale, Layer.ShopSell, //
			Layer.Mount, Layer.Invalid, //
			Layer.Bank, Layer.Backpack // 
		};

	    private static readonly Layer[] _EquipLayers = ((Layer)0).GetValues<Layer>().Not(_InvalidLayers.Contains).ToArray();

        [CommandProperty(PvPTemplates.Access, true)]
        public bool Deleted { get; private set; }

        [CommandProperty(PvPTemplates.Access, true)]
        public PlayerMobile Owner { get; private set; }

        public List<Item> Equipment { get; private set; }
        public List<Item> PackItems { get; private set; }

        private TemplateCollection Templates { get; set; }

        [CommandProperty(PvPTemplates.Access, true)]
        public Template Selected { get; private set; }

        [CommandProperty(PvPTemplates.Access, true)]
        public bool Active { get; private set; }


        public TemplateProfile(PlayerMobile owner)
        {
            Owner = owner;

            Equipment = new List<Item>();
            PackItems = new List<Item>();

            Templates = new TemplateCollection();

            Selected = null;
        }

        public TemplateProfile(GenericReader reader)
            : base(reader)
        {}

        public override void Clear()
        {
            if(!Deleted)
            {
                Templates.Clear();
            }
        }

        public override void Reset()
        {
            if(!Deleted)
            {
                Templates.Reset();
            }
        }

        public bool Select(Template template)
        {
            if(Deleted || !Contains(template))
            {
                return false;
            }

            Template old = Selected;

            Selected = template;
            PvPTemplates.InvokeTemplateSelected(this, old);
            return true;
        }

        public Template Create(
            string name,
            string notes = null,
            IDictionary<SkillName, double> skills = null,
            IDictionary<StatType, int> stats = null)
        {
            if(Deleted)
            {
                return null;
            }

            Template template = Templates.Create(name, notes, skills, stats);

            if(template != null)
            {
                PvPTemplates.InvokeTemplateCreated(this, template);
            }

            return template;
        }

        public void Add(Template template)
        {
            if(!Deleted)
            {
                Templates.Add(template);
            }
        }

        public bool Remove(Template template)
        {
            if(Selected == template)
            {
                Selected = null;
            }

            if(!Templates.Remove(template))
            {
                return false;
            }

            PvPTemplates.InvokeTemplateDeleted(this, template);
            return true;
        }

        public bool Remove(TemplateSerial uid)
        {
            Template template;

            return TryFind(uid, out template) && Remove(template);
        }

        public bool Contains(Template template)
        {
            return Templates.Contains(template);
        }

        public bool Contains(TemplateSerial uid)
        {
            return Templates.Contains(uid);
        }

        public Template Find(TemplateSerial uid)
        {
            return Templates.Find(uid);
        }

        public bool TryFind(TemplateSerial uid, out Template template)
        {
            return Templates.TryFind(uid, out template);
        }

        public void ApplyDelta(bool equipment)
        {
            if(Deleted)
            {
                return;
            }

            if (CentralGump.EnsureProfile(Owner).DisablePvPTemplate)
            {
                return;
            }

            ApplyTemplate();

            if (equipment && !CentralGump.EnsureProfile(Owner).DisableTemplateEquipment)
            {
                GiveEquipment();
            }
        }

        public void ClearDelta()
        {
            if(Deleted)
            {
                return;
            }

            ClearTemplate();
            TakeEquipment();
        }

        public void ApplyTemplate()
        {
            if(Deleted)
            {
                return;
            }

            ClearTemplate();

            if(Selected == null || Owner == null)
            {
                return;
            }

            Dictionary<SkillName, double> skills = Selected.Skills;
            Dictionary<StatType, int> stats = Selected.Stats;

            if(skills == null || stats == null)
            {
                return;
            }

            string name;

	        int mStr =(stats.ContainsKey(StatType.Str) ? stats[StatType.Str] : Owner.RawStr) - Owner.RawStr;
			int mDex = (stats.ContainsKey(StatType.Dex) ? stats[StatType.Dex] : Owner.RawDex) - Owner.RawDex;
			int mInt = (stats.ContainsKey(StatType.Int) ? stats[StatType.Int] : Owner.RawInt) - Owner.RawInt;

            if(mStr != 0)
            {
                name = String.Format("Template[{0}][Str]", Selected.UID);
                new TemplateStatMod(StatType.Str, name, mStr).ApplyTo(Owner);
                Owner.Hits = Owner.Str;
            }

            if(mDex != 0)
            {
                name = String.Format("Template[{0}][Dex]", Selected.UID);
                new TemplateStatMod(StatType.Dex, name, mDex).ApplyTo(Owner);
                Owner.Stam = Owner.Dex;
            }

            if(mInt != 0)
            {
                name = String.Format("Template[{0}][Int]", Selected.UID);
                new TemplateStatMod(StatType.Int, name, mInt).ApplyTo(Owner);
                Owner.Mana = Owner.Int;
            }

            foreach(KeyValuePair<SkillName, double> kv in skills)
            {
                name = String.Format("Template[{0}][{1}]", Selected.UID, kv.Key);
                new TemplateSkillMod(kv.Key, name, kv.Value).ApplyTo(Owner);
            }

            Active = true;

            Owner.SendMessage(54, "You are currently using the template: " + Selected.Name);
        }

        public void ClearTemplate()
        {
            if(Deleted || Owner == null)
            {
                return;
            }

            foreach(TemplateSkillMod sm in Owner.SkillMods.OfType<TemplateSkillMod>().ToArray())
            {
                sm.RemoveFrom(Owner);
            }

            foreach(TemplateStatMod sm in Owner.StatMods.OfType<TemplateStatMod>().ToArray())
            {
                sm.RemoveFrom(Owner);
            }

            Active = false;
        }

        public void GiveEquipment()
        {
            if(Deleted || Owner == null)
            {
                return;
            }

            TakeEquipment();

            if(Owner.Dex > 80)
            {
                Equipment.AddRange(
                    new Item[]
                    {
                        new CloseHelm
                        {
                            Quality = ArmorQuality.Exceptional,
                            Association = 51,
                            Weight = 0,
                        },
                        new ChainChest
                        {
                            Quality = ArmorQuality.Exceptional,
                            Association = 51,
                            Weight = 0,
                        },
                        new ChainLegs
                        {
                            Quality = ArmorQuality.Exceptional,
                            Association = 51,
                            Weight = 0,
                        },
                        new RingmailArms
                        {
                            Quality = ArmorQuality.Exceptional,
                            Association = 51,
                            Weight = 0,
                        },
                        new RingmailGloves
                        {
                            Quality = ArmorQuality.Exceptional,
                            Association = 51,
                            Weight = 0,
                        }
                    });
            }
            else
            {
                Equipment.AddRange(
                    new Item[]
                    {
                        new LeatherChest
                        {
                            Quality = ArmorQuality.Exceptional,
                            Association = 51,
                            Weight = 0,
                        },
                        new LeatherLegs
                        {
                            Quality = ArmorQuality.Exceptional,
                            Association = 51,
                            Weight = 0,
                        },
                        new LeatherGorget
                        {
                            Quality = ArmorQuality.Exceptional,
                            Association = 51,
                            Weight = 0,
                        },
                        new LeatherArms
                        {
                            Quality = ArmorQuality.Exceptional,
                            Association = 51,
                            Weight = 0,
                        },
                        new LeatherGloves
                        {
                            Quality = ArmorQuality.Exceptional,
                            Association = 51,
                            Weight = 0,
                        }
                    });
            }

            Equipment.AddRange(
                new Item[]
                {
                    new Sandals(), new Cloak()
                });

            PackItems.AddRange(
                new Item[]
                {
                    new TemplateSpellBook(), 
                    new TemplateBagOfReagents(100),
                    new TemplateBandages()
                });

            PackItems.AddRange(
                new Item[]
                {
                    new TemplateTotalRefreshPotion(),
                    new TemplateGreaterExplosionPotion{Amount = 20},
                    new TemplateGreaterCurePotion(),
                    new TemplateGreaterHealPotion()
                });

            if(Owner.Skills.Poisoning.Value > 50)
            {
                PackItems.Add(
                    new TemplateDeadlyPoisonPotion
                    {
                        Association = 51,
                        Weight = 0,
                        Stackable = true,
                        Amount = 20
                    });
            }

            if(Owner.Skills.Fencing.Value > 50)
            {
                Equipment.AddRange(
                    new Item[]
                    {
                        new WarFork
                        {
                            Association = 51,
                            Quality = WeaponQuality.Exceptional,
                            Weight = 0
                        },
                        new ShortSpear
                        {
                            Association = 51,
                            Quality = WeaponQuality.Exceptional,
                            Weight = 0
                        }
                    });
            }

            if(Owner.Skills.Swords.Value > 50 && Owner.Skills.Lumberjacking.Value == 0 ||
               Owner.Skills.Swords.Value > 50 && Owner.Int > 50)
            {
                Equipment.Add(
                    new Halberd
                    {
                        Association = 51,
                        Quality = WeaponQuality.Exceptional,
                        Weight = 0
                    });
                PackItems.Add(
                    new Katana
                    {
                        Association = 51,
                        Quality = WeaponQuality.Exceptional,
                        Weight = 0
                    });
            }

            if(Owner.Skills.Swords.Value > 50 && Owner.Skills.Lumberjacking.Value > 50)
            {
                Equipment.Add(
                    new TwoHandedAxe
                    {
                        Association = 51,
                        Quality = WeaponQuality.Exceptional,
                        Weight = 0
                    });
                PackItems.AddRange(
                    new Item[]
                    {
                        new Halberd
                        {
                            Association = 51,
                            Quality = WeaponQuality.Exceptional,
                            Weight = 0
                        },
                        new DoubleAxe
                        {
                            Association = 51,
                            Quality = WeaponQuality.Exceptional,
                            Weight = 0
                        },
                        new Katana
                        {
                            Association = 51,
                            Quality = WeaponQuality.Exceptional,
                            Weight = 0
                        }
                    });
            }

            if(Owner.Skills.Macing.Value > 50)
            {
                Equipment.Add(
                    new WarAxe
                    {
                        Association = 51,
                        Quality = WeaponQuality.Exceptional,
                        Weight = 0
                    });
                PackItems.Add(
                    new WarHammer
                    {
                        Association = 51,
                        Quality = WeaponQuality.Exceptional,
                        Weight = 0
                    });
            }

            if(Owner.Skills.Archery.Value > 50)
            {
                Equipment.Add(
                    new Bow
                    {
                        Association = 51,
                        Quality = WeaponQuality.Exceptional,
                        Weight = 0
                    });
                PackItems.AddRange(
                    new Item[]
                    {
                        new HeavyCrossbow
                        {
                            Association = 51,
                            Quality = WeaponQuality.Exceptional,
                            Weight = 0
                        },
                        new TemplateArrows(),
                        new TemplateBolts()
                    });
            }

            if(Owner.Skills.Parry.Value > 50)
            {
                Equipment.Add(
                    new MetalKiteShield
                    {
                        Association = 51,
                        Quality = ArmorQuality.Exceptional,
                        MaxHitPoints = 10000,
                        HitPoints = 10000,
                        Weight = 0,
                    });
            }

            foreach(Item i in Equipment.Where(item => !Owner.EquipItem(item) && !Owner.AddToBackpack(item)).ToArray()
                )
            {
                i.Delete();
                Equipment.Remove(i);
            }

            foreach(Item i in PackItems.Where(item => !Owner.AddToBackpack(item)).ToArray())
            {
                i.Delete();
                PackItems.Remove(i);
            }
        }

        public void TakeEquipment()
        {
            if(Deleted)
            {
                return;
            }

            Owner.DropHolding();
			
            foreach(Item i in Equipment)
			{
				if (i is Container)
				{
					var items = ((Container)i).FindItemsByType<Item>(
						true, item => item != null && !item.Deleted && !Equipment.Contains(item) && !PackItems.Contains(item));

					foreach (var item in items)
					{
						Owner.Backpack.DropItem(item);
					}
				}

                i.Delete();
            }
			
            foreach(Item i in PackItems)
			{
				if (i is Container)
				{
					var items = ((Container)i).FindItemsByType<Item>(
						true, item => item != null && !item.Deleted && !Equipment.Contains(item) && !PackItems.Contains(item));

					foreach (var item in items)
					{
						Owner.Backpack.DropItem(item);
					}
				}

				i.Delete();
            }
			
	        var assoc51 = Owner.Backpack.FindItemsByType<Item>(true, i => i != null && !i.Deleted && i.Association == 51);

	        foreach (var i in assoc51)
	        {
		        i.Delete();
	        }

	        Equipment.Clear();
            PackItems.Clear();
        }

		public void UndressToPack(params Layer[] layers)
		{
			if (Deleted || Owner == null)
			{
				return;
			}

			Owner.Items.Not(
				i =>
				i == null || i.Deleted || !i.Movable || i == Owner.Backpack || i == Owner.FindBankNoCreate() || i == Owner.Mount)
				 .Not(i => _InvalidLayers.Contains(i.Layer))
				 .Where(item => layers == null || layers.Length == 0 || layers.Contains(item.Layer))
				 .ForEach(Owner.Backpack.DropItem);
		}

        public void UndressToBank()
        {
            if(Deleted || Owner == null)
            {
                return;
            }

			UndressToPack(_EquipLayers);

            var bag = new Bag
            {
                Name = "Equipment",
                Hue = 33
            };

            foreach(Item item in Owner.Backpack.Items.ToArray())
            {
                bag.DropItem(item);
            }

            Owner.BankBox.DropItem(bag);
        }

        public void Delete()
        {
            if(Deleted)
            {
                return;
            }

            TakeEquipment();
            ClearTemplate();

            Templates.Clear();

            Owner = null;

            Deleted = true;
        }

        public IEnumerator<Template> GetEnumerator()
        {
            return Templates.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

            #region Critical Non-Versioned Values

            writer.Write(Owner);
            Templates.Serialize(writer);

            writer.Write(Selected != null ? Selected.UID : null);

            #endregion

            switch(version)
            {
                case 0:
                    {
                        writer.WriteBlockList(PackItems, writer.Write);
                        writer.WriteBlockList(Equipment, writer.Write);
                    }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            #region Critical Non-Versioned Values

            Owner = reader.ReadMobile<PlayerMobile>();
            Templates = new TemplateCollection(reader);
            Selected = Templates.Find(reader.ReadHashCode<TemplateSerial>());

            #endregion

            switch(version)
            {
                case 0:
                    {
                        PackItems = reader.ReadBlockList(reader.ReadItem);
                        Equipment = reader.ReadBlockList(reader.ReadItem);
                    }
                    break;
            }
        }
    }
}