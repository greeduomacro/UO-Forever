#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Engines.Conquests
{
    public class MovementConquestContainer
    {
        private Mobile m_Mobile;
        private Direction m_Direction;
        private bool m_Blocked;

        public Mobile Mobile { get { return m_Mobile; } }
        public Direction Direction { get { return m_Direction; } }
        public bool Blocked { get { return m_Blocked; } set { m_Blocked = value; } }

        private static readonly Queue<MovementConquestContainer> m_Pool = new Queue<MovementConquestContainer>();

        public static MovementConquestContainer Create(Mobile mobile, Direction dir)
        {
            MovementConquestContainer args;

            if (m_Pool.Count > 0)
            {
                args = m_Pool.Dequeue();

                args.m_Mobile = mobile;
                args.m_Direction = dir;
                args.m_Blocked = false;
            }
            else
            {
                args = new MovementConquestContainer(mobile, dir);
            }

            return args;
        }

        public MovementConquestContainer(Mobile mobile, Direction dir)
        {
            m_Mobile = mobile;
            m_Direction = dir;
        }

        public void Free()
        {
            m_Pool.Enqueue(this);
        }
    }

	public class MoveConquest : Conquest
	{
		private static readonly Dictionary<Mobile, Direction> _DirectionCache = new Dictionary<Mobile, Direction>();

		public override string DefCategory { get { return "Movement"; } }

		public virtual string DefRegion { get { return null; } }
		public virtual Map DefMap { get { return null; } }

		public virtual bool DefSpeedChangeReset { get { return false; } }
		public virtual bool DefDirectionChangeReset { get { return false; } }
		public virtual bool DefRegionChangeReset { get { return false; } }
		public virtual bool DefMapChangeReset { get { return false; } }

		private bool _WalkOnly;
		private bool _RunOnly;

		[CommandProperty(Conquests.Access)]
		public bool WalkOnly
		{
			get { return _WalkOnly; }
			set
			{
				_WalkOnly = value;

				if (_WalkOnly && _RunOnly)
				{
					_RunOnly = false;
				}
			}
		}

		[CommandProperty(Conquests.Access)]
		public bool RunOnly
		{
			get { return _RunOnly; }
			set
			{
				_RunOnly = value;

				if (_RunOnly && _WalkOnly)
				{
					_WalkOnly = false;
				}
			}
		}

		[CommandProperty(Conquests.Access)]
		public string Region { get; set; }

		[CommandProperty(Conquests.Access)]
		public Map Map { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool SpeedChangeReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool DirectionChangeReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool RegionChangeReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool MapChangeReset { get; set; }

		public MoveConquest()
		{ }

		public MoveConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			_WalkOnly = false;
			_RunOnly = false;

			Region = DefRegion;
			Map = DefMap;

			SpeedChangeReset = DefSpeedChangeReset;
			DirectionChangeReset = DefDirectionChangeReset;
			RegionChangeReset = DefRegionChangeReset;
			MapChangeReset = DefMapChangeReset;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
            return GetProgress(state, args as MovementConquestContainer);
		}

        protected virtual int GetProgress(ConquestState state, MovementConquestContainer args)
		{
            if (state.User == null)
                return 0;

			if (args == null || args.Blocked)
			{
				return 0;
			}

			if (Map != null && Map != Map.Internal && (args.Mobile.Map == null || args.Mobile.Map != Map))
			{
				_DirectionCache.Remove(args.Mobile);

				if (MapChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			if (!String.IsNullOrWhiteSpace(Region) && (args.Mobile.Region == null || !args.Mobile.Region.IsPartOf(Region)))
			{
				_DirectionCache.Remove(args.Mobile);

				if (RegionChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			bool running = args.Direction.HasFlag(Direction.Running);

			if ((WalkOnly && running) || (RunOnly && !running))
			{
				_DirectionCache.Remove(args.Mobile);

				if (SpeedChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			if (!_DirectionCache.ContainsKey(args.Mobile))
			{
				_DirectionCache.Add(args.Mobile, args.Direction);
			}
			else if ((_DirectionCache[args.Mobile] & Direction.Running) != (args.Direction & Direction.Running))
			{
				if (DirectionChangeReset)
				{
					_DirectionCache.Remove(args.Mobile);
					return -state.Progress;
				}

				_DirectionCache[args.Mobile] = args.Direction;
			}

			return 1;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.Write(WalkOnly);
						writer.Write(RunOnly);

						writer.Write(Region);
						writer.Write(Map);

						writer.Write(SpeedChangeReset);
						writer.Write(DirectionChangeReset);
						writer.Write(RegionChangeReset);
						writer.Write(MapChangeReset);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
						WalkOnly = reader.ReadBool();
						RunOnly = reader.ReadBool();

						Region = reader.ReadString();
						Map = reader.ReadMap();

						SpeedChangeReset = reader.ReadBool();
						DirectionChangeReset = reader.ReadBool();
						RegionChangeReset = reader.ReadBool();
						MapChangeReset = reader.ReadBool();
					}
					break;
			}
		}
	}
}