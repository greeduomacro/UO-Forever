using System;
using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items
{
	[FlipableAttribute( 0x19BC, 0x19BD )]
	public abstract class HalloweenMask : Item
	{
		public abstract int BodyMod{ get; }
		public abstract int HueMod{ get; }
		public virtual int Rarity{ get{ return 1; } }

        public virtual DateTime UsableYear { get; set; }

		public override bool DisplayLootType{ get{ return false; } }

		public HalloweenMask() : base ( 0x19BC )
		{
			Layer = Layer.FirstValid;
			Weight = 1.0;
		    UsableYear = DateTime.UtcNow;
            Hue = 1358;
		}

		public HalloweenMask( Serial serial ) : base ( serial )
		{
		}

		public override bool OnEquip( Mobile from )
		{
			//if ( (from.BodyMod == 0 && !TransformationSpellHelper.UnderTransformation( from ) && BaseHalloweenGiftGiver.IsHalloween()) || from.AccessLevel >= AccessLevel.Player )
            DateTime now = DateTime.UtcNow;
            int day = now.Day;
            int month = now.Month;
            bool isOneWeekBeforeOrAfterHalloween = (month == 10 && day >= 24) || (month == 11 && day <= 7);

		    if ((UsableYear.Year != DateTime.UtcNow.Year) && (month != 10 || day != 31))
		    {
                from.SendMessage("You this mask is only wearable during the Halloween of: " + UsableYear.Year + " or on Halloween day.");
		        return false;
		    }
            if ((from.BodyMod == 0 && !TransformationSpellHelper.UnderTransformation(from) && isOneWeekBeforeOrAfterHalloween) || from.AccessLevel > AccessLevel.Player)
				return base.OnEquip( from );
			else
			{
				from.SendMessage( "You do not believe it would be appropriate to wear the costume now." );
				return false;
			}
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );
			AddEffect(); //parent checks are done here
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;
                TransformationSpellHelper.RemoveContextNoHue(from, true, false); //assume resetGraphics = true
			}
		}

		public virtual void AddEffect()
		{
			if ( Parent is Mobile )
			{
				Mobile from = (Mobile)Parent;
				if ( from.Mounted )
				{
					IMount mount = (IMount)from.Mount;
					mount.Rider = null;
				}

				from.BodyMod = BodyMod;
				TransformationSpellHelper.AddContext( from, new TransformContext( null, typeof(HalloweenMask), null ) );
			}
		}

		private void ForceUnEquip()
		{
			if ( Parent is Mobile )
			{
				Mobile from = (Mobile)Parent;
				if ( from.AddToBackpack( this ) )
					from.SendMessage( "The spirit of halloween has left.  You put the halloween mask in your backpack." );
				else
				{
					from.BankBox.AddItem( this );
					from.SendMessage( "The spirit of halloween has left.  You put the halloween mask in your bankbox." );
				}
			}
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Halloween " + UsableYear.Year, 1358);
        }

        public static Item Randomize()
        {
            Type[] types =
                        {
                            typeof(BrownDrakeCostume), typeof(CyclopsCostume), typeof(EvilWispCostume),
                            typeof(GazerCostume), typeof(LichCostume),
                            typeof(LizardmanCostume), typeof(MummyCostume), typeof(OphidianMatriarchCostume),
                            typeof(OphidianWarriorCostume), typeof(RatmanCostume), typeof(RedDrakeCostume),
                            typeof(ShadowWyrmCostume), typeof(SkeletonCostume), typeof(TerathanAvengerCostume),
                            typeof(TerathanDroneCostume), typeof(TerathanMatriarchCostume), typeof(TerathanWarriorCostume),
                            typeof(TransgenderCostume), typeof(WispCostume), typeof(ZombieCostume)
                        };
            var item = Loot.Construct(types);

            if (item.TypeEquals(typeof(BrownDrakeCostume)) || item.TypeEquals(typeof(RedDrakeCostume)) || item.TypeEquals(typeof(ShadowWyrmCostume)))
            {
                return (Utility.RandomDouble() < 0.4 ? item : Loot.Construct(types));
            }

            return item;
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

            writer.WriteEncodedInt(1); // version

            writer.Write(UsableYear);
			if ( !BaseHalloweenGiftGiver.IsHalloween() )
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( ForceUnEquip ) );
			//else
			//	Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( AddEffect ) );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );


            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                {
                    UsableYear = reader.ReadDateTime();
                    if (UsableYear == DateTime.MinValue)
                    {
                        UsableYear = new DateTime(2013, 10, 31);
                    }
                }
                    goto case 0;
                case 0:
                    {
                    }
                    break;
            }

			if ( !BaseHalloweenGiftGiver.IsHalloween() )
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( ForceUnEquip ) );
			else
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( AddEffect ) );
		}
	}
}