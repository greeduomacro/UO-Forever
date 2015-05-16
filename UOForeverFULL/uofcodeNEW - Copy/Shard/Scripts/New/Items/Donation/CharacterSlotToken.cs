using System;
using Server.Items;
using Server.Network;
using Server.Accounting;

namespace Server.Items
{
        public class CharacterSlotToken : Item
        {
                public static int MaxCharacters = 7;

                [Constructable]
                public CharacterSlotToken() : base( 0x2AAA )
                {
                        Name = "a character slot token";
                        LootType = LootType.Blessed;
                        Light = LightType.Circle300;
                        Weight = 5.0;
                }

                public CharacterSlotToken( Serial serial ) : base( serial )
                {
                }

                public override void GetProperties( ObjectPropertyList list )
                {
                        base.GetProperties( list );

                        list.Add( 1070998, "extra character slot" ); // Use this to redeem<br>your ~1_PROMO~
                }

                public override void OnDoubleClick( Mobile from )
                {
                        if( !IsChildOf( from.Backpack ) )
                        {
                                from.SendLocalizedMessage( 1062334 ); // This item must be in your backpack to be used.
                        }
                        else
                        {
                                Account acct = from.Account as Account;
                                int chars = Convert.ToInt32( acct.GetTag("maxChars") );

                                if ( chars < 3 )
                                        chars = 3;
                                from.SendMessage(" Chars: " + chars);

                                if ( chars > 7 )
                                        chars = 7;

                                if ( chars < MaxCharacters )
                                {
                                    chars++;
                                        if ( chars == 4 )
                                        {
                                                acct.SetTag( "maxChars", "4" );
                                                from.SendMessage( "You have unlocked your 4th characters slot." );
                                        }

                                        if ( chars == 5 )
                                        {
                                                acct.SetTag( "maxChars", "5" );
                                                from.SendMessage( "You have unlocked your 5th characters slot." );
                                                from.SendMessage( 38, "You now have the max amount of character slots possible." );
                                        }
                                        if ( chars == 6 )
                                        {
                                                acct.SetTag( "maxChars", "6" );
                                                from.SendMessage( "You have unlocked your 6th characters slot." );
                                        }

                                        if ( chars == 7 )
                                        {
                                                acct.SetTag( "maxChars", "7" );
                                                from.SendMessage( "You have unlocked your 7th characters slot." );
                                                from.SendMessage( 38, "You now have the max amount of character slots possible." );
                                        }


                                        this.Delete();
                                }
                                else
                                {
                                        from.SendMessage( "You cannot increase your accounts characters slots any further." );
                                }
                        }
                }

                public override void Serialize( GenericWriter writer )
                {
                        base.Serialize( writer );

                        writer.Write( (int) 0 ); // version
                }

                public override void Deserialize( GenericReader reader )
                {
                        base.Deserialize( reader );

                        int version = reader.ReadInt();
                }
        }
}