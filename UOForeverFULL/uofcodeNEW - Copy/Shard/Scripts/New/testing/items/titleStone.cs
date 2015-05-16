//Original Script By boba fett
using System; 
using System.IO; 
using System.Collections; 
using Server.Items;
using Server.Misc; 
using Server.Gumps;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Accounting;


namespace Server.Items 
{ 
public class TitleStone : Item 
{ 
[Constructable] 
public TitleStone() : base( 0xED4 ) 
{ 
Movable = false; 
Hue = 0; 
Name = "Title Stone";
} 

public override void OnDoubleClick( Mobile from )
{
if ( from.Mounted )
{
from.SendLocalizedMessage( 1042146 );
}
else
{
from.SendGump(new ClassicRaceGump(from, ClassicRacePage.Info) );
}
}

public TitleStone( Serial serial ) : base( serial ) 
{ 
} 

public override void Serialize( GenericWriter writer ) 
{ 
base.Serialize( writer ); 

writer.Write( (int) 0 );  
} 

public override void Deserialize( GenericReader reader ) 
{ 
base.Deserialize( reader ); 

int version = reader.ReadInt(); 
} 
} 

public enum ClassicRacePage 
{ 
Info, 
Collector, 
Trainer, 
Murderer, 
Orc, 
Deamons, 
Gargoyles,
} 

public class ClassicRaceGump : Gump 
{ 
private Mobile m_From; 
private ClassicRacePage m_Page; 

private const int Blanco = 0xFFFFFF; 
private const int Azul = 0x8080FF; 

public void AddPageButton( int x, int y, int buttonID, string text, ClassicRacePage page, params ClassicRacePage[] subpage ) 
{ 
bool seleccionado = ( m_Page == page ); 

for ( int i = 0; !seleccionado && i < subpage.Length; ++i ) 
seleccionado = ( m_Page == subpage[i] ); 

AddButton( x, y - 1, seleccionado ? 4006 : 4005, 4007, buttonID, GumpButtonType.Reply, 0 ); 
AddHtml( x + 35, y, 200, 20, Color( text, seleccionado ? Azul : Blanco ), false, false ); 
} 

public void AddButtonLabeled( int x, int y, int buttonID, string text ) 
{ 
AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 ); 
AddHtml( x + 35, y, 240, 20, Color( text, Blanco ), false, false ); 
} 

public int GetButtonID( int type, int index ) 
{ 
return 1 + (index * 15) + type; 
} 

public string Color( string text, int color ) 
{ 
return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text ); 
} 

public ClassicRaceGump ( Mobile from, ClassicRacePage page) : base ( 40, 45 ) 
{ 
from.CloseGump( typeof( ClassicRaceGump ) ); 

m_From = from; 
m_Page = page; 
Closable = true; 
Dragable = true; 
from.Frozen = false; 

AddPage( 0 ); 
AddBackground( 0, 0, 531, 360, 5054 ); 
AddAlphaRegion( 10, 10, 511, 340 ); 

 
AddImageTiled( 11, 37, 149, 19, 0x52 ); 
AddImageTiled( 11, 37, 148, 18, 0xBBC ); 
AddLabel( 35, 37, 0, "Choose Your title" );  

 
AddImageTiled( 11, 11, 509, 21, 0x52 ); 
AddImageTiled( 11, 11, 507, 19, 0xBBC ); 
AddLabel( 165, 11, 0, "Choose your destiny" );
 
AddImageTiled( 10, 32, 511, 5, 5058 ); 
AddImageTiled( 10, 287, 511, 5, 5058 ); 
AddImageTiled( 160, 37, 5, 250, 5058 ); 

AddPageButton( 40, 75, GetButtonID( 0, 0 ), "Info", ClassicRacePage.Info );  
AddPageButton( 40, 110, GetButtonID( 0, 1 ), "Collector", ClassicRacePage.Collector);  
AddPageButton( 40, 135, GetButtonID( 0, 2 ), "Trainer", ClassicRacePage.Trainer);  
AddPageButton( 40, 160, GetButtonID( 0, 3 ), "Murderer", ClassicRacePage.Murderer);  
AddPageButton( 40, 185, GetButtonID( 0, 4 ), "Orc", ClassicRacePage.Orc);  
AddPageButton( 40, 210, GetButtonID( 0, 5 ), "Deamons", ClassicRacePage.Deamons);  
AddPageButton( 40, 235, GetButtonID( 0, 6 ), "Gargoyles", ClassicRacePage.Gargoyles); 
	AddButton( 490, 11, 0xFB1, 0xFB3, 7, GumpButtonType.Reply, 0 ); 
	AddLabel( 450, 11, 0x34, "Close" ); 


switch ( page ) 
{ 
case ClassicRacePage.Info: 
{ 
AddHtml( 172, 40, 337, 245, String.Format("Perilous"), true, false );  
break; 
} 

case ClassicRacePage.Collector: 
{ 
AddHtml( 172, 40, 337, 245, String.Format("<center>* Collector *</center>\nThe Collectors are out for rares.\n"), true, true );
AddPageButton( 50, 317, GetButtonID( 1, 0 ), "Back To List of titles/Classes", ClassicRacePage.Info); 
AddButtonLabeled( 300, 317, GetButtonID( 1, 1 ), "I want To be an Collector" );  
break; 
} 

case ClassicRacePage.Trainer: 
{ 
AddHtml( 172, 40, 337, 245, String.Format("<center>* Trainer *</center>\nTrainers work hard on skills for them and there pets .\n"), true, true ); 
AddPageButton( 50, 317, GetButtonID( 1, 0 ), "Back To The List of titles/Classes", ClassicRacePage.Info);  
AddButtonLabeled( 300, 317, GetButtonID( 1, 2 ), "I want to be a Trainer." ); 
break; 
} 

case ClassicRacePage.Murderer: 
{ 
AddHtml( 172, 40, 337, 245, String.Format("<center>* Murderer *</center>\nThe Murderers are vial creatures who roam the earth to kill the weak...\n "), true, true );  
AddPageButton( 50, 317, GetButtonID( 1, 0 ), "Back To the List of titles/Classes", ClassicRacePage.Info); 
AddButtonLabeled( 300, 317, GetButtonID( 1, 3 ), "I Want To Be A Murderer." );  
break; 
} 

case ClassicRacePage.Orc: 
{ 
AddHtml( 172, 40, 337, 245, String.Format("<center>* Orc *</center>\nOrcs are a race of there own with there own language, color, and camps...\n "), true, true );  
AddPageButton( 50, 317, GetButtonID( 1, 0 ), "Back To The List of titles/Classes", ClassicRacePage.Info); 
AddButtonLabeled( 300, 317, GetButtonID( 1, 4 ), "I Want To Be A Orc" );  
break; 
} 

case ClassicRacePage.Deamons: 
{ 
AddHtml( 172, 40, 337, 245, String.Format("<center>* Deamons *</center>\nTo be a Deamons is to be at ease with death. Deamons are pure evil all that exist between them and hell is life. With no fear to cross the barrier they are almost defeatless... and very erie, the discoverers of astronomy and the zodiacs... They combine the art of natural force to suck your soul... beware... holy light does sufficient damage.\n"), true, true );  
AddPageButton( 50, 317, GetButtonID( 1, 0 ), "Back To The List of titles/Classes", ClassicRacePage.Info); 
AddButtonLabeled( 300, 317, GetButtonID( 1, 5 ), "I Want To Be A Deamons" );  
break; 
} 

case ClassicRacePage.Gargoyles: 
{ 
AddHtml( 172, 40, 337, 245, String.Format("<center>* Gargoyles *</center>\nGargoyless are the scum of the earth and cares about only themselves hunting humans in all forms ...\n"), true, true ); 
AddPageButton( 50, 317, GetButtonID( 1, 0 ), "Back To The List of titles/Classes", ClassicRacePage.Info);  
AddButtonLabeled( 300, 317, GetButtonID( 1, 6 ), "I Want To Be A Gargoyles" ); 
break; 
} 



} 

} 

public override void OnResponse( Server.Network.NetState sender, RelayInfo info ) 
{ 
int val = info.ButtonID - 1; 

if ( val < 0 ) 
return; 

Mobile from = m_From; 
from.Frozen = false; 

int type = val % 15; 
int index = val / 15; 

switch ( type ) 
{ 
case 0: 
{ 
ClassicRacePage page; 

switch ( index ) 
{ 
case 0: page = ClassicRacePage.Info; break; 
case 1: page = ClassicRacePage.Collector; break; 
case 2: page = ClassicRacePage.Trainer; break; 
case 3: page = ClassicRacePage.Murderer; break; 
case 4: page = ClassicRacePage.Orc; break; 
case 5: page = ClassicRacePage.Deamons; break; 
case 6: page = ClassicRacePage.Gargoyles; break; 
default: return; 
} 

from.SendGump( new ClassicRaceGump( from, page) ); 
break; 
} 

case 1: 
{ 
switch ( index ) 
{ 
case 0: 
{ 
from.SendGump( new ClassicRaceGump( from, ClassicRacePage.Info) ); 
break; 
} 

case 1: 
{ 
m_From.SendMessage( "you are now an Collector" ); 
m_From.Hue = 1130;
m_From.Title = "the Collector"; 
m_From.Location = new Point3D( 1496, 1628, 10 );
break; 
} 

case 2: 
{ 
m_From.SendMessage( "you are now a Trainer" ); 
m_From.Hue = 1201; 
m_From.Title = "the Trainer";
m_From.Location = new Point3D( 1496, 1628, 10 );
break; 
} 

case 3: 
{ 
m_From.SendMessage( "you are now a Murderer" ); 
m_From.Hue = 900;
m_From.Title = "the Murderer";
m_From.Location = new Point3D( 1496, 1628, 10 );
break; 
} 

case 4: 
{ 
m_From.SendMessage( "you are now a Orc" ); 
m_From.Hue = 1002; 
m_From.Title = "the Orc";
m_From.Location = new Point3D( 1496, 1628, 10 );
break; 
} 

case 5: 
{ 
m_From.SendMessage( "you are now a Deamon" ); 
m_From.Hue = 1104; 
m_From.Title = "the Deamons";
m_From.Location = new Point3D( 1496, 1628, 10 );
break; 
} 

case 6: 
{ 
m_From.SendMessage( "you are now a Gargoyle" ); 
m_From.Hue = 1007; 
m_From.Title = "the Gargoyles";
m_From.Location = new Point3D( 1496, 1628, 10 ); 
break; 
} 


} 
break; 
} 

} 

} 
} 
}