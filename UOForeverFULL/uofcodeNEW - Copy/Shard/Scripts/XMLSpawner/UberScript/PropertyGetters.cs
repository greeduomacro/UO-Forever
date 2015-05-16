using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Server.Mobiles;
using System.Collections;

namespace Server.Engines.XmlSpawner2
{
    // These functions are borrowed heavily from Arte Gordon's BaseXmlSpawner class
    class PropertyGetters
    {
        /// <summary>
        /// Propsplitter is like string.Split('.') except it keeps array accessors together (like objs.mobs[objs.mobs.count - 1])
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static List<string> PropSplitter(string property)
        {
            List<string> output = new List<string>();
            int numOpenBrackets = 0;
            for (int i = 0; i < property.Length; i++)
            {
                if (numOpenBrackets > 0 && property[i] != '[' && property[i] != ']')
                {
                    continue;
                }
                if (property[i] == '.')
                {
                    // it's just a simple property (e.g. "hits" in objs.mobslist[1].hits)
                    output.Add(property.Substring(0, i));
                    property = property.Substring(i + 1);
                    i = 0;
                }
                else if (property[i] == '[')
                {
                    numOpenBrackets++;
                }
                else if (property[i] == ']')
                {
                    numOpenBrackets--;
                    if (numOpenBrackets < 0)
                    {
                        throw new UberScriptException(property + " did not have balanced []!");
                    }
                }
            }
            if (property.Length != 0)
            {
                output.Add(property); // add the final property
            }
            return output;
        }


        public static string GetStringValue(TriggerObject trigObject, object o, string name, out Type ptype)
        {
            object value = GetObject(trigObject, o, name, out ptype);
            string ToString;
            if (value == null)
                ToString = "null";
            else
                ToString = value.ToString();

            return ToString;
        }

        public static object GetObject(TriggerObject trigObj, object o, string name, out Type ptype) // used for all comparisons
        {
            // ALL EXCEPTIONS CAUGHT OUTSIDE OF THIS FUNCTION (so that a useful error can be displayed)
            // trigobject used in case there are array indeces with a function [THIS().x]
            ptype = null;
            if (o == null || name == null) return null;

            Type type = o.GetType();
            object po = null;

            PropertyInfo[] props = null;
            try
            {
                props = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            }
            catch (Exception e)
            {
                throw new UberScriptException("GetProperties Error!", e);
            }

            // parse the strings of the form property.attribute into two parts
            // first get the property
            List<string> arglist = PropSplitter(name);
            string propname = arglist[0];
            if (propname.Length == 0) throw new UberScriptException("Empty string property!");
            if (arglist.Count > 1 && (propname[0] == 'x' || propname[0] == 'X'))
            {
                if (propname.ToLower() == "xmlints")
                {
                    IEntity entity = o as IEntity;
                    if (entity == null) throw new UberScriptException("Can't set xmlints on anything but Mobile or Item");
                    // check for existing xmlValue attachment or create a new one
                    XmlValue xmlValue = XmlAttach.GetValueAttachment(o as IEntity, arglist[1]);
                    if (xmlValue == null) return null;//throw new UberScriptException("Could not find XmlValue named " + name + " on " + o);
                    if (arglist.Count > 2) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
                        return GetObject(trigObj, xmlValue, arglist[2], out ptype);
                    ptype = typeof(int);
                    return xmlValue.Value;
                }
                else if (propname.ToLower() == "xmlstrings")
                {
                    IEntity entity = o as IEntity;
                    if (entity == null) throw new UberScriptException("Can't set xmlstrings on anything but Mobile or Item");
                    XmlLocalVariable xmlLocalVariable = XmlAttach.GetStringAttachment(o as IEntity, arglist[1]);
                    if (xmlLocalVariable == null) return null; // throw new UberScriptException("Could not find XmlLocalVariable named " + name + " on " + o);
                    if (arglist.Count > 2) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
                        return GetObject(trigObj, xmlLocalVariable, arglist[2], out ptype);
                    ptype = typeof(string);
                    return xmlLocalVariable.Data;
                }
                else if (propname.ToLower() == "xmldoubles")
                {
                    IEntity entity = o as IEntity;
                    if (entity == null) throw new UberScriptException("Can't set xmldoubles on anything but Mobile or Item");
                    XmlDouble xmlDouble = XmlAttach.GetDoubleAttachment(entity, arglist[1]);
                    if (xmlDouble == null) return null; //  throw new UberScriptException("Could not find XmlDouble named " + name + " on " + o);
                    if (arglist.Count > 2) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
                        return GetObject(trigObj, xmlDouble, arglist[2], out ptype);
                    ptype = typeof(double);
                    return xmlDouble.Value;
                }
                else if (propname.ToLower() == "xmlobjs")
                {
                    IEntity entity = o as IEntity;
                    if (entity == null) throw new UberScriptException("Can't set xmlobjs on anything but Mobile or Item");
                    // since the object might be a list of some kind, need to check for the [] indexing first
                    string objName = arglist[1];
                    int openBracketIndex = objName.IndexOf('[');
                    int listindex = -1; // for lists / arrays
                    if (openBracketIndex > 0)
                    {
                        int closingBracketIndex = objName.IndexOf(']');
                        if (closingBracketIndex < openBracketIndex + 1)
                        {
                            throw new UberScriptException("xmlobjs." + objName + " [] indexing error: must have at least 1 character between the []");
                        }

                        listindex = (int)(new MathTree(null, objName.Substring(openBracketIndex + 1, closingBracketIndex - openBracketIndex - 1))).Calculate(trigObj);
                        objName = objName.Substring(0, openBracketIndex);
                    }

                    XmlObject xmlObject = XmlAttach.GetObjectAttachment(entity, objName);
                    if (xmlObject == null) return null; // throw new UberScriptException("Could not find XmlObject named " + name + " on " + o);
                    //if (arglist.Count > 2) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
                    //    return GetObject(trigObject, xmlObject, arglist[2], out ptype);
                    

                    //=====
                    if (arglist.Count > 2)
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
                            if (arglist.Count < 4) return xmlObject;

                            string propLookup = arglist[3]; // add this first so later additions all prepended with '.'
                            for (int i = 4; i < arglist.Count; i++)
                            {
                                propLookup += "." + arglist[i];
                            }
                            return PropertyGetters.GetObject(trigObj, xmlObject, propLookup, out ptype);
                        }
                        else
                        {
                            if (xmlObject.Value == null) return null;
                            string propLookup = arglist[2]; // add this first so later additions all prepended with '.'
                            for (int i = 3; i < arglist.Count; i++)
                            {
                                propLookup += "." + arglist[i];
                            }

                            if (listindex >= 0)
                            {
                                ptype = xmlObject.Value.GetType();
                                if (ptype.GetInterface("IList") != null)
                                {
                                    try
                                    {
                                        return PropertyGetters.GetObject(trigObj, ((IList)xmlObject.Value)[listindex], propLookup, out ptype);
                                        //return ((IList)xmlObject.Value)[listindex];
                                    }
                                    catch (Exception e)
                                    {
                                        throw new UberScriptException("Get Array value error!", e);
                                    }
                                }
                                else
                                {
                                    throw new UberScriptException("xmlobj." + objName + " was indexed with [] indexer, but did not contain a list!");
                                }
                            }
                            
                            return PropertyGetters.GetObject(trigObj, xmlObject.Value, propLookup, out ptype);
                        }
                    }
                    else if (listindex >= 0)
                    {
                        // no properties afterward but has a list index (e.g. xmlobjs.spawnedPlatforms[0])
                        ptype = xmlObject.Value.GetType();
                        if (ptype.GetInterface("IList") != null)
                        {
                            try
                            {
                                return ((IList)xmlObject.Value)[listindex];
                            }
                            catch (Exception e)
                            {
                                throw new UberScriptException("Get Array value error!", e);
                            }
                        }
                        else
                        {
                            throw new UberScriptException("xmlobj." + objName + " was indexed with [] indexer, but did not contain a list!");
                        }
                    }
                    //====


                    ptype = typeof(object);
                    return xmlObject.Value;
                }
            }
            // parse up to 4 comma separated args for special keyword properties
            /*
            string[] keywordargs = ParseString(propname, 4, ",");

            // check for special keywords
            if (keywordargs[0] == "ATTACHMENT")
            {
                // syntax is ATTACHMENT,type,name,property
                if (keywordargs.Length < 4)
                {
                    return "Invalid ATTACHMENT format";
                }

                string apropname = keywordargs[3];
                string aname = keywordargs[2];
                Type attachtype = SpawnerType.GetType(keywordargs[1]);

                // allow empty string specifications to be used to indicate a null string which will match any name
                if (aname == "") aname = null;

                ArrayList attachments = XmlAttach.FindAttachments(o, attachtype, aname);

                if (attachments != null && attachments.Count > 0)
                {
                    string getvalue = GetPropertyValue(spawner, attachments[0], apropname, out ptype);

                    return getvalue;
                }
                else
                    return "Attachment not found";
            }
             * */

            // I REALLY OUGHT TO IMPLEMENT GETTING TYPE SOME TIME
            // do a bit of parsing to handle array references
            string[] arraystring = arglist[0].Split('[');
            int index = -1;
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

            PropertyInfo propInfoToGet = null;
            string lowerCasePropName = propname.ToLower();

            // optimization to find propertyInfo without looping through
            if (o is BaseCreature)
            {
                if (PropertySetters.BaseCreatureProperties.ContainsKey(lowerCasePropName)) propInfoToGet = PropertySetters.BaseCreatureProperties[lowerCasePropName];
            }
            else if (o is PlayerMobile)
            {
                if (PropertySetters.PlayerMobileProperties.ContainsKey(lowerCasePropName)) propInfoToGet = PropertySetters.PlayerMobileProperties[lowerCasePropName];
            }
            else if (o is Item)
            {
                if (PropertySetters.ItemProperties.ContainsKey(lowerCasePropName)) propInfoToGet = PropertySetters.ItemProperties[lowerCasePropName];
            }
            // is a nested property with attributes so first get the property
            if (propInfoToGet == null) foreach (PropertyInfo p in props)
            {
                if (Insensitive.Equals(p.Name, propname))
                {
                    propInfoToGet = p;
                    break;
                }
            }
            if (propInfoToGet != null)
            {
                if (!propInfoToGet.CanRead)
                    throw new UberScriptException("Property " + propname + " is write only.");
                //if (IsProtected(type, propname))
                //    return "Property is protected.";
                ptype = propInfoToGet.PropertyType;
                if (arglist.Count > 1)
                {
                    if (ptype.IsPrimitive)
                    {
                        po = propInfoToGet.GetValue(o, null);
                    }
                    else if ((ptype.GetInterface("IList") != null) && index >= 0)
                    {
                        try
                        {
                            object arrayvalue = propInfoToGet.GetValue(o, null);
                            po = ((IList)arrayvalue)[index];
                        }
                        catch (Exception e)
                        { 
                            throw new UberScriptException("Get Array value error!",e);
                        }
                    }
                    else
                    {
                        po = propInfoToGet.GetValue(o, null);
                    }
                    // now set the nested attribute using the new property list
                    string propLookup = arglist[1];
                    for (int i = 2; i < arglist.Count; i++)
                    {
                        propLookup += "." + arglist[i];
                    }
                    return (GetObject(trigObj, po, propLookup, out ptype));
                }
                else
                {
                    // its just a simple single property
                    return InternalGetValue(o, propInfoToGet, index);
                }
            }

            throw new UberScriptException("Could not find property on " + o + ": " + name);
        }

        // -------------------------------------------------------------
        // Begin modified code from Beta-36 Properties.cs
        // Added support for nested attribute and array access
        // -------------------------------------------------------------
        public static object InternalGetValue(object o, PropertyInfo p, int index)
        {
            Type type = p.PropertyType;
            object value = null;

            if (type.IsPrimitive)
            {
                value = p.GetValue(o, null);
            }
            else if ((type.GetInterface("IList") != null) && index >= 0)
            {
                try
                {
                    object arrayvalue = p.GetValue(o, null);
                    value = ((IList)arrayvalue)[index];
                }
                catch { }
            }
            else
            {
                value = p.GetValue(o, null);
            }

            return value;
        }
    }
}
