using System;
using Server.Engines.Craft;
using Server.Engines.XmlSpawner2;
using Server.Factions;

namespace Server.Items
{
	public enum GemType
	{
		None,
		StarSapphire,
		Emerald,
		Sapphire,
		Ruby,
		Citrine,
		Amethyst,
		Tourmaline,
		Amber,
		Diamond
	}

    public abstract class BaseJewel : Item, ICraftable, IFactionItem
	{
		private int m_MaxHitPoints;
		private int m_HitPoints;

		private CraftResource m_Resource;
		private GemType m_GemType;

        #region Factions
        private FactionItem m_FactionState;

        public FactionItem FactionItemState { get { return m_FactionState; } set { m_FactionState = value; } }
        #endregion

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxHitPoints
		{
			get{ return m_MaxHitPoints; }
			set{ m_MaxHitPoints = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitPoints
		{
			get
			{
				return m_HitPoints;
			}
			set
			{
				if ( value != m_HitPoints && MaxHitPoints > 0 )
				{
					m_HitPoints = value;

					if ( m_HitPoints < 0 )
						Delete();
					else if ( m_HitPoints > MaxHitPoints )
						m_HitPoints = MaxHitPoints;

					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get{ return m_Resource; }
			set{ m_Resource = value; Hue = CraftResources.GetHue( m_Resource ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public GemType GemType
		{
			get{ return m_GemType; }
			set{ m_GemType = value; InvalidateProperties(); }
		}

		public override Type DyeType{ get{ return typeof(RewardMetalDyeTub); } }

		public override bool Dye( Mobile from, IDyeTub sender )
		{
			if ( !(m_Resource == CraftResource.None || m_Resource == CraftResource.Iron) )
				return false;

			return base.Dye( from, sender );
		}

		public virtual int BaseGemTypeNumber{ get{ return 0; } }

		public virtual int InitMinHits{ get{ return 0; } }
		public virtual int InitMaxHits{ get{ return 0; } }

		public override int LabelNumber
		{
			get
			{
				if ( m_GemType == GemType.None )
					return base.LabelNumber;

				return BaseGemTypeNumber + (int)m_GemType - 1;
			}
		}

		public override void OnAfterDuped( Item newItem )
		{
			BaseJewel jewel = newItem as BaseJewel;

			if ( jewel == null )
				return;
		}

		public virtual int ArtifactRarity{ get{ return 0; } }

		public BaseJewel( int itemID, Layer layer ) : base( itemID )
		{
			m_Resource = CraftResource.Iron;
			m_GemType = GemType.None;

			Layer = layer;

			m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax( InitMinHits, InitMaxHits );
		}

        public override void OnAdded( object parent )
		{
            // true if return override
            

            // XmlAttachment check for OnEquip and CanEquip
            if (parent is Mobile)
            {
                if (XmlScript.HasTrigger(this, TriggerName.onEquip) && UberScriptTriggers.Trigger(this, (Mobile)parent, TriggerName.onEquip))
                {
                    ((Mobile)parent).AddToBackpack(this); // override, put it in their pack
                    base.OnAdded(parent);
                    return;
                }

                /* Don't care about XmlSpawner no more!
                if (Server.Engines.XmlSpawner2.XmlAttach.CheckCanEquip(this, (Mobile)parent))
                {
                    Server.Engines.XmlSpawner2.XmlAttach.CheckOnEquip(this, (Mobile)parent);
                }
                else
                {
                    ((Mobile)parent).AddToBackpack(this);
                }*/
            }
            else if (parent is Item)
            {
                Item parentItem = (Item)parent;
                if (XmlScript.HasTrigger(this, TriggerName.onAdded))
                    UberScriptTriggers.Trigger(this, parentItem.RootParentEntity as Mobile, TriggerName.onAdded, parentItem);
            }

            base.OnAdded(parent);
		}

		public override void OnRemoved( object parent )
		{
            if (parent is Mobile)
            {
                if (XmlScript.HasTrigger(this, TriggerName.onUnequip))
                    UberScriptTriggers.Trigger(this, (Mobile)parent, TriggerName.onUnequip);
            }
            else if (parent is Item)
            {
                Item parentItem = (Item)parent;
                if (XmlScript.HasTrigger(this, TriggerName.onRemove))
                    UberScriptTriggers.Trigger(this, parentItem.RootParentEntity as Mobile, TriggerName.onRemove, parentItem);
            }

            //Server.Engines.XmlSpawner2.XmlAttach.CheckOnRemoved(this, parent);
            base.OnRemoved(parent);
		}

        public override void OnDelete()
        {
            if (XmlScript.HasTrigger(this, TriggerName.onDelete))
                UberScriptTriggers.Trigger(this, this.RootParentEntity as Mobile, TriggerName.onDelete);
            base.OnDelete();
        }
 
		public BaseJewel( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			int prop;

			if ( (prop = ArtifactRarity) > 0 )
				list.Add( 1061078, prop.ToString() ); // artifact rarity ~1_val~

            #region Factions
            if (m_FactionState != null)
            {
                list.Add(1041350); // faction item
            }
            #endregion

			if ( m_HitPoints >= 0 && m_MaxHitPoints > 0 )
				list.Add( 1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints ); // durability ~1_val~ / ~2_val~
		}

		public override void Serialize( GenericWriter writer )
		{
            #region Factions
            if (m_FactionState != null && m_FactionState.HasExpired)
            {
                m_FactionState.Detach();
            }
            #endregion

			base.Serialize( writer );

			writer.Write( (int) 4 ); // version

			writer.WriteEncodedInt( (int) m_MaxHitPoints );
			writer.WriteEncodedInt( (int) m_HitPoints );

			writer.WriteEncodedInt( (int) m_Resource );
			writer.WriteEncodedInt( (int) m_GemType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                case 4:
                {
                    m_MaxHitPoints = reader.ReadEncodedInt();
                    m_HitPoints = reader.ReadEncodedInt();
                    m_Resource = (CraftResource)reader.ReadEncodedInt();
                    m_GemType = (GemType)reader.ReadEncodedInt();
                    // DO NOT GO ON TO CASE 3!
                    break;
                }
                case 3:
				{
					m_MaxHitPoints = reader.ReadEncodedInt();
					m_HitPoints = reader.ReadEncodedInt();

					goto case 2;
				}
				case 2:
				{
					m_Resource = (CraftResource)reader.ReadEncodedInt();
                    m_GemType = (GemType)reader.ReadEncodedInt();
					goto case 1;
				}
				case 1:
				{
					new AosAttributes( this, reader );
					new AosElementAttributes( this, reader );
					new AosSkillBonuses( this, reader );
					if ( Parent is Mobile )
						((Mobile)Parent).CheckStatTimers();

					break;
				}
				case 0:
				{
					new AosAttributes( this );
					new AosElementAttributes( this );
					new AosSkillBonuses( this );

					break;
				}
			}

			if ( version < 2 )
			{
				m_Resource = CraftResource.Iron;
				m_GemType = GemType.None;
			}
		}
		#region ICraftable Members

		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, IBaseTool tool, CraftItem craftItem, int resHue )
		{
			Type resourceType = typeRes;

			if ( resourceType == null )
				resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

			Resource = CraftResources.GetFromType( resourceType );

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;

			if ( 1 < craftItem.Resources.Count )
			{
				resourceType = craftItem.Resources.GetAt( 1 ).ItemType;

				if ( resourceType == typeof( StarSapphire ) )
					GemType = GemType.StarSapphire;
				else if ( resourceType == typeof( Emerald ) )
					GemType = GemType.Emerald;
				else if ( resourceType == typeof( Sapphire ) )
					GemType = GemType.Sapphire;
				else if ( resourceType == typeof( Ruby ) )
					GemType = GemType.Ruby;
				else if ( resourceType == typeof( Citrine ) )
					GemType = GemType.Citrine;
				else if ( resourceType == typeof( Amethyst ) )
					GemType = GemType.Amethyst;
				else if ( resourceType == typeof( Tourmaline ) )
					GemType = GemType.Tourmaline;
				else if ( resourceType == typeof( Amber ) )
					GemType = GemType.Amber;
				else if ( resourceType == typeof( Diamond ) )
					GemType = GemType.Diamond;
			}

			return 1;
		}

		#endregion
	}
}