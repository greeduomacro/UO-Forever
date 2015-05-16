using System;
using System.Collections;
using Server;

namespace Server.Commands.Generic
{
	public class GlobalCommandImplementor : BaseCommandImplementor
	{
		public GlobalCommandImplementor()
		{
			Accessors = new string[]{ "Global" };
			SupportRequirement = CommandSupport.Global;
			SupportsConditionals = true;
			AccessLevel = AccessLevel.Administrator;
			Usage = "Global <command> [condition]";
			Description = "Invokes the command on all appropriate objects in the world. Optional condition arguments can further restrict the set of objects.";
		}

		public override void Compile( Mobile from, BaseCommand command, ref string[] args, ref object obj )
		{
			try
			{
				if (LoggingCustom.CommandDebug)
					LoggingCustom.LogCommandDebug("Global...compiling\t");
                Extensions ext = Extensions.Parse( from, ref args );

				bool items, mobiles;
                if (LoggingCustom.CommandDebug) LoggingCustom.LogCommandDebug( "1\t");
				if ( !CheckObjectTypes( from, command, ext, out items, out mobiles ) )
					return;

				ArrayList list = new ArrayList();

				if ( items )
				{
                    if (LoggingCustom.CommandDebug) LoggingCustom.LogCommandDebug( "2\t");
                    foreach ( Item item in World.Items.Values )
					{
						if ( ext.IsValid( item ) )
							list.Add( item );
					}
				}

				if ( mobiles )
				{
                    if (LoggingCustom.CommandDebug) LoggingCustom.LogCommandDebug( "3\t");
                    foreach ( Mobile mob in World.Mobiles.Values )
					{
						if ( ext.IsValid( mob ) )
							list.Add( mob );
					}
				}
                if (LoggingCustom.CommandDebug) LoggingCustom.LogCommandDebug( "startfilter:list with " + list.Count+ " in it\t");
				ext.Filter( list );
                if (LoggingCustom.CommandDebug) LoggingCustom.LogCommandDebug( "4\t");
				obj = list;
			}
			catch ( Exception ex )
			{
				from.SendMessage( ex.Message );
			}
		}
	}
}