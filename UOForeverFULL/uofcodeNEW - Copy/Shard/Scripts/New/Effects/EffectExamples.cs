#region References
using VitaNex.FX;
using VitaNex.Network;
#endregion

namespace Server.FX
{
	public static class EffectExamples
	{
		public static void ExampleQueuedFlamestrikes(IPoint3D source, IPoint3D target, Map map)
		{
			//Create new standard queue.
			//Deferred option is true by default, meaning all effects will be processed sequentially.
			//If it was false, all effects would be processed at the same time, regardless of any delays.
			EffectQueue queue = new EffectQueue();

			//Attach an optional effect handler.
			//This callback is fired for every effect in the queue when they are sent.
			queue.Handler = effect =>
			{
				//It's handy for using the effect object to reference the location of 
				//the effect and other values needed to do stuff.
			};

			//Attach an optional callback handler.
			//This callback is fired when all effects in the queue have been processed and all delays have elapsed.
			queue.Callback = () =>
			{
				//Handy for making sure an action only happens after the entire queue is done processing.
			};

			//Generate a line of points (aka a 'path')
			var line = source.GetLine3D(target, map);

			//Build the queue based on the points in the line.
			foreach (Point3D p in line)
			{
				queue.Add(
					new EffectInfo(
						p,
						map,
						14089,
						0,
						10,
						30,
						EffectRender.Normal,
						null,
						() =>
						{
							//Callback fired after effect is sent and any delay has elapsed.
							//I like to use this for sound effects, or maybe gather mobs to damage.
							//It's fired pretty much at the same time as the queue.Handler callback.
						}));
			}

			//Starts processing the queue and sending the effects!
			queue.Process();
		}
	}
}