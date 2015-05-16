using System;
using Server.Network;
using Server.Misc;

namespace Server.Items
{
	public enum TrapType
	{
		None,
		MagicTrap,
		ExplosionTrap,
		DartTrap,
		PoisonTrap
	}

	public abstract class TrapableContainer : BaseContainer, ITelekinesisable
	{
		private TrapType m_TrapType;
		private int m_TrapPower;
		private int m_TrapLevel;
		private Mobile m_TrapCreator;
        private bool m_Armed;

		public virtual bool HueOnMagicTrap { get { return false; } }

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get{ return ( HueOnMagicTrap && m_TrapPower > 0 && m_TrapType == TrapType.MagicTrap ) ? 38 : base.Hue; }
			set{ base.Hue = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TrapType TrapType
		{
			get
			{
				return m_TrapType;
			}
			set
			{
				m_TrapType = value;
                // Lets erase the owner here since that field isnt used anywhere else in the code.
	            if (value == Server.Items.TrapType.None)
                {
	                m_TrapCreator = null;
	            }
				ReleaseWorldPackets();
				Delta( ItemDelta.Update );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TrapPower
		{
			get
			{
				return m_TrapPower;
			}
			set
			{
				m_TrapPower = value;
				Delta( ItemDelta.Update );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TrapLevel
		{
			get
			{
				return m_TrapLevel;
			}
			set
			{
				m_TrapLevel = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile TrapCreator
		{
			get
			{
				return m_TrapCreator;
			}
			set
			{
				m_TrapCreator = value;
			}
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Armed
        {
            get
            {
                return m_Armed;
            }
            set
            {
                m_Armed = value;
            }
        }

		public virtual bool TrapOnOpen{ get{ return true; } }

		public TrapableContainer( int itemID ) : base( itemID )
		{
            m_Armed = true;
		}

		public TrapableContainer( Serial serial ) : base( serial )
		{
		}

		private void SendMessageTo( Mobile to, int number, int hue )
		{
			if ( Deleted || !to.CanSee( this ) )
				return;

			to.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, hue, 3, number, "", "" ) );
		}

		private void SendMessageTo( Mobile to, string text, int hue )
		{
			if ( Deleted || !to.CanSee( this ) )
				return;

			to.Send( new UnicodeMessage( Serial, ItemID, MessageType.Regular, hue, 3, to.Language, "", text ) );
		}

		public virtual bool ExecuteTrap( Mobile from )
		{
			if (m_TrapType != TrapType.None )
			{
				Point3D loc = this.GetWorldLocation();
				Map facet = this.Map;

				if ( from.AccessLevel >= AccessLevel.GameMaster )
				{
					SendMessageTo( from, "That is trapped, but you open it with your godly powers.", 0x3B2 );
					return false;
				}

				switch ( m_TrapType )
				{
					case TrapType.ExplosionTrap:
					{
						SendMessageTo( from, 502999, 0x3B2 ); // You set off a trap!

						if ( from.InRange( loc, 3 ) )
						{
							int damage;

							if ( m_TrapLevel > 0 )
								damage = Utility.RandomMinMax( 8, 20 ) * m_TrapLevel;
							else
								damage = m_TrapPower;

                            from.Damage(damage);

							// Your skin blisters from the heat!
							from.LocalOverheadMessage( Network.MessageType.Regular, 0x2A, 503000 );
						}

						Effects.SendLocationEffect( loc, facet, 0x36BD, 15, 10 );
						Effects.PlaySound( loc, facet, 0x307 );

						break;
					}
					case TrapType.MagicTrap:
					{
						if ( from.InRange( loc, 1 ) )
							from.Damage( m_TrapPower );
							//AOS.Damage( from, m_TrapPower, 0, 100, 0, 0, 0 );

						Effects.PlaySound( loc, Map, 0x307 );

						Effects.SendLocationEffect( new Point3D( loc.X - 1, loc.Y, loc.Z ), Map, 0x36BD, 15 );
						Effects.SendLocationEffect( new Point3D( loc.X + 1, loc.Y, loc.Z ), Map, 0x36BD, 15 );

						Effects.SendLocationEffect( new Point3D( loc.X, loc.Y - 1, loc.Z ), Map, 0x36BD, 15 );
						Effects.SendLocationEffect( new Point3D( loc.X, loc.Y + 1, loc.Z ), Map, 0x36BD, 15 );

						Effects.SendLocationEffect( new Point3D( loc.X + 1, loc.Y + 1, loc.Z + 11 ), Map, 0x36BD, 15 );

						//Update item for hue purposes.
						ReleaseWorldPackets();
						Delta( ItemDelta.Update );

						break;
					}
					case TrapType.DartTrap:
					{
						SendMessageTo( from, 502999, 0x3B2 ); // You set off a trap!

						if ( from.InRange( loc, 3 ) )
						{
							int damage;

							if ( m_TrapLevel > 0 )
								damage = Utility.RandomMinMax( 5, 15 ) * m_TrapLevel;
							else
								damage = m_TrapPower;

                            from.Damage(damage);

							// A dart imbeds itself in your flesh!
							from.LocalOverheadMessage( Network.MessageType.Regular, 0x62, 502998 );
						}

						Effects.PlaySound( loc, facet, 0x223 );

						break;
					}
					case TrapType.PoisonTrap:
					{
						SendMessageTo( from, 502999, 0x3B2 ); // You set off a trap!

						if ( from.InRange( loc, 3 ) )
						{
							Poison poison;

							if ( m_TrapLevel > 0 )
								poison = Poison.GetPoison( Math.Max( 0, Math.Min( 4, m_TrapLevel - 1 ) ) );
							else
								poison = null;

							if ( m_TrapPower > 0 )
							{
                                from.Damage(m_TrapPower);
								if ( m_TrapLevel == 0 )
									poison = Poison.Greater;
							}

							from.ApplyPoison( from, poison );

							// You are enveloped in a noxious green cloud!
							from.LocalOverheadMessage( Network.MessageType.Regular, 0x44, 503004 );
						}

						Effects.SendLocationEffect( loc, facet, 0x113A, 10, 20 );
						Effects.PlaySound( loc, facet, 0x231 );

						break;
					}
				}

                m_TrapCreator = null;
				m_TrapType = TrapType.None;
				m_TrapPower = 0;
				m_TrapLevel = 0;
				return true;
			}

			return false;
		}

		public virtual void OnTelekinesis( Mobile from )
		{
			Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5022 );
			Effects.PlaySound( Location, Map, 0x1F5 );

			if( this.TrapOnOpen )
				ExecuteTrap( from );
		}

		public override void Open( Mobile from )
		{
			if ( !this.TrapOnOpen || !ExecuteTrap( from ) )
				base.Open( from );
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );
			if ( ( RootParent == from && ( from == m_TrapCreator || from.AccessLevel >= AccessLevel.GameMaster ) ) && m_TrapType == TrapType.MagicTrap && !(this is ChargeableTrapPouch) && ( m_TrapPower > 0 || m_TrapLevel > 0 ) )
				LabelTo( from, "(trapped)", 38 );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 4 ); // version

            writer.Write( (bool) m_Armed);

			writer.Write( m_TrapCreator );

			writer.Write( (int) m_TrapLevel );

			writer.Write( (int) m_TrapPower );
			writer.Write( (int) m_TrapType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                case 4:
                {
                    m_Armed = reader.ReadBool();
                    goto case 3;
                }
				case 3:
				{
					m_TrapCreator = reader.ReadMobile();
					goto case 2;
				}
				case 2:
				{
					m_TrapLevel = reader.ReadInt();
					goto case 1;
				}
				case 1:
				{
					m_TrapPower = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_TrapType = (TrapType)reader.ReadInt();
					break;
				}
			}
		}
	}
}