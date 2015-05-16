#region References
using System;
using System.Globalization;

using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
#endregion

namespace Server.Items
{
	public class CharacterStatuePlinth : Static, IAddon
	{
		public Item Deed { get { return new CharacterStatueDeed(m_Statue); } }
		public override int LabelNumber { get { return 1076201; } } // Character Statue

		private CharacterStatue m_Statue;

		public CharacterStatuePlinth(CharacterStatue statue)
			: base(0x32F2)
		{
			m_Statue = statue;

			InvalidateHue();
		}

		public CharacterStatuePlinth(Serial serial)
			: base(serial)
		{ }

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if (m_Statue == null)
			{
				return;
			}

			m_Statue.Delete();
			m_Statue = null;
		}

		public override void OnMapChange()
		{
			if (m_Statue != null)
			{
				m_Statue.Map = Map;
			}
		}

		public override void OnLocationChange(Point3D oldLocation)
		{
			if (m_Statue != null)
			{
				m_Statue.Location = new Point3D(X, Y, Z + 5);
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (m_Statue != null)
			{
				from.SendGump(new CharacterPlinthGump(m_Statue));
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version

			writer.Write(m_Statue);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadEncodedInt();

			m_Statue = reader.ReadMobile<CharacterStatue>();

			if (m_Statue == null || m_Statue.SculptedBy == null || Map == Map.Internal)
			{
				Timer.DelayCall(TimeSpan.Zero, Delete);
			}
		}

		public void InvalidateHue()
		{
			if (m_Statue != null)
			{
                switch (m_Statue.StatueType)
                {
                    case StatueType.Minax:
                        {
                            Hue = 1645;
                            break;
                        }
                    case StatueType.TB:
                        {
                            Hue = 2214;
                            break;
                        }
                    case StatueType.CoM:
                        {
                            Hue = 1325;
                            break;
                        }
                    case StatueType.SL:
                        {
                            Hue = 1109;
                            break;
                        }
                }
			}
		}

		public virtual bool CouldFit(IPoint3D p, Map map)
		{
			if (p == null || map == null || map == Map.Internal)
			{
				return false;
			}

			var point = new Point3D(p.X, p.Y, p.Z);

			if (!map.CanFit(point, 20))
			{
				return false;
			}

			BaseHouse house = BaseHouse.FindHouseAt(point, map, 20);

			if (house == null)
			{
				return false;
			}

			AddonFitResult result = CharacterStatueTarget.CheckDoors(point, 20, house);

			if (result == AddonFitResult.Valid)
			{
				return true;
			}

			return false;
		}

		private class CharacterPlinthGump : Gump
		{
			public CharacterPlinthGump(CharacterStatue statue)
				: base(60, 30)
			{
				Closable = true;
				Disposable = true;
				Dragable = true;
				Resizable = false;

				AddPage(0);
				AddImage(0, 0, 0x24F4);
				AddHtml(55, 50, 150, 20, statue.Name, false, false);
				AddHtml(55, 75, 150, 20, statue.SculptedOn.ToString(CultureInfo.InvariantCulture), false, false);
			}
		}
	}
}