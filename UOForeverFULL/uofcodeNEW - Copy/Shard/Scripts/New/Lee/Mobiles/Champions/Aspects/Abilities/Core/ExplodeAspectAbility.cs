using VitaNex.FX;

namespace Server.Mobiles
{
	public abstract class ExplodeAspectAbility : AspectAbility
	{
		protected abstract BaseExplodeEffect CreateEffect(BaseAspect aspect);

		protected override void OnInvoke(BaseAspect aspect)
		{
			BaseExplodeEffect fx = CreateEffect(aspect);

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

				foreach (var t in AcquireTargets<Mobile>(aspect, e.Source.Location, 0))
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