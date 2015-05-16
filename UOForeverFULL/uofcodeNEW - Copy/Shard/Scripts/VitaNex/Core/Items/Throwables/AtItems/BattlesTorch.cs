#region Header
//   Vorspire    _,-'/-'/  Axe.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Items;
using Server.Network;
using Server.Targeting;
using VitaNex.FX;
using VitaNex.Network;

#endregion

namespace VitaNex.Items
{
	public class ThrowableBattlesTorch : BaseThrowable<Item>
	{
        #region Launch Effect
        public virtual int DefLaunchID { get { return ItemID; } }
        public virtual int DefLaunchHue { get { return Hue; } }
        public virtual int DefLaunchSpeed { get { return 8; } }
        public virtual EffectRender DefLaunchRender { get { return EffectRender.Normal; } }
        public virtual int DefLaunchSound { get { return 551; } }
        public virtual int DefLaunchRangeMin { get { return 1; } }
        public virtual int DefLaunchRangeMax { get { return 3; } }
        public virtual int DefLaunchHeightMin { get { return 70; } }
        public virtual int DefLaunchHeightMax { get { return 80; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LaunchID { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LaunchHue { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LaunchSpeed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public EffectRender LaunchRender { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LaunchSound { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LaunchHeightMin { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LaunchHeightMax { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LaunchRangeMin { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LaunchRangeMax { get; set; }
        #endregion

        #region Trail Effect
        public virtual int DefTrailID { get { return 14120; } }
        public virtual int DefTrailHue { get { return 0; } }
        public virtual int DefTrailSpeed { get { return 10; } }
        public virtual int DefTrailDuration { get { return 10; } }
        public virtual EffectRender DefTrailRender { get { return EffectRender.SemiTransparent; } }
        public virtual int DefTrailSound { get { return 856; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrailID { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrailHue { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrailSpeed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrailDuration { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public EffectRender TrailRender { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrailSound { get; set; }
        #endregion

        #region Explode Effect
        public virtual int DefExplodeID { get { return 14000; } }
        public virtual int DefExplodeHue { get { return 0; } }
        public virtual int DefExplodeSpeed { get { return 10; } }
        public virtual int DefExplodeDuration { get { return 13; } }
        public virtual EffectRender DefExplodeRender { get { return EffectRender.LightenMore; } }
        public virtual int DefExplodeSound { get { return 776; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ExplodeID { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ExplodeHue { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ExplodeSpeed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ExplodeDuration { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public EffectRender ExplodeRender { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ExplodeSound { get; set; }
        #endregion

        #region Stars Effect
        public virtual FireworkStars DefStarsEffect { get { return FireworkStars.Peony; } }
        public virtual int DefStarsHue { get { return 0; } }
        public virtual int DefStarsSound { get { return 776; } }
        public virtual int DefStarsRangeMin { get { return 5; } }
        public virtual int DefStarsRangeMax { get { return 10; } }
        public virtual int[] DefStars { get { return new[] { 14170, 14155, 14138, 10980, 10296, 10297, 10298, 10299, 10300, 10301 }; } }
        public virtual int[] DefStarHues { get { return new int[0]; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public FireworkStars StarsEffect { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StarsHue { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StarsSound { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StarsRangeMin { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StarsRangeMax { get; set; }

        public List<int> Stars { get; set; }
        public List<int> StarHues { get; set; }
        #endregion

		[Constructable]
		public ThrowableBattlesTorch()
			: this(1)
		{ }

		[Constructable]
		public ThrowableBattlesTorch(int amount)
            : base(2580, amount)
		{
			Name = "battle torch";
			Usage = "When thrown, light battle braziers on fire.";

			Weight = 0.0;
			Stackable = true;
			Consumable = false;

		    Hue = 1161;

            EffectSpeed = 1;

			ThrowSound = 513;
			ImpactSound = 1310;

            EffectID = 2580;
			EffectHue = 1160;

			ThrowRecovery = TimeSpan.FromSeconds(3.0);

			RequiredSkillValue = 0.0;

            LaunchID = DefLaunchID;
            LaunchHue = DefLaunchHue;
            LaunchSpeed = DefLaunchSpeed;
            LaunchRender = DefLaunchRender;
            LaunchSound = DefLaunchSound;
            LaunchHeightMin = DefLaunchHeightMin;
            LaunchHeightMax = DefLaunchHeightMax;
            LaunchRangeMin = DefLaunchRangeMin;
            LaunchRangeMax = DefLaunchRangeMax;

            TrailID = DefTrailID;
            TrailHue = DefTrailHue;
            TrailSpeed = DefTrailSpeed;
            TrailDuration = DefTrailDuration;
            TrailRender = DefTrailRender;
            TrailSound = DefTrailSound;

            ExplodeID = DefExplodeID;
            ExplodeHue = DefExplodeHue;
            ExplodeSpeed = DefExplodeSpeed;
            ExplodeDuration = DefExplodeDuration;
            ExplodeRender = DefExplodeRender;
            ExplodeSound = DefExplodeSound;

            StarsEffect = DefStarsEffect;
            StarsHue = DefStarsHue;
            StarsSound = DefStarsSound;
            StarsRangeMin = DefStarsRangeMin;
            StarsRangeMax = DefStarsRangeMax;
            Stars = new List<int>(DefStars);
            StarHues = new List<int> { 38, 1153, 5 };
		}

        public ThrowableBattlesTorch(Serial serial)
			: base(serial)
		{ }

        protected override void OnThrownAt(Mobile m, Item target)
        {
            if (m != null && target != null)
            {
                var count = 0;
                var braziers = target.GetItemsInRange<Item>(10).Where(x => x.ItemID == 3633);
                foreach (var brazier in braziers)
                {
                    Item brazier1 = brazier;
                    Timer.DelayCall(TimeSpan.FromSeconds(5 + count),() => Launch(m, brazier1));
                    var battlefire = new BattlesFire();
                    battlefire.MoveToWorld(new Point3D(brazier.X, brazier.Y, brazier.Z+7), brazier.Map);
                    count += 5;
                }
                Effects.SendFlashEffect(m, (FlashType)2);
            }

            base.OnThrownAt(m, target);
        }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
                        writer.Write(LaunchID);
                        writer.Write(LaunchHue);
                        writer.Write(LaunchSpeed);
                        writer.WriteFlag(LaunchRender);
                        writer.Write(LaunchSound);
                        writer.Write(LaunchRangeMin);
                        writer.Write(LaunchRangeMax);
                        writer.Write(LaunchHeightMin);
                        writer.Write(LaunchHeightMax);

                        writer.Write(TrailID);
                        writer.Write(TrailHue);
                        writer.Write(TrailSpeed);
                        writer.Write(TrailDuration);
                        writer.WriteFlag(TrailRender);
                        writer.Write(TrailSound);

                        writer.Write(ExplodeID);
                        writer.Write(ExplodeHue);
                        writer.Write(ExplodeSpeed);
                        writer.Write(ExplodeDuration);
                        writer.WriteFlag(ExplodeRender);
                        writer.Write(ExplodeSound);

                        writer.WriteFlag(StarsEffect);
                        writer.Write(StarsHue);
                        writer.Write(StarsSound);
                        writer.Write(StarsRangeMin);
                        writer.Write(StarsRangeMax);
                        writer.WriteList(Stars, writer.Write);
                        writer.WriteList(StarHues, writer.Write);
					}
					break;
			}
		}

        protected virtual Point3D[] GetPath(Item brazier)
        {
            var start = brazier.GetWorldLocation();
            var end = start.GetAllPointsInRange(Map, LaunchRangeMin, LaunchRangeMax, false)
                           .GetRandom()
                           .Clone3D(0, 0, Utility.RandomMinMax(LaunchHeightMin, LaunchHeightMax));

            return start.GetLine3D(end, Map, false);
        }

        protected bool Launch(Mobile m, Item brazier)
        {
            if (m == null || m.Deleted)
            {
                return false;
            }

            var path = GetPath(brazier);

            if (path == null || path.Length == 0)
            {
                return false;
            }

            Point3D start = path.First();
            Point3D end = path.Last();


            MovingEffectQueue q = new MovingEffectQueue(() => Explode(m, end));

            path.For(
                (i, p) =>
                {
                    if (i + 1 < path.Length)
                    {
                        q.Add(
                            new MovingEffectInfo(
                                p,
                                path[i + 1],
                                Map,
                                LaunchID,
                                Hue - 1,
                                LaunchSpeed,
                                LaunchRender,
                                TimeSpan.Zero,
                                () => LaunchTrail(path[i + 1])));
                    }
                });

            q.Process();

            if (LaunchSound > 0)
            {
                Effects.PlaySound(GetWorldLocation(), Map, LaunchSound);
            }

            return true;
        }

        protected void LaunchTrail(Point3D p)
        {
            if (TrailID > 0)
            {
                var fx = new EffectInfo(p, Map, TrailID, TrailHue, TrailSpeed, TrailDuration, TrailRender);

                fx.Send();
            }

            if (TrailSound > 0)
            {
                Effects.PlaySound(p, Map, TrailSound);
            }
        }

        protected void Explode(Mobile m, Point3D p)
        {
            if (m == null || m.Deleted)
            {
                return;
            }

            if (ExplodeID > 0)
            {
                var fx = new EffectInfo(p, Map, ExplodeID, ExplodeHue, ExplodeSpeed, ExplodeDuration, ExplodeRender);

                fx.Send();
            }

            if (ExplodeSound >= 0)
            {
                Effects.PlaySound(p, Map, ExplodeSound);
            }

            ExplodeStars(
                m,
                StarsEffect,
                p,
                Utility.RandomMinMax(StarsRangeMin, StarsRangeMax),
                StarsSound,
                Stars.ToArray(),
                StarsHue > 0 ? new[] { StarsHue } : StarHues.ToArray());
        }

        protected virtual void ExplodeStars(
    Mobile m, FireworkStars fx, Point3D p, int radius, int sound, int[] stars, int[] hues)
        {
            if (m != null && !m.Deleted && fx != FireworkStars.None && stars != null && stars.Length != 0 && !Deleted)
            {
                fx.DoStarsEffect(p, Map, radius, sound, stars, hues);
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
                        LaunchID = reader.ReadInt();
                        LaunchHue = reader.ReadInt();
                        LaunchSpeed = reader.ReadInt();
                        LaunchRender = reader.ReadFlag<EffectRender>();
                        LaunchSound = reader.ReadInt();
                        LaunchRangeMin = reader.ReadInt();
                        LaunchRangeMax = reader.ReadInt();
                        LaunchHeightMin = reader.ReadInt();
                        LaunchHeightMax = reader.ReadInt();

                        TrailID = reader.ReadInt();
                        TrailHue = reader.ReadInt();
                        TrailSpeed = reader.ReadInt();
                        TrailDuration = reader.ReadInt();
                        TrailRender = reader.ReadFlag<EffectRender>();
                        TrailSound = reader.ReadInt();

                        ExplodeID = reader.ReadInt();
                        ExplodeHue = reader.ReadInt();
                        ExplodeSpeed = reader.ReadInt();
                        ExplodeDuration = reader.ReadInt();
                        ExplodeRender = reader.ReadFlag<EffectRender>();
                        ExplodeSound = reader.ReadInt();

                        StarsEffect = reader.ReadFlag<FireworkStars>();
                        StarsHue = reader.ReadInt();
                        StarsSound = reader.ReadInt();
                        StarsRangeMin = reader.ReadInt();
                        StarsRangeMax = reader.ReadInt();
                        Stars = reader.ReadList(reader.ReadInt);
                        StarHues = reader.ReadList(reader.ReadInt);
					}
					break;
			}
		}
	}
}