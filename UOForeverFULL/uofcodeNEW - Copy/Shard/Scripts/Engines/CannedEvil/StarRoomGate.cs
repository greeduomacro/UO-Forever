#region References
using System;
#endregion

namespace Server.Items
{
	public sealed class StarRoomGate : Moongate
	{
		private bool _Decays;
		private DateTime _DecayTime;
		private Timer _Timer;

		public override int LabelNumber { get { return 1049498; } } // dark moongate

		[Constructable]
		public StarRoomGate()
			: this(false)
		{ }

		[Constructable]
		public StarRoomGate(bool decays)
			: base(new Point3D(5143, 1774, 0), Map.Felucca)
		{
			Dispellable = false;
			ItemID = 0x1FD4;

			if (!decays)
			{
				return;
			}

			_Decays = true;
			_DecayTime = DateTime.UtcNow + TimeSpan.FromMinutes(2.0);

			_Timer = new InternalTimer(this, _DecayTime);
			_Timer.Start();
		}

		public StarRoomGate(Serial serial)
			: base(serial)
		{ }

		public override void OnAfterDelete()
		{
			if (_Timer != null)
			{
				_Timer.Stop();
			}

			base.OnAfterDelete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(_Decays);

			if (_Decays)
			{
				writer.WriteDeltaTime(_DecayTime);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						_Decays = reader.ReadBool();

						if (_Decays)
						{
							_DecayTime = reader.ReadDeltaTime();

							_Timer = new InternalTimer(this, _DecayTime);
							_Timer.Start();
						}
					}
					break;
			}
		}

		private sealed class InternalTimer : Timer
		{
			private readonly Item _Item;

			public InternalTimer(Item item, DateTime end)
				: base(end - DateTime.UtcNow)
			{
				_Item = item;
			}

			protected override void OnTick()
			{
				_Item.Delete();
			}
		}
	}
}