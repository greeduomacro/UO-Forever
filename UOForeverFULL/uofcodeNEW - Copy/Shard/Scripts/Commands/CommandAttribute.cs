using System;
using System.Reflection;
using System.Diagnostics;
using Server.Commands;

namespace Server
{
    [AttributeUsage( AttributeTargets.Method )]
    public class CommandAttribute : Attribute
    {
        public static void Initialize()
        {
            Console.Write( "Commands: Loading... " );
            int count = 0;
            Stopwatch sw = new Stopwatch();

            sw.Start();

            foreach ( Assembly asm in ScriptCompiler.Assemblies )
            {
                foreach ( Type t in asm.GetTypes() )
                {
                    foreach ( MethodInfo mi in t.GetMethods() )
                    {
                        ParameterInfo[] parameters = mi.GetParameters();
                        if ( parameters.Length != 1 || parameters[ 0 ].ParameterType != typeof( CommandEventArgs ) )
                            continue;

                        object[] attr = mi.GetCustomAttributes( typeof( CommandAttribute ), true );

                        if ( attr.Length == 1 )
                        {
                            CommandAttribute ca = (CommandAttribute)attr[ 0 ];
                            ++count;
                            CommandSystem.Register( ca.Command, ca.Level, (CommandEventHandler)Delegate.CreateDelegate( typeof( CommandEventHandler ), mi ) );
                        }
                    }
                }
            }

            sw.Stop();

            Console.Write( "done({0} commands) ({1:F1} seconds) \n", count, sw.Elapsed.TotalSeconds );
        }

        public string Command;
        public AccessLevel Level;

        public CommandAttribute( string cmd, AccessLevel level )
        {
            Command = cmd;
            Level = level;
        }
    }
}