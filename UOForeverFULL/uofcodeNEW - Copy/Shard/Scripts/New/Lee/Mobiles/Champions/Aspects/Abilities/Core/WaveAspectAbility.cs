#region References
using VitaNex.FX;
#endregion

namespace Server.Mobiles
{
	public abstract class WaveAspectAbility : AspectAbility
	{
		protected abstract BaseWaveEffect CreateEffect(BaseAspect aspect);

		protected override void OnInvoke(BaseAspect aspect)
		{
			BaseWaveEffect fx = CreateEffect(aspect);

			if (fx == null)
			{
				return;
			}

			fx.EffectHandler = e =>
			{
				if (e.ProcessIndex != 0)
				{
					return;
				}

				foreach (Mobile t in AcquireTargets<Mobile>(aspect, e.Source.Location, 0))
				{
					OnTargeted(aspect, t);
				}
			};

			fx.Send();
		}

		protected virtual void OnTargeted(BaseAspect aspect, Mobile target)
		{
			Damage(aspect, target);
		}
	}
}