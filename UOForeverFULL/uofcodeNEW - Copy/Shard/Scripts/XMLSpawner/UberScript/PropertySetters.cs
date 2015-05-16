using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Server.Mobiles;
using System.Collections;

namespace Server.Engines.XmlSpawner2
{
    class PropertySetters
    {
        public static Dictionary<string, PropertyInfo> BaseCreatureProperties = new Dictionary<string, PropertyInfo>();
        public static Dictionary<string, PropertyInfo> PlayerMobileProperties = new Dictionary<string, PropertyInfo>();
        public static Dictionary<string, PropertyInfo> ItemProperties = new Dictionary<string, PropertyInfo>();
        
        public static void Initialize()
        {
            // Initialize dictionary for the most probable classes--this will increase the speed
            // of property lookup significantly for properties connected to these 3 types
            PropertyInfo[] props = (typeof(BaseCreature)).GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo prop in props)
            {
                BaseCreatureProperties[prop.Name.ToLower()] = prop;
            }

            props = (typeof(PlayerMobile)).GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo prop in props)
            {
                PlayerMobileProperties[prop.Name.ToLower()] = prop;
            }

            props = (typeof(Item)).GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo prop in props)
            {
                ItemProperties[prop.Name.ToLower()] = prop;
            }
        }
        
        
        // These functions are borrowed heavily from ArteGordon's BaseXmlSpawner class

        // set property values with support for nested attributes
        public static void SetPropertyValue(TriggerObject trigObj, object o, string name, object value)
        {
            if (o == null)
            {
                throw new UberScriptException("Null object");
            }

            //Type ptype = null;
            object po = null;
            Type type = o.GetType();

            PropertyInfo[] props = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

            // parse the strings of the form property.attribute into two parts
            // first get the property
            List<string> arglist = PropertyGetters.PropSplitter(name);
            
            string propname = arglist[0];
            if (propname.Length == 0) throw new UberScriptException("Empty string property!");

            // Check whether they are trying to set an xml attachment through the
            // shortcut e.g. TRIGMOB().xmlints.score = TRIGMOB().xmlints.score + 1
            
            if (arglist.Count > 1 && (propname[0] == 'x' || propname[0] == 'X'))
            {
                // NEED TO HANDLE SETTING PROPERTIES ON THE ATTACHMENTS!
                string lowerPropName = propname.ToLower();
                if (lowerPropName == "xmlints")
                {
                    IEntity entity = o as IEntity;
                    if (entity == null) throw new UberScriptException("Can't set xmlints on anything but Mobile or Item");
                    // check for existing xmlValue attachment or create a new one
                    XmlValue xmlValue = XmlAttach.GetValueAttachment(entity, arglist[1]);
                    if (xmlValue == null)
                    {
                        // arglist should only have [xmlints, name], nothing more
                        if (arglist.Count > 2) throw new UberScriptException("Attempted to set property on a not-yet-existant attachment!:" + name );
                        xmlValue = new XmlValue(arglist[1], (int)value);
                        XmlAttach.AttachTo(null, entity, xmlValue);
                    }
                    else if (arglist.Count == 2)
                    {
                        if (value == null)
                        {
                            xmlValue.Delete();
                        }
                        else
                        {
                            xmlValue.Value = (int)value;
                        }
                    }
                    else if (arglist.Count > 2) 
                    {
                        // could be setting a property on an existing XmlAttachment!
                        // e.g. xmlints.test.expiration = 0:0:1
                        SetPropertyValue(trigObj, xmlValue, arglist[2], value);
                    }
                    return;
                }
                else if (lowerPropName == "xmlstrings")
                {
                    IEntity entity = o as IEntity;
                    if (entity == null) throw new UberScriptException("Can't set xmlstrings on anything but Mobile or Item");
                    XmlLocalVariable xmlLocalVariable = XmlAttach.GetStringAttachment(entity, arglist[1]);
                    if (xmlLocalVariable == null)
                    {
                        // arglist should only have [xmlints, name], nothing more
                        if (arglist.Count > 2) throw new UberScriptException("Attempted to set property on a not-yet-existant attachment!:" + name);
                        xmlLocalVariable = new XmlLocalVariable(arglist[1], (string)value);
                        XmlAttach.AttachTo(null, entity, xmlLocalVariable);
                    }
                    else if (arglist.Count == 2)
                    {
                        if (value == null)
                        {
                            xmlLocalVariable.Delete();
                        }
                        else
                        {
                            xmlLocalVariable.Data = (string)value;
                        }
                    }
                    else if (arglist.Count > 2)
                    {
                        // could be setting a property on an existing XmlAttachment!
                        // e.g. xmlints.test.expiration = 0:0:1
                        SetPropertyValue(trigObj, xmlLocalVariable, arglist[2], value);
                    }
                    return;
                }
                else if (lowerPropName == "xmldoubles")
                {
                    IEntity entity = o as IEntity;
                    if (entity == null) throw new UberScriptException("Can't set xmldoubles on anything but Mobile or Item");
                    XmlDouble xmlDouble = XmlAttach.GetDoubleAttachment(entity, arglist[1]);
                    if (xmlDouble == null)
                    {
                        // arglist should only have [xmlints, name], nothing more
                        if (arglist.Count > 2) throw new UberScriptException("Attempted to set property on a not-yet-existant attachment!:" + name);
                        xmlDouble = new XmlDouble(arglist[1], (double)value);
                        XmlAttach.AttachTo(null, entity, xmlDouble);
                    }
                    else if (arglist.Count == 2)
                    {
                        if (value == null)
                        {
                            xmlDouble.Delete();
                        }
                        else
                        {
                            xmlDouble.Value = (double)value;
                        }
                    }
                    else if (arglist.Count > 2)
                    {
                        // could be setting a property on an existing XmlAttachment!
                        // e.g. xmlints.test.expiration = 0:0:1
                        SetPropertyValue(trigObj, xmlDouble, arglist[2], value);
                    }
                    return;
                }
                else if (lowerPropName == "xmlobjs")
                {
                    IEntity entity = o as IEntity;
                    if (entity == null) throw new UberScriptException("Can't set xmlobjs on anything but Mobile or Item");
                    XmlObject xmlObject = XmlAttach.GetObjectAttachment(entity, arglist[1]);
                    if (xmlObject == null)
                    {
                        // arglist should only have [xmlints, name], nothing more
                        if (arglist.Count > 2) throw new UberScriptException("Attempted to set property on a not-yet-existant attachment!:" + name);
                        xmlObject = new XmlObject(arglist[1], value);
                        XmlAttach.AttachTo(null, entity, xmlObject);
                    }
                    else if (arglist.Count == 2)
                    {
                        if (value == null)
                        {
                            xmlObject.Delete();
                        }
                        else
                        {
                            xmlObject.Value = value;
                        }
                    }
                    else if (arglist.Count > 2)
                    {
                        // XmlObject only contains a few properties that
                        // can be accessed through statements like THIS().xmlobjs.test._____
                        // since there is a potential conflict between the developer wanting access
                        // to the properties on the object contained in the XmlObject.Value (most likely)
                        // or the properties on the XmlObject itself (far less likely)
                        string testPropName = arglist[2].ToLower();
                        // to access properties on the xmlobject itself (e.g. expiration), one must do this:
                        //  THIS().xmlobjs.test.xmlobject.expiration
                        if (testPropName == "xmlobject")
                        {
                            if (arglist.Count < 4)
                            {
                                throw new UberScriptException("Can't set an xmlobject directly, use ATTACH function!");
                            }
                            else
                            {
                                string propLookup = arglist[3]; // add this first so later additions all prepended with '.'
                                for (int i = 4; i < arglist.Count; i++)
                                {
                                    propLookup += "." + arglist[i];
                                }
                                SetPropertyValue(trigObj, xmlObject, propLookup, value);
                            }
                        }
                        else
                        {
                            string propLookup = arglist[2]; // add this first so later additions all prepended with '.'
                            for (int i = 3; i < arglist.Count; i++)
                            {
                                propLookup += "." + arglist[i];
                            }
                            SetPropertyValue(trigObj, xmlObject.Value, propLookup, value);
                        }
                    }
                    
                    //else if (arglist.Count > 2)
                    //{
                        // could be setting a property on an existing XmlAttachment!
                        // e.g. xmlints.test.expiration = 0:0:1
                    //    SetPropertyValue(trigObject, xmlObject, arglist[2], value);
                    //}
                    return;
                }
            }

            /*
            string[] keywordargs = ParseString(propname, 4, ",");

            // check for special keywords
            if (keywordargs[0] == "ATTACHMENT")
            {
                if (keywordargs.Length < 4)
                {
                    return "Invalid ATTACHMENT format";
                }
                // syntax is ATTACHMENT,type,name,propname

                string apropname = keywordargs[3];
                string aname = keywordargs[2];
                Type attachtype = SpawnerType.GetType(keywordargs[1]);

                // allow empty string specifications to be used to indicate a null string which will match any name
                if (aname == "") aname = null;

                ArrayList attachments = XmlAttach.FindAttachments(o, attachtype, aname);

                if (attachments != null && attachments.Count > 0)
                {
                    // change the object, object type, and propname to refer to the attachment
                    o = attachments[0];
                    propname = apropname;

                    if (o == null)
                    {
                        return "Null object";
                    }

                    type = o.GetType();
                    props = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                }
                else
                    return "Attachment not found";

            }
            else if (keywordargs[0] == "SKILL")
            {
                if (keywordargs.Length < 2)
                {
                    return "Invalid SKILL format";
                }
                bool found = true;
                try
                {
                    SkillName skillname = (SkillName)Enum.Parse(typeof(SkillName), keywordargs[1], true);
                    if (o is Mobile)
                    {

                        Skill skill = ((Mobile)o).Skills[skillname];

                        skill.Base = double.Parse(value);

                        return "Property has been set.";
                    }
                    else
                        return "Object is not mobile";
                }
                catch { found = false; }

                if (!found)
                    return "Invalid SKILL reference.";
            }
            else if (keywordargs[0] == "STEALABLE")
            {

                bool found = true;
                try
                {
                    if (o is Item)
                    {

                        ItemFlags.SetStealable(o as Item, bool.Parse(value));

                        return "Property has been set.";
                    }
                    else
                        return "Object is not an item";
                }
                catch { found = false; }

                if (!found)
                    return "Invalid Stealable assignment.";
            }
             * */

            // do a bit of parsing to handle array references
            string[] arraystring = propname.Split('[');
            int index = 0;
            if (arraystring.Length > 1)
            {
                // parse the property name from the indexing
                propname = arraystring[0];

                // then parse to get the index value
                string[] arrayvalue = arraystring[1].Split(']');

                if (arrayvalue.Length > 0)
                {
                    try
                    {
                        index = int.Parse(arrayvalue[0]);
                    }
                    catch 
                    {
                        try
                        {
                            index = (int)(new MathTree(null, arrayvalue[0])).Calculate(trigObj);
                        }
                        catch
                        {
                            throw new UberScriptException("Could not get int array value from: " + arrayvalue[0] + " for prop of " + name + " on object " + o);
                        }
                    }
                }
            }
            PropertyInfo propInfoToSet = null;
            string lowerCasePropName = propname.ToLower();

            // optimization to find propertyInfo without looping through
            if (o is BaseCreature)
            {
                if (BaseCreatureProperties.ContainsKey(lowerCasePropName)) propInfoToSet = BaseCreatureProperties[lowerCasePropName];
            }
            else if (o is PlayerMobile)
            {
                if (PlayerMobileProperties.ContainsKey(lowerCasePropName)) propInfoToSet = PlayerMobileProperties[lowerCasePropName];
            }
            else if (o is Item)
            {
                if (ItemProperties.ContainsKey(lowerCasePropName)) propInfoToSet = ItemProperties[lowerCasePropName];
            }
            // is a nested property with attributes so first get the property
            if (propInfoToSet == null) foreach (PropertyInfo p in props)
                {
                    if (Insensitive.Equals(p.Name, propname))
                    {
                        propInfoToSet = p;
                        break;
                    }
                }
            if (propInfoToSet != null)
            {
                //if (IsProtected(type, propname))
                //    return "Property is protected.";

                if (arglist.Count > 1)
                {
                    //ptype = propInfoToSet.PropertyType;
                    po = propInfoToSet.GetValue(o, null);
                    // now set the nested attribute using the new property list
                    string propLookup = arglist[1];
                    for (int i = 2; i < arglist.Count; i++)
                    {
                        propLookup += "." + arglist[i];
                    }
                    SetPropertyValue(trigObj, po, propLookup, value);
                    return;
                    // otherwise let it roll through and throw an exception
                }
                else // arglist.Count == 1
                {
                    //if (IsProtected(type, propname))
                    //    return "Property is protected.";
                    if (!propInfoToSet.CanWrite)
                        throw new UberScriptException("Property is read only.");

                    InternalSetValue(o, propInfoToSet, value, false, index);
                    return;
                }
            }

            throw new UberScriptException("Property " + name + " not found on " + o + ".");
        }

        public static void InternalSetValue( object o, PropertyInfo p, object value, bool shouldLog, int index)
        {
            object toSet = null;
            Type ptype = p.PropertyType;

            if (value is string)
            {
                ConstructFromString(p, p.PropertyType, o, value as string, ref toSet);
            }
            else if (value == null)
            {
                Type oType = o.GetType();
                if (!oType.IsValueType)
                {
                    p.SetValue(o, null, null);
                } 
                else
                {
                    throw new UberScriptException("Set value error: The property " + p + " was not nullable on " + o + " is a non-nullable object that was attempted to be set to null.");
                }
            }
            else if (ptype.IsAssignableFrom(value.GetType()))
            {
                p.SetValue(o, value, null);
                return;
            }
            else
            {
                throw new UberScriptException("Attempted to set " + ptype + " with an object: " + value);
            }

            //if (shouldLog)
            //    CommandLogging.LogChangeProperty(from, o, p.Name, value);

            if (ptype.IsPrimitive)
            {
                p.SetValue(o, toSet, null);
            }
            else if ((ptype.GetInterface("IList") != null) && index >= 0)
            {
                //try
                    object arrayvalue = p.GetValue(o, null);
                    ((IList)arrayvalue)[index] = toSet;
            }
            else
            {
                p.SetValue(o, toSet, null);
            }
        }

        public static void ConstructFromString(PropertyInfo p, Type type, object obj, string value, ref object constructed)
        {
            object toSet;

            if (value == "null" && !type.IsValueType)
                value = null;

            if (IsEnum(type))
            {

                try
                {
                    toSet = Enum.Parse(type, value, true);
                }
                catch
                {
                    throw new UberScriptException("That is not a valid enumeration member.");
                }
            }
            else if (IsCustomEnum(type))
            {

                try
                {

                    MethodInfo info = p.PropertyType.GetMethod("Parse", new Type[] { typeof(string) });

                    if (info != null)
                        toSet = info.Invoke(null, new object[] { value });
                    else if (p.PropertyType == typeof(Enum) || p.PropertyType.IsSubclassOf(typeof(Enum)))
                        toSet = Enum.Parse(p.PropertyType, value, false);
                    else
                        toSet = null;

                    if (toSet == null)
                        throw new UberScriptException("That is not a valid custom enumeration member.");

                }
                catch
                {
                    throw new UberScriptException("That is not a valid custom enumeration member.");
                }
            }
            else if (IsType(type))
            {

                try
                {
                    toSet = ScriptCompiler.FindTypeByName(value);

                    if (toSet == null)
                        throw new UberScriptException("No type with that name was found.");
                }
                catch
                {
                    throw new UberScriptException("No type with that name was found.");
                }
            }
            else if (IsParsable(type))
            {

                try
                {
                    toSet = Parse(obj, type, value);
                }
                catch
                {
                    throw new UberScriptException("That is not properly formatted.");
                }
            }
            else if (value == null)
            {
                toSet = null;
            }
            else if (value.StartsWith("0x") && IsNumeric(type))
            {
                try
                {
                    toSet = Convert.ChangeType(Convert.ToUInt64(value.Substring(2), 16), type);
                }
                catch
                {
                    throw new UberScriptException("That is not properly formatted. not convertible.");
                }
            }
            else if (value.StartsWith("0x") && (IsItem(type) || IsMobile(type)))
            {
                try
                {
                    // parse out the mobile or item name from the value string
                    int ispace = value.IndexOf(' ');
                    string valstr = value.Substring(2);
                    if (ispace > 0)
                    {
                        valstr = value.Substring(2, ispace - 2);
                    }

                    toSet = World.FindEntity(Convert.ToInt32(valstr, 16));

                    // now check to make sure the object returned is consistent with the type
                    if (!((toSet is Mobile && IsMobile(type)) || (toSet is Item && IsItem(type))))
                    {
                        throw new UberScriptException("Item/Mobile type mismatch. cannot assign.");
                    }
                }
                catch
                {
                    throw new UberScriptException("That is not properly formatted. not convertible.");
                }
            }
            else if ((type.GetInterface("IList") != null))
            {
                try
                {
                    object arrayvalue = p.GetValue(obj, null);

                    object po = ((IList)arrayvalue)[0];

                    Type atype = po.GetType();

                    toSet = Parse(obj, atype, value);
                }
                catch
                {
                    throw new UberScriptException("That is not properly formatted.");
                }
            }
            else
            {
                try
                {
                    toSet = Convert.ChangeType(value, type);
                }
                catch
                {
                    throw new UberScriptException("That is not properly formatted.");
                }
            }

            constructed = toSet;
        }

        // -------------------------------------------------------------
        // Modified from Beta-36 Properties.cs code
        // -------------------------------------------------------------

        private static Type typeofTimeSpan = typeof(TimeSpan);
        private static Type typeofParsable = typeof(ParsableAttribute);
        private static Type typeofCustomEnum = typeof(CustomEnumAttribute);

        private static bool IsParsable(Type t)
        {
            return (t == typeofTimeSpan || t.IsDefined(typeofParsable, false));
        }

        private static Type[] m_ParseTypes = new Type[] { typeof(string) };
        private static object[] m_ParseParams = new object[1];

        private static object Parse(object o, Type t, string value)
        {
            MethodInfo method = t.GetMethod("Parse", m_ParseTypes);

            m_ParseParams[0] = value;

            return method.Invoke(o, m_ParseParams);
        }

        private static Type[] m_NumericTypes = new Type[]
		{
			typeof( Byte ), typeof( SByte ),
			typeof( Int16 ), typeof( UInt16 ),
			typeof( Int32 ), typeof( UInt32 ),
			typeof( Int64 ), typeof( UInt64 ), typeof( Server.Serial )
		};

        public static bool IsNumeric(Type t)
        {
            return (Array.IndexOf(m_NumericTypes, t) >= 0);
        }

        private static Type typeofType = typeof(Type);

        private static bool IsType(Type t)
        {
            return (t == typeofType);
        }

        private static Type typeofChar = typeof(Char);

        private static bool IsChar(Type t)
        {
            return (t == typeofChar);
        }

        private static Type typeofString = typeof(String);

        private static bool IsString(Type t)
        {
            return (t == typeofString);
        }

        private static bool IsEnum(Type t)
        {
            return t.IsEnum;
        }

        private static bool IsCustomEnum(Type t)
        {
            return t.IsDefined(typeofCustomEnum, false);
        }

        // -------------------------------------------------------------
        //	End modified Beta-36 Properties.cs code
        // -------------------------------------------------------------

        public static bool IsItem(Type type)
        {
            return (type != null && (type == typeof(Item) || type.IsSubclassOf(typeof(Item))));
        }

        public static bool IsMobile(Type type)
        {
            return (type != null && (type == typeof(Mobile) || type.IsSubclassOf(typeof(Mobile))));
        }

        /*
        public static bool IsProtected(Type type, string property)
        {
            if (type == null || property == null) return false;

            // search through the protected list for a matching entry
            foreach (ProtectedProperty p in ProtectedPropertiesList)
            {
                if ((p.ObjectType == type || type.IsSubclassOf(p.ObjectType)) && (property.ToLower() == p.Name.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }*/
    }
}
