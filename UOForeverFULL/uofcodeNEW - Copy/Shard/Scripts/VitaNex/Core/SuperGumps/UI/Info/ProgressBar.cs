#region Header
//   Vorspire    _,-'/-'/  ProgressBar.cs
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
using System.Drawing;

using Server;
using Server.Factions;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public enum ProgressBarFlow
	{
		Up = Direction.Up,
		UpRight = Direction.North,
		Right = Direction.Right,
		DownRight = Direction.East,
		Down = Direction.Down,
		DownLeft = Direction.South,
		Left = Direction.Left,
		UpLeft = Direction.West
	}

	public class ProgressBarGump : SuperGump
	{
		public static int DefaultWidth = 210;
		public static int DefaultHeight = 25;
		public static int DefaultPadding = 5;
		public static int DefaultBackgroundID = 9272;
		public static int DefaultForegroundID = 1464;
		public static string DefaultText = "Progress";
		public static ProgressBarFlow DefaultFlow = ProgressBarFlow.Right;

		private double? _InitValue, _InitMaxValue;

		private double _Value;
		private double _MaxValue;

		public int Width { get; set; }
		public int Height { get; set; }
		public int Padding { get; set; }

		public int BackgroundID { get; set; }
		public int ForegroundID { get; set; }

		public Color TextColor { get; set; }
		public string Text { get; set; }

		public bool DisplayPercent { get; set; }
		public ProgressBarFlow Flow { get; set; }
		public Action<ProgressBarGump, double> ValueChanged { get; set; }

		public double MaxValue
		{
			get { return _MaxValue; }
			set
			{
				_MaxValue = value;

				if (_InitMaxValue == null)
				{
					_InitMaxValue = _MaxValue;
				}
			}
		}

		public double Value
		{
			get { return _Value; }
			set
			{
				if (_Value == value)
				{
					return;
				}

				double oldValue = _Value;

				InternalValue = Math.Min(_MaxValue, value);

				OnValueChanged(oldValue);
			}
		}

		public double InternalValue
		{
			get { return Value; }
			set
			{
				if (_Value == value)
				{
					return;
				}

				_Value = Math.Min(_MaxValue, value);

				if (_InitValue == null)
				{
					_InitValue = _Value;
				}
			}
		}

		public double PercentComplete
		{
			get
			{
				if (_Value == _MaxValue)
				{
					return 1.0;
				}

				if (_MaxValue != 0)
				{
					return _Value / _MaxValue;
				}

				return 1.0;
			}
		}

		public bool Completed { get { return PercentComplete >= 1.0; } }

        public FactionObelisk Obelisk { get; set; }

	    public ProgressBarGump(
	        PlayerMobile user,
	        FactionObelisk obelisk)
	        : this(user, null, null, null)
	    {
	        Obelisk = obelisk;
	    }

		public ProgressBarGump(
			PlayerMobile user,
			Gump parent = null,
			int? x = null,
			int? y = null)
			: base(user, parent, x, y)
		{
			Padding = DefaultPadding;

			ForceRecompile = true;
		}

		public virtual void Reset()
		{
			_Value = _InitValue ?? 0;
			_MaxValue = _InitMaxValue ?? 100;

			_InitValue = _InitMaxValue = null;
		}

		protected override void Compile()
		{
			if (Padding < 0)
			{
				Padding = DefaultPadding;
			}

			base.Compile();
		}

		protected virtual void OnValueChanged(double oldValue)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, oldValue);
			}

			Refresh(true);
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			int xyPadding = Padding;

			layout.Add(
				"background/body/base",
				() =>
				{
					AddBackground(0, 0, 244, 60, 9270);

                    AddBackground(10, 10, 225, 40, 9350);

					//AddTooltip(Completed ? 1049071 : 1049070);
				});

			layout.Add(
				"imagetiled/body/visual",
				() =>
				{
				    int x = xyPadding + 8, y = xyPadding + 9;

                    for (int i = 0; i < Obelisk.CurrentCharge; i++)
					    {
					        AddImage(x + (7*i), y, 255, Obelisk.OwnerHue-1);
                            if (Obelisk.FullyControlled)
                                AddImage(x + (7 * 29), y, 255, Obelisk.OwnerHue - 1);
					    }
				});
		}
	}
}