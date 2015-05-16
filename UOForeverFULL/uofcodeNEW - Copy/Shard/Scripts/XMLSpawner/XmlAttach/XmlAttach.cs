#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Reflection;

using Server.Commands;
using Server.Commands.Generic;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

using CPA = Server.CommandPropertyAttribute;
#endregion

namespace Server.Engines.XmlSpawner2
{
	[AttributeUsage(AttributeTargets.Constructor)]
	public class Attachable : Attribute
	{ }

	public class ASerial
	{
		private readonly int m_SerialValue;

		public int Value { get { return m_SerialValue; } }

		public ASerial(int serial)
		{
			m_SerialValue = serial;
		}

		private static int m_GlobalSerialValue;

		public static bool serialInitialized = false;

		public static ASerial NewSerial()
		{
			// it is possible for new attachments to be constructed before existing attachments are deserialized and the current m_globalserialvalue
			// restored.  This creates a possible serial conflict, so dont allow assignment of valid serials until proper deser of m_globalserialvalue
			// Resolve unassigned serials in initialization
			if (!serialInitialized)
			{
				return new ASerial(0);
			}

			if (m_GlobalSerialValue == int.MaxValue || m_GlobalSerialValue < 0)
			{
				m_GlobalSerialValue = 0;
			}

			// try the next serial number in the series
			int newserialno = m_GlobalSerialValue + 1;

			// check to make sure that it is not in use
			while (XmlAttach.AllAttachments.ContainsKey(newserialno))
			{
				newserialno++;
				if (newserialno == int.MaxValue || newserialno < 0)
				{
					newserialno = 1;
				}
			}

			m_GlobalSerialValue = newserialno;

			return new ASerial(m_GlobalSerialValue);
		}

		internal static void GlobalSerialize(GenericWriter writer)
		{
			writer.Write(m_GlobalSerialValue);
		}

		internal static void GlobalDeserialize(GenericReader reader)
		{
			m_GlobalSerialValue = reader.ReadInt();
		}
	}

	public class XmlAttach
	{
		private static readonly Type m_AttachableType = typeof(Attachable);

		public static bool IsAttachable(ConstructorInfo ctor)
		{
			return ctor.IsDefined(m_AttachableType, false);
		}

		public static void HashSerial(ASerial key, XmlAttachment o)
		{
			if (key.Value != 0)
			{
				AllAttachments.Add(key.Value, o);
			}
			else
			{
				UnassignedAttachments.Add(o);
			}
		}

		// each entry in the hashtable is an array of XmlAttachments that is keyed by an object.
		public static Dictionary<int, ArrayList> EntityAttachments = new Dictionary<int, ArrayList>();
		// keyed by entity serial

		public static Dictionary<int, XmlAttachment> AllAttachments = new Dictionary<int, XmlAttachment>();
		// keyed by attachment serial

		private static readonly ArrayList UnassignedAttachments = new ArrayList();

		public static bool HasAttachments(IEntity o)
		{
			if (o == null)
			{
				return false;
			}

			ArrayList alist = null;
			EntityAttachments.TryGetValue(o.Serial.Value, out alist);

			if (alist == null || alist.Count == 0)
			{
				return false;
			}

			// check to see if there are any valid attachments in the list
			foreach (XmlAttachment a in alist)
			{
				if (!a.Deleted)
				{
					return true;
				}
			}

			return false;
		}

		public static XmlAttachment[] Values
		{
			get
			{
				var valuearray = new XmlAttachment[AllAttachments.Count];
				AllAttachments.Values.CopyTo(valuearray, 0);
				return valuearray;
			}
		}

		public static void Configure()
		{
			EventSink.WorldLoad += Load;
			EventSink.WorldSave += Save;
		}

		public static void Initialize()
		{
			ASerial.serialInitialized = true;

			// resolve unassigned serials
			foreach (XmlAttachment a in UnassignedAttachments)
			{
				// get the next unique serial id
				ASerial serial = ASerial.NewSerial();
				a.Serial = serial;

				// register the attachment in the serial keyed hashtable
				HashSerial(serial, a);
			}

			// Register our speech handler
			//EventSink.Speech += new SpeechEventHandler(EventSink_Speech); // not needed anymore with UberScript

			// Register our movement handler
			//EventSink.Movement += new MovementEventHandler(EventSink_Movement);

			//CommandSystem.Register( "ItemAtt", AccessLevel.GameMaster, new CommandEventHandler( ListItemAttachments_OnCommand ) );
			//CommandSystem.Register( "MobAtt", AccessLevel.GameMaster, new CommandEventHandler( ListMobileAttachments_OnCommand ) );
			CommandSystem.Register("GetAtt", AccessLevel.GameMaster, GetAttachments_OnCommand);
			//CommandSystem.Register( "DelAtt", AccessLevel.GameMaster, new CommandEventHandler( DeleteAttachments_OnCommand ) );
			//CommandSystem.Register( "TrigAtt", AccessLevel.GameMaster, new CommandEventHandler( ActivateAttachments_OnCommand ) );
			//CommandSystem.Register( "AddAtt", AccessLevel.GameMaster, new CommandEventHandler( AddAttachment_OnCommand ) );
			TargetCommands.Register(new AddAttCommand());
			TargetCommands.Register(new DelAttCommand());
			CommandSystem.Register("AvailAtt", AccessLevel.GameMaster, ListAvailableAttachments_OnCommand);
			TargetCommands.Register(new PrintAttCommand());
		}

		public class PrintAttCommand : BaseCommand
		{
			public PrintAttCommand()
			{
				AccessLevel = AccessLevel.GameMaster;
				Supports = CommandSupport.All;
				Commands = new[] {"PrintAtt"};
				ObjectTypes = ObjectTypes.Both;
				Usage = "PrintAtt type [attributename-e.g. successaction]";
				Description = "Prints properties of an attachment to a file.";
				ListOptimized = true;
			}

			public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
			{
				if (e.Arguments.Length >= 2)
				{
					return true;
				}

				e.Mobile.SendMessage("Usage: " + Usage);
				return false;
			}

			public override void ExecuteList(CommandEventArgs e, ArrayList list)
			{
				if (e != null && list != null && e.Length >= 1)
				{
					// create a new attachment and add it to the item
					int nargs = e.Arguments.Length - 1;

					var args = new string[nargs];

					for (int j = 0; j < nargs; j++)
					{
						args[j] = e.Arguments[j + 1];
					}

					Type attachtype = SpawnerType.GetType(e.Arguments[0]);

					if (attachtype != null && attachtype.IsSubclassOf(typeof(XmlAttachment)))
					{
						for (int i = 0; i < list.Count; i++)
						{
							ArrayList alist = FindAttachments(list[i] as IEntity, attachtype);
							if (alist != null)
							{
								foreach (XmlAttachment a in alist)
								{
									var props = attachtype.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
									for (int j = 0; j < props.Length; j++)
									{
										if (props[j].CanRead && props[j].Name.ToLower() == args[0].ToLower())
										{
											AddResponse(BaseXmlSpawner.InternalGetValue(a, props[j], 0));
										}
									}
								}
							}
						}
					}
					else
					{
						AddResponse(String.Format("Invalid attachment type {0}", e.Arguments[0]));
					}
				}
			}
		}

		public class AddAttCommand : BaseCommand
		{
			public AddAttCommand()
			{
				AccessLevel = AccessLevel.GameMaster;
				Supports = CommandSupport.All;
				Commands = new[] {"AddAtt"};
				ObjectTypes = ObjectTypes.Both;
				Usage = "AddAtt type [args]";
				Description = "Adds an attachment to the targeted object.";
				ListOptimized = true;
			}

			public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
			{
				if (e.Arguments.Length >= 1)
				{
					return true;
				}

				e.Mobile.SendMessage("Usage: " + Usage);
				return false;
			}

			public override void ExecuteList(CommandEventArgs e, ArrayList list)
			{
				if (e != null && list != null && e.Length >= 1)
				{
					// create a new attachment and add it to the item
					int nargs = e.Arguments.Length - 1;

					var args = new string[nargs];

					for (int j = 0; j < nargs; j++)
					{
						args[j] = e.Arguments[j + 1];
					}

					Type attachtype = SpawnerType.GetType(e.Arguments[0]);

					if (attachtype != null && attachtype.IsSubclassOf(typeof(XmlAttachment)))
					{
						// go through all of the objects in the list
						int count = 0;

						for (int i = 0; i < list.Count; ++i)
						{
							XmlAttachment o = (XmlAttachment)XmlSpawner.CreateObject(attachtype, args, false, true);

							if (o == null)
							{
								AddResponse(String.Format("Unable to construct {0} with specified args", attachtype.Name));
								break;
							}

							if (AttachTo(null, list[i] as IEntity, o, true))
							{
								if (list.Count < 10)
								{
									AddResponse(String.Format("Added {0} to {1}", attachtype.Name, list[i]));
								}
								count++;
							}
							else
							{
								LogFailure(String.Format("Attachment {0} not added to {1}", attachtype.Name, list[i]));
							}
						}
						if (count > 0)
						{
							AddResponse(String.Format("Attachment {0} has been added [{1}]", attachtype.Name, count));
						}
						else
						{
							AddResponse(String.Format("Attachment {0} not added", attachtype.Name));
						}
					}
					else
					{
						AddResponse(String.Format("Invalid attachment type {0}", e.Arguments[0]));
					}
				}
			}
		}

		public class DelAttCommand : BaseCommand
		{
			public DelAttCommand()
			{
				AccessLevel = AccessLevel.GameMaster;
				Supports = CommandSupport.All;
				Commands = new[] {"DelAtt"};
				ObjectTypes = ObjectTypes.Both;
				Usage = "DelAtt type [optional name]";
				Description = "Deletes an attachment on the targeted object.";
				ListOptimized = true;
			}

			public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
			{
				if (e.Arguments.Length >= 1)
				{
					return true;
				}

				e.Mobile.SendMessage("Usage: " + Usage);
				return false;
			}

			public override void ExecuteList(CommandEventArgs e, ArrayList list)
			{
				string name = null;
				bool deleteOnlyNullNamedAttachment = false;
				if (e != null && list != null && e.Length >= 1)
				{
					if (e.Length >= 2)
					{
						name = e.Arguments[1];
						if (name == "(-null-)")
						{
							deleteOnlyNullNamedAttachment = true;
						}
					}

					Type attachtype = SpawnerType.GetType(e.Arguments[0]);

					if (attachtype != null && attachtype.IsSubclassOf(typeof(XmlAttachment)))
					{
						// go through all of the objects in the list
						int count = 0;

						for (int i = 0; i < list.Count; ++i)
						{
							ArrayList alist = FindAttachments(list[i] as IEntity, attachtype);

							if (alist != null)
							{
								// delete the attachments
								foreach (XmlAttachment a in alist)
								{
									if (name == null || a.Name == name || (a.Name == null && deleteOnlyNullNamedAttachment))
									{
										a.Delete();
										if (list.Count < 10)
										{
											AddResponse(String.Format("Deleted {0} from {1}", attachtype.Name, list[i]));
										}
										count++;
									}
								}
							}
						}

						if (count > 0)
						{
							AddResponse(String.Format("Attachment {0} has been deleted [{1}]", attachtype.Name, count));
						}
						else
						{
							AddResponse(String.Format("Attachment {0} not deleted", attachtype.Name));
						}
					}
					else
					{
						AddResponse(String.Format("Invalid attachment type {0}", e.Arguments[0]));
					}
				}
			}
		}

		public static void CleanUp()
		{
			// clean up any unowned attachments
			foreach (XmlAttachment a in Values)
			{
				if (a.OwnedBy == null || (a.OwnedBy is Mobile && ((Mobile)a.OwnedBy).Deleted) ||
					(a.OwnedBy is Item && ((Item)a.OwnedBy).Deleted))
				{
					a.Delete();
				}
			}
		}

		public static void Save(WorldSaveEventArgs e)
		{
			if (EntityAttachments == null)
			{
				return;
			}

			CleanUp();

			if (!Directory.Exists("Saves/Attachments"))
			{
				Directory.CreateDirectory("Saves/Attachments");
			}

			string filePath = Path.Combine("Saves/Attachments", "Attachments.bin"); // the attachment serializations
			string imaPath = Path.Combine("Saves/Attachments", "Attachments.ima"); // the item/mob attachment tables
			string fpiPath = Path.Combine("Saves/Attachments", "Attachments.fpi"); // the file position indices

			BinaryFileWriter writer = null;
			BinaryFileWriter imawriter = null;
			BinaryFileWriter fpiwriter = null;

			try
			{
				writer = new BinaryFileWriter(filePath, true);
				imawriter = new BinaryFileWriter(imaPath, true);
				fpiwriter = new BinaryFileWriter(fpiPath, true);
			}
			catch (Exception err)
			{
				ErrorReporter.GenerateErrorReport(err.ToString());
				return;
			}

			if (writer != null && imawriter != null && fpiwriter != null)
			{
				// save the current global attachment serial state
				ASerial.GlobalSerialize(writer);

				// remove all deleted attachments
				FullDefrag();

				// save the attachments themselves
				if (AllAttachments != null)
				{
					writer.Write(AllAttachments.Count);

					var valuearray = new XmlAttachment[AllAttachments.Count];
					AllAttachments.Values.CopyTo(valuearray, 0);

					var keyarray = new int[AllAttachments.Count];
					AllAttachments.Keys.CopyTo(keyarray, 0);

					for (int i = 0; i < keyarray.Length; i++)
					{
						// write the key
						writer.Write(keyarray[i]);

						XmlAttachment a = valuearray[i];

						// write the value type
						writer.Write(a.GetType().ToString());

						// serialize the attachment itself
						a.Serialize(writer);

						// save the fileposition index
						fpiwriter.Write(writer.Position);
					}
				}
				else
				{
					writer.Write(0);
				}

				writer.Close();

				/* // Collapsed into a single IEntity Hash
                // save the hash table info for items and mobiles
                // mobile attachments
                if (XmlAttach.MobileAttachments != null)
                {
                    imawriter.Write(XmlAttach.MobileAttachments.Count);

                    object[] valuearray = new object[XmlAttach.MobileAttachments.Count];
                    XmlAttach.MobileAttachments.Values.CopyTo(valuearray, 0);

                    object[] keyarray = new object[XmlAttach.MobileAttachments.Count];
                    XmlAttach.MobileAttachments.Keys.CopyTo(keyarray, 0);

                    for (int i = 0; i < keyarray.Length; i++)
                    {
                        // write the key
                        imawriter.Write((Mobile)keyarray[i]);

                        // write out the attachments
                        ArrayList alist = (ArrayList)valuearray[i];

                        imawriter.Write((int)alist.Count);
                        foreach (XmlAttachment a in alist)
                        {
                            // write the attachment serial
                            imawriter.Write((int)a.Serial.Value);

                            // write the value type
                            imawriter.Write((string)a.GetType().ToString());

                            // save the fileposition index
                            fpiwriter.Write((long)imawriter.Position);
                        }
                    }
                }
                else
                {
                    // no mobile attachments
                    imawriter.Write((int)0);
                }

                // item attachments
                if (XmlAttach.ItemAttachments != null)
                {
                    imawriter.Write(XmlAttach.ItemAttachments.Count);

                    object[] valuearray = new object[XmlAttach.ItemAttachments.Count];
                    XmlAttach.ItemAttachments.Values.CopyTo(valuearray, 0);

                    object[] keyarray = new object[XmlAttach.ItemAttachments.Count];
                    XmlAttach.ItemAttachments.Keys.CopyTo(keyarray, 0);

                    for (int i = 0; i < keyarray.Length; i++)
                    {
                        // write the key
                        imawriter.Write((Item)keyarray[i]);

                        // write out the attachments			             
                        ArrayList alist = (ArrayList)valuearray[i];

                        imawriter.Write((int)alist.Count);
                        foreach (XmlAttachment a in alist)
                        {
                            // write the attachment serial
                            imawriter.Write((int)a.Serial.Value);

                            // write the value type
                            imawriter.Write((string)a.GetType().ToString());

                            // save the fileposition index
                            fpiwriter.Write((long)imawriter.Position);
                        }
                    }
                }
                else
                { 
                    // no item attachments
                    imawriter.Write((int)0);
                }*/

				// Alan MOD
				// save the hash table info for items and mobiles
				// mobile attachments

				if (EntityAttachments != null)
				{
					imawriter.Write(EntityAttachments.Count);

					var valuearray = new ArrayList[EntityAttachments.Count];
					EntityAttachments.Values.CopyTo(valuearray, 0);

					var keyarray = new int[EntityAttachments.Count];
					EntityAttachments.Keys.CopyTo(keyarray, 0);

					for (int i = 0; i < keyarray.Length; i++)
					{
						// write the key
						imawriter.Write(keyarray[i]);

						// write out the attachments
						ArrayList alist = valuearray[i];

						imawriter.Write(alist.Count);
						foreach (XmlAttachment a in alist)
						{
							// write the attachment serial
							imawriter.Write(a.Serial.Value);

							// write the value type
							imawriter.Write(a.GetType().ToString());

							// save the fileposition index
							fpiwriter.Write(imawriter.Position);
						}
					}
				}
				else
				{
					// no mobile attachments
					imawriter.Write(0);
				}

				imawriter.Write(0);
				// pretend no item attachments and leave the deserialization as-is -- this way deserialization will still work with older saves -Alan
				// END ALAN MOD

				imawriter.Close();
				fpiwriter.Close();
			}
		}

		public static void Load()
		{
			string filePath = Path.Combine("Saves/Attachments", "Attachments.bin"); // the attachment serializations
			string imaPath = Path.Combine("Saves/Attachments", "Attachments.ima"); // the item/mob attachment tables
			string fpiPath = Path.Combine("Saves/Attachments", "Attachments.fpi"); // the file position indices

			if (!File.Exists(filePath))
			{
				return;
			}

			FileStream fs = null;
			BinaryFileReader reader = null;
			FileStream imafs = null;
			BinaryFileReader imareader = null;
			FileStream fpifs = null;
			BinaryFileReader fpireader = null;

			try
			{
				fs = new FileStream(filePath, (FileMode)3, (FileAccess)1, (FileShare)1);
				reader = new BinaryFileReader(new BinaryReader(fs));
				imafs = new FileStream(imaPath, (FileMode)3, (FileAccess)1, (FileShare)1);
				imareader = new BinaryFileReader(new BinaryReader(imafs));
				fpifs = new FileStream(fpiPath, (FileMode)3, (FileAccess)1, (FileShare)1);
				fpireader = new BinaryFileReader(new BinaryReader(fpifs));
			}
			catch (Exception e)
			{
				ErrorReporter.GenerateErrorReport(e.ToString());
				return;
			}

			if (reader != null && imareader != null && fpireader != null)
			{
				// restore the current global attachment serial state
				try
				{
					ASerial.GlobalDeserialize(reader);
				}
				catch (Exception e)
				{
					ErrorReporter.GenerateErrorReport(e.ToString());
					return;
				}

				ASerial.serialInitialized = true;

				// read in the serial attachment hash table information
				int count = 0;
				try
				{
					count = reader.ReadInt();
				}
				catch (Exception e)
				{
					ErrorReporter.GenerateErrorReport(e.ToString());
					return;
				}

				for (int i = 0; i < count; i++)
				{
					// read the serial
					ASerial serialno = null;
					try
					{
						serialno = new ASerial(reader.ReadInt());
					}
					catch (Exception e)
					{
						ErrorReporter.GenerateErrorReport(e.ToString());
						return;
					}

					// read the attachment type
					string valuetype = null;
					try
					{
						valuetype = reader.ReadString();
					}
					catch (Exception e)
					{
						ErrorReporter.GenerateErrorReport(e.ToString());
						return;
					}

					// read the position of the beginning of the next attachment deser within the .bin file
					long position = 0;
					try
					{
						position = fpireader.ReadLong();
					}
					catch (Exception e)
					{
						ErrorReporter.GenerateErrorReport(e.ToString());
						return;
					}

					bool skip = false;

					XmlAttachment o = null;
					try
					{
						o = (XmlAttachment)Activator.CreateInstance(Type.GetType(valuetype), new object[] {serialno});
					}
					catch
					{
						skip = true;
					}

					if (skip)
					{
						if (!AlreadyReported(valuetype))
						{
							Console.WriteLine("\nError deserializing attachments {0}.\nMissing a serial constructor?\n", valuetype);
							ReportDeserError(valuetype, "Missing a serial constructor?");
						}
						// position the .ima file at the next deser point
						try
						{
							reader.Seek(position, SeekOrigin.Begin);
						}
						catch
						{
							ErrorReporter.GenerateErrorReport(
								"Error deserializing. Attachments save file corrupted. Attachment load aborted.");
							return;
						}
						continue;
					}

					try
					{
						o.Deserialize(reader);
					}
					catch
					{
						skip = true;
					}

					// confirm the read position
					if (reader.Position != position || skip)
					{
						if (!AlreadyReported(valuetype))
						{
							Console.WriteLine("\nError deserializing attachments {0}\n", valuetype);
							ReportDeserError(valuetype, "save file corruption or incorrect Serialize/Deserialize methods?");
						}
						// position the .ima file at the next deser point
						try
						{
							reader.Seek(position, SeekOrigin.Begin);
						}
						catch
						{
							ErrorReporter.GenerateErrorReport(
								"Error deserializing. Attachments save file corrupted. Attachment load aborted.");
							return;
						}
						continue;
					}

					// add it to the hash table
					try
					{
						AllAttachments.Add(serialno.Value, o);
					}
					catch
					{
						ErrorReporter.GenerateErrorReport(
							String.Format(
								"\nError deserializing {0} serialno {1}. Attachments save file corrupted. Attachment load aborted.\n",
								valuetype,
								serialno.Value));
						return;
					}
				}

				// read in the mobile attachment hash table information
				try
				{
					count = imareader.ReadInt();
				}
				catch (Exception e)
				{
					ErrorReporter.GenerateErrorReport(e.ToString());
					return;
				}

				for (int i = 0; i < count; i++)
				{
					IEntity key = null;
					try
					{
						key = imareader.ReadEntity();
					}
					catch (Exception e)
					{
						ErrorReporter.GenerateErrorReport(e.ToString());
						return;
					}

					int nattach = 0;
					try
					{
						nattach = imareader.ReadInt();
					}
					catch (Exception e)
					{
						ErrorReporter.GenerateErrorReport(e.ToString());
						return;
					}

					for (int j = 0; j < nattach; j++)
					{
						// and serial
						ASerial serialno = null;
						try
						{
							serialno = new ASerial(imareader.ReadInt());
						}
						catch (Exception e)
						{
							ErrorReporter.GenerateErrorReport(e.ToString());
							return;
						}

						// read the attachment type
						string valuetype = null;
						try
						{
							valuetype = imareader.ReadString();
						}
						catch (Exception e)
						{
							ErrorReporter.GenerateErrorReport(e.ToString());
							return;
						}

						// read the position of the beginning of the next attachment deser within the .bin file
						long position = 0;
						try
						{
							position = fpireader.ReadLong();
						}
						catch (Exception e)
						{
							ErrorReporter.GenerateErrorReport(e.ToString());
							return;
						}

						XmlAttachment o = FindAttachmentBySerial(serialno.Value);

						if (o == null || imareader.Position != position)
						{
							if (!AlreadyReported(valuetype))
							{
								Console.WriteLine("\nError deserializing attachments of type {0}.\n", valuetype);
								ReportDeserError(valuetype, "save file corruption or incorrect Serialize/Deserialize methods?");
							}
							// position the .ima file at the next deser point
							try
							{
								imareader.Seek(position, SeekOrigin.Begin);
							}
							catch
							{
								ErrorReporter.GenerateErrorReport(
									"Error deserializing. Attachments save file corrupted. Attachment load aborted.");
								return;
							}
							continue;
						}

						// attachment successfully deserialized so attach it
						AttachTo(key, o, false);
					}
				}

				// read in the item attachment hash table information
				try
				{
					count = imareader.ReadInt();
				}
				catch (Exception e)
				{
					ErrorReporter.GenerateErrorReport(e.ToString());
					return;
				}

				for (int i = 0; i < count; i++)
				{
					Item key = null;
					try
					{
						key = imareader.ReadItem();
					}
					catch (Exception e)
					{
						ErrorReporter.GenerateErrorReport(e.ToString());
						return;
					}

					int nattach = 0;
					try
					{
						nattach = imareader.ReadInt();
					}
					catch (Exception e)
					{
						ErrorReporter.GenerateErrorReport(e.ToString());
						return;
					}

					for (int j = 0; j < nattach; j++)
					{
						// and serial
						ASerial serialno = null;
						try
						{
							serialno = new ASerial(imareader.ReadInt());
						}
						catch (Exception e)
						{
							ErrorReporter.GenerateErrorReport(e.ToString());
							return;
						}

						// read the attachment type
						string valuetype = null;
						try
						{
							valuetype = imareader.ReadString();
						}
						catch (Exception e)
						{
							ErrorReporter.GenerateErrorReport(e.ToString());
							return;
						}

						// read the position of the beginning of the next attachment deser within the .bin file
						long position = 0;
						try
						{
							position = fpireader.ReadLong();
						}
						catch (Exception e)
						{
							ErrorReporter.GenerateErrorReport(e.ToString());
							return;
						}

						XmlAttachment o = FindAttachmentBySerial(serialno.Value);

						if (o == null || imareader.Position != position)
						{
							if (!AlreadyReported(valuetype))
							{
								Console.WriteLine("\nError deserializing attachments of type {0}.\n", valuetype);
								ReportDeserError(valuetype, "save file corruption or incorrect Serialize/Deserialize methods?");
							}
							// position the .ima file at the next deser point
							try
							{
								imareader.Seek(position, SeekOrigin.Begin);
							}
							catch
							{
								ErrorReporter.GenerateErrorReport(
									"Error deserializing. Attachments save file corrupted. Attachment load aborted.");
								return;
							}
							continue;
						}

						// attachment successfully deserialized so attach it
						AttachTo(key, o, false);
					}
				}
				if (fs != null)
				{
					fs.Close();
				}
				if (imafs != null)
				{
					imafs.Close();
				}
				if (fpifs != null)
				{
					fpifs.Close();
				}

				if (desererror != null)
				{
					ErrorReporter.GenerateErrorReport("Error deserializing particular attachments.");
				}
			}
		}

		private class DeserErrorDetails
		{
			public readonly string Type;
			public readonly string Details;

			public DeserErrorDetails(string type, string details)
			{
				Type = type;
				Details = details;
			}
		}

		private static ArrayList desererror;

		private static void ReportDeserError(string typestr, string detailstr)
		{
			if (desererror == null)
			{
				desererror = new ArrayList();
			}

			desererror.Add(new DeserErrorDetails(typestr, detailstr));
		}

		private static bool AlreadyReported(string typestr)
		{
			if (desererror == null)
			{
				return false;
			}
			foreach (DeserErrorDetails s in desererror)
			{
				if (s.Type == typestr)
				{
					return true;
				}
			}
			return false;
		}

		public static void CheckOnBeforeKill(Mobile m_killed, Mobile m_killer)
		{
			// do not register creature vs creature kills, nor any kills involving staff
			//            if (m_killer == null || m_killed == null || !(m_killer.Player || m_killed.Player) /*|| (m_killer.AccessLevel > AccessLevel.Player) || (m_killed.AccessLevel > AccessLevel.Player) */)
			//				return;

			if (m_killer != null)
			{
				// check the killer
				ArrayList alist = FindAttachments(m_killer);
				if (alist != null)
				{
					foreach (XmlAttachment a in alist)
					{
						if (a != null && !a.Deleted && a.HandlesOnKill)
						{
							a.OnBeforeKill(m_killed, m_killer);
						}
					}
				}

				// check any equipped items
				var equiplist = m_killer.Items;
				if (equiplist != null)
				{
					foreach (Item i in equiplist)
					{
						if (i == null || i.Deleted)
						{
							continue;
						}
						alist = FindAttachments(i);
						if (alist != null)
						{
							foreach (XmlAttachment a in alist)
							{
								if (a != null && !a.Deleted && a.CanActivateEquipped && a.HandlesOnKill)
								{
									a.OnBeforeKill(m_killed, m_killer);
								}
							}
						}
					}
				}
			}

			if (m_killed != null)
			{
				// check the killed
				ArrayList alist = FindAttachments(m_killed);
				if (alist != null)
				{
					foreach (XmlAttachment a in alist)
					{
						if (a != null && !a.Deleted && a.HandlesOnKilled)
						{
							a.OnBeforeKilled(m_killed, m_killer);
						}
					}
				}
			}
		}

		public static List<XmlScript> GetScripts(IEntity m)
		{
			if (m == null)
			{
				return null;
			}
			ArrayList alist = FindAttachments(m);
			if (alist == null || alist.Count == 0)
			{
				return null;
			}
			var output = new List<XmlScript>();
			foreach (Object xmlattachment in alist)
			{
				if (xmlattachment is XmlScript)
				{
					output.Add(xmlattachment as XmlScript);
				}
			}
			if (output.Count > 0)
			{
				return output;
			}
			return null;
		}

		public static List<XmlTeam> GetTeams(IEntity m)
		{
			if (m == null)
			{
				return null;
			}
			ArrayList alist = FindAttachments(m);
			if (alist == null || alist.Count == 0)
			{
				return null;
			}
			var output = new List<XmlTeam>();
			foreach (Object xmlattachment in alist)
			{
				if (xmlattachment is XmlTeam && !((XmlTeam)xmlattachment).Deleted)
				{
					output.Add(xmlattachment as XmlTeam);
				}
			}
			if (output.Count == 0)
			{
				return null;
			}
			return output;
		}

		public static List<XmlSlayer> GetSlayerAttachments(IEntity m)
		{
			if (m == null)
			{
				return null;
			}
			ArrayList alist = FindAttachments(m);
			if (alist == null || alist.Count == 0)
			{
				return null;
			}
			var output = new List<XmlSlayer>();
			foreach (Object xmlattachment in alist)
			{
				if (xmlattachment is XmlSlayer)
				{
					output.Add(xmlattachment as XmlSlayer);
				}
			}
			if (output.Count > 0)
			{
				return output;
			}
			return null;
		}

		/*
        public static List<XmlTeam> GetHighestPriorityTeams(Object m)
        {
            ArrayList alist = XmlAttach.FindAttachments(m);
            if (alist == null || alist.Count == 0) { return null; }
            List<XmlTeam> output = new List<XmlTeam>();
            int currentTopPriority = 0;
            foreach (Object xmlattachment in alist)
            {
                if (xmlattachment is XmlTeam)
                {
                    XmlTeam xmlTeam = (XmlTeam)xmlattachment;
                }
            }
            return output;
        }
         */

		public static void CheckOnKill(Mobile m_killed, Mobile m_killer)
		{
			// do not register creature vs creature kills, nor any kills involving staff
			//            if (m_killer == null || m_killed == null || !(m_killer.Player || m_killed.Player) /*|| (m_killer.AccessLevel > AccessLevel.Player) || (m_killed.AccessLevel > AccessLevel.Player) */)
			//				return;

			if (m_killer != null)
			{
				// check the killer
				ArrayList alist = FindAttachments(m_killer);
				if (alist != null)
				{
					foreach (XmlAttachment a in alist)
					{
						if (a != null && !a.Deleted && a.HandlesOnKill)
						{
							a.OnKill(m_killed, m_killer);
						}
					}
				}

				// check any equipped items
				var equiplist = m_killer.Items;
				if (equiplist != null)
				{
					foreach (Item i in equiplist)
					{
						if (i == null || i.Deleted)
						{
							continue;
						}
						alist = FindAttachments(i);
						if (alist != null)
						{
							foreach (XmlAttachment a in alist)
							{
								if (a != null && !a.Deleted && a.CanActivateEquipped && a.HandlesOnKill)
								{
									a.OnKill(m_killed, m_killer);
								}
							}
						}
					}
				}
			}

			if (m_killed != null)
			{
				// check the killed
				ArrayList alist = FindAttachments(m_killed);
				if (alist != null)
				{
					foreach (XmlAttachment a in alist)
					{
						if (a != null && !a.Deleted && a.HandlesOnKilled)
						{
							a.OnKilled(m_killed, m_killer);
						}
					}
				}
			}
		}

		public static XmlValue GetValueAttachment(IEntity o, string name)
		{
			if (o != null)
			{
				ArrayList alist = FindAttachments(o);
				if (alist != null)
				{
					foreach (XmlAttachment a in alist)
					{
						if (a != null && !a.Deleted && a is XmlValue && a.Name == name)
						{
							return (XmlValue)a;
						}
					}
				}
			}
			return null;
		}

		public static XmlLocalVariable GetStringAttachment(IEntity o, string name)
		{
			if (o != null)
			{
				ArrayList alist = FindAttachments(o);
				if (alist != null)
				{
					foreach (XmlAttachment a in alist)
					{
						if (a != null && !a.Deleted && a is XmlLocalVariable && a.Name == name)
						{
							return (XmlLocalVariable)a;
						}
					}
				}
			}
			return null;
		}

		public static XmlDouble GetDoubleAttachment(IEntity o, string name)
		{
			if (o == null)
			{
				return null;
			}
			ArrayList alist = FindAttachments(o);
			if (alist != null)
			{
				foreach (XmlAttachment a in alist)
				{
					if (a != null && !a.Deleted && a is XmlDouble && a.Name == name)
					{
						return (XmlDouble)a;
					}
				}
			}
			return null;
		}

		public static XmlObject GetObjectAttachment(IEntity o, string name)
		{
			if (o == null)
			{
				return null;
			}
			ArrayList alist = FindAttachments(o);
			if (alist != null)
			{
				foreach (XmlAttachment a in alist)
				{
					if (a != null && !a.Deleted && a is XmlObject && a.Name == name)
					{
						return (XmlObject)a;
					}
				}
			}
			return null;
		}

		public static void CheckOnHit(Mobile m_hit, Mobile m_hitter)
		{
			// do not register creature vs creature kills, nor any kills involving staff
			//            if (m_hitter == null || m_hitter == null || !(m_hitter.Player || m_hit.Player) /*|| (m_hitter.AccessLevel > AccessLevel.Player) || (m_hit.AccessLevel > AccessLevel.Player) */)
			//				return;

			if (m_hit != null)
			{
				// check the killer
				ArrayList alist = FindAttachments(m_hit);
				if (alist != null)
				{
					foreach (XmlAttachment a in alist)
					{
						if (a != null && !a.Deleted && a is XmlOnHit)
						{
							((XmlOnHit)a).OnHit(m_hit, m_hitter);
						}
					}
				}
			}
		}

		public static void EventSink_Movement(MovementEventArgs args)
		{
			Mobile from = args.Mobile;

			if (!from.Player /* || from.AccessLevel > AccessLevel.Player */)
			{
				return;
			}

			// check for any items in the same sector
			if (from.Map != null)
			{
				IPooledEnumerable itemlist = from.Map.GetItemsInRange(from.Location, Map.SectorSize);
				if (itemlist != null)
				{
					foreach (Item i in itemlist)
					{
						if (i == null || i.Deleted)
						{
							continue;
						}

						ArrayList alist = FindAttachments(i);
						if (alist != null)
						{
							foreach (XmlAttachment a in alist)
							{
								if (a != null && !a.Deleted && a.HandlesOnMovement)
								{
									a.OnMovement(args);
								}
							}
						}
					}
					itemlist.Free();
				}

				// check for mobiles
				IPooledEnumerable moblist = from.Map.GetMobilesInRange(from.Location, Map.SectorSize);
				if (moblist != null)
				{
					foreach (Mobile i in moblist)
					{
						// dont respond to self motion
						if (i == null || i.Deleted || i == from)
						{
							continue;
						}

						ArrayList alist = FindAttachments(i);
						if (alist != null)
						{
							foreach (XmlAttachment a in alist)
							{
								if (a != null && !a.Deleted && a.HandlesOnMovement)
								{
									a.OnMovement(args);
								}
							}
						}
					}
					moblist.Free();
				}
			}
		}

		public static void EventSink_Speech(SpeechEventArgs args)
		{
			Mobile from = args.Mobile;

			if (from == null || from.Map == null /*|| from.AccessLevel > AccessLevel.Player */)
			{
				return;
			}

			// check the mob for any attachments that might handle speech
			ArrayList alist = FindAttachments(from);
			if (alist != null)
			{
				foreach (XmlAttachment a in alist)
				{
					if (a != null && !a.Deleted && a.HandlesOnSpeech)
					{
						a.OnSpeech(args);
					}
				}
			}

			// check for any nearby items
			IPooledEnumerable itemlist = from.Map.GetItemsInRange(from.Location, Map.SectorSize);
			if (itemlist != null)
			{
				foreach (Item i in itemlist)
				{
					if (i == null || i.Deleted)
					{
						continue;
					}

					alist = FindAttachments(i);
					if (alist != null)
					{
						foreach (XmlAttachment a in alist)
						{
							if (a != null && !a.Deleted && a.CanActivateInWorld && a.HandlesOnSpeech)
							{
								a.OnSpeech(args);
							}
						}
					}
				}
				itemlist.Free();
			}

			// check for any nearby mobs
			IPooledEnumerable moblist = from.Map.GetMobilesInRange(from.Location, Map.SectorSize);
			if (moblist != null)
			{
				foreach (Mobile i in moblist)
				{
					if (i == null || i.Deleted)
					{
						continue;
					}

					alist = FindAttachments(i);
					if (alist != null)
					{
						foreach (XmlAttachment a in alist)
						{
							if (a != null && !a.Deleted && a.HandlesOnSpeech)
							{
								a.OnSpeech(args);
							}
						}
					}
				}
				moblist.Free();
			}

			// also check for any items in the mobs toplevel backpack
			if (from.Backpack != null)
			{
				var packlist = from.Backpack.Items;
				if (packlist != null)
				{
					foreach (Item i in packlist)
					{
						if (i == null || i.Deleted)
						{
							continue;
						}
						alist = FindAttachments(i);
						if (alist != null)
						{
							foreach (XmlAttachment a in alist)
							{
								if (a != null && !a.Deleted && a.CanActivateInBackpack && a.HandlesOnSpeech)
								{
									a.OnSpeech(args);
								}
							}
						}
					}
				}
			}

			// check any equipped items
			var equiplist = from.Items;
			if (equiplist != null)
			{
				foreach (Item i in equiplist)
				{
					if (i == null || i.Deleted)
					{
						continue;
					}
					alist = FindAttachments(i);
					if (alist != null)
					{
						foreach (XmlAttachment a in alist)
						{
							if (a != null && !a.Deleted && a.CanActivateEquipped && a.HandlesOnSpeech)
							{
								a.OnSpeech(args);
							}
						}
					}
				}
			}
		}

		public static XmlAttachment FindAttachmentOnMobile(Mobile from, Type type, string name)
		{
			if (from == null)
			{
				return null;
			}
			// check the mob for any attachments
			ArrayList alist = FindAttachments(from);
			if (alist != null)
			{
				foreach (XmlAttachment a in alist)
				{
					if (a != null && !a.Deleted && (type == null || (a.GetType() == type || a.GetType().IsSubclassOf(type))) &&
						(name == null || name == a.Name))
					{
						return a;
					}
				}
			}

			// also check for any items in the mobs toplevel backpack
			if (from.Backpack != null)
			{
				var itemlist = from.Backpack.Items;
				if (itemlist != null)
				{
					foreach (Item i in itemlist)
					{
						if (i == null || i.Deleted)
						{
							continue;
						}
						alist = FindAttachments(i);
						if (alist != null)
						{
							foreach (XmlAttachment a in alist)
							{
								if (a != null && !a.Deleted && (type == null || (a.GetType() == type || a.GetType().IsSubclassOf(type))) &&
									(name == null || name == a.Name))
								{
									return a;
								}
							}
						}
					}
				}
			}

			// check any equipped items
			var equiplist = from.Items;
			if (equiplist != null)
			{
				foreach (Item i in equiplist)
				{
					if (i == null || i.Deleted)
					{
						continue;
					}

					alist = FindAttachments(i);

					if (alist != null)
					{
						foreach (XmlAttachment a in alist)
						{
							if (a != null && !a.Deleted && (type == null || (a.GetType() == type || a.GetType().IsSubclassOf(type))) &&
								(name == null || name == a.Name))
							{
								return a;
							}
						}
					}
				}
			}
			return null;
		}

		private class AttachTarget : Target
		{
			private readonly CommandEventArgs m_e;
			private readonly string m_set;

			public AttachTarget(CommandEventArgs e, string set)
				: base(30, false, TargetFlags.None)
			{
				m_e = e;
				m_set = set;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				IEntity entity = targeted as IEntity;
				if (from == null || entity == null)
				{
					return;
				}

				Type type = null;
				string name = null;

				if (m_e.Arguments.Length > 0)
				{
					type = SpawnerType.GetType(m_e.Arguments[0]);
				}
				if (m_e.Arguments.Length > 1)
				{
					name = m_e.Arguments[1];
				}

				Defrag(entity.Serial);

				ArrayList plist = FindAttachments(targeted as IEntity, type);

				if (plist == null && m_set != "add")
				{
					from.SendMessage("No attachments");
					return;
				}

				switch (m_set)
				{
					case "add":

						if (m_e.Arguments.Length < 1)
						{
							from.SendMessage("Must specify an attachment type.");
							return;
						}

						// create a new attachment and add it to the item
						int nargs = m_e.Arguments.Length - 1;

						var args = new string[nargs];

						for (int j = 0; j < nargs; j++)
						{
							args[j] = m_e.Arguments[j + 1];
						}

						XmlAttachment o = null;

						Type attachtype = SpawnerType.GetType(m_e.Arguments[0]);

						if (attachtype != null && attachtype.IsSubclassOf(typeof(XmlAttachment)))
						{
							o = (XmlAttachment)XmlSpawner.CreateObject(attachtype, args, false, true);
						}

						if (o != null)
						{
							//o.Name = aname;
							if (AttachTo(from, targeted as IEntity, o, true))
							{
								from.SendMessage("Added attachment {2} : {0} to {1}", m_e.Arguments[0], targeted, o.Serial.Value);
							}
							else
							{
								from.SendMessage("Attachment not added: {0}", m_e.Arguments[0]);
							}
						}
						else
						{
							from.SendMessage("Unable to construct attachment {0}", m_e.Arguments[0]);
						}

						break;
					case "get":
						/*
                            foreach(XmlAttachment p in plist)
                            {
                                if(p == null || p.Deleted || (name != null && name != p.Name) || (type != null && type != p.GetType())) continue;

                                from.SendMessage("Found attachment {3} : {0} : {1} : {2}",p.GetType().Name,p.Name,p.OnIdentify(from), p.Serial.Value);

                            }
                            */
						from.SendGump(new XmlGetAttGump(from, targeted, 0, 0));

						break;
					case "delete":
						/*
                            foreach(XmlAttachment p in plist)
                            {
                                if(p == null || p.Deleted || (name != null && name != p.Name) || (type != null && type != p.GetType())) continue;

                                from.SendMessage("Deleting attachment {3} : {0} : {1} : {2}",p.GetType().Name,p.Name,p.OnIdentify(from), p.Serial.Value);
                                p.Delete();
                            }
                            */
						from.SendGump(new XmlGetAttGump(from, targeted, 0, 0));

						break;
					case "activate":
						foreach (XmlAttachment p in plist)
						{
							if (p == null || p.Deleted || (name != null && name != p.Name) || (type != null && type != p.GetType()))
							{
								continue;
							}

							from.SendMessage(
								"Activating attachment {3} : {0} : {1} : {2}", p.GetType().Name, p.Name, p.OnIdentify(from), p.Serial.Value);
							p.OnTrigger(null, from);
						}

						break;
				}
			}
		}

		[Usage("GetAtt [type/serialno [name]]")]
		[Description("Returns descriptions of the attachments on the targeted object.")]
		public static void GetAttachments_OnCommand(CommandEventArgs e)
		{
			int ser = -1;
			if (e.Arguments.Length > 0)
			{
				// is this a numeric arg?
				char c = e.Arguments[0][0];
				if (c >= '0' && c <= '9')
				{
					try
					{
						ser = int.Parse(e.Arguments[0]);
					}
					catch
					{ }
					XmlAttachment a = FindAttachmentBySerial(ser);
					if (a != null)
					{
						// open up the props gump on the attachment
						e.Mobile.SendGump(new PropertiesGump(e.Mobile, a));
					}
					else
					{
						e.Mobile.SendMessage("Attachment {0} does not exist", ser);
					}
				}
			}

			if (ser == -1)
			{
				e.Mobile.Target = new AttachTarget(e, "get");
			}
		}

		[Usage("AddAtt type [args]")]
		[Description("Adds an attachment to the targeted object.")]
		public static void AddAttachment_OnCommand(CommandEventArgs e)
		{
			e.Mobile.Target = new AttachTarget(e, "add");
		}

		[Usage("DelAtt [type/serialno [name]]")]
		[Description("Deletes attachments on the targeted object.")]
		public static void DeleteAttachments_OnCommand(CommandEventArgs e)
		{
			int ser = -1;
			if (e.Arguments.Length > 0)
			{
				// is this a numeric arg?
				char c = e.Arguments[0][0];
				if (c >= '0' && c <= '9')
				{
					try
					{
						ser = int.Parse(e.Arguments[0]);
					}
					catch
					{ }
					XmlAttachment a = FindAttachmentBySerial(ser);
					if (a != null)
					{
						e.Mobile.SendMessage("Deleting attachment {0} : {1}", ser, a);
						a.Delete();
					}
					else
					{
						e.Mobile.SendMessage("Attachment {0} does not exist", ser);
					}
				}
			}

			if (ser == -1)
			{
				e.Mobile.Target = new AttachTarget(e, "delete");
			}
		}

		[Usage("TrigAtt [type [name]]")]
		[Description("Triggers attachments on the targeted object.")]
		public static void ActivateAttachments_OnCommand(CommandEventArgs e)
		{
			e.Mobile.Target = new AttachTarget(e, "activate");
		}

		/* // not really needed with Xml Find (AND I moved everything into a single IEntity dictionary) - Alan
        [Usage("ItemAtt")]
        [Description("Lists all item attachments.")]
        public static void ListItemAttachments_OnCommand(CommandEventArgs e)
        {
            if (ItemAttachments == null) return;

            XmlAttach.FullDefrag(ItemAttachments);

            Item[] itemarray = new Item[ItemAttachments.Count];

            ItemAttachments.Keys.CopyTo(itemarray, 0);

            e.Mobile.SendMessage("{0} items with attachments", ItemAttachments.Count);

            for (int i = 0; i < itemarray.Length; i++)
            {
                e.Mobile.SendMessage("Attachments for {0} :", itemarray[i]);
                ArrayList list = FindAttachments(itemarray[i]);

                if (list != null)
                {
                    foreach (XmlAttachment a in list)
                    {
                        if (a != null && !a.Deleted)
                            e.Mobile.SendMessage("\t{0} : {1} : {2}", a.GetType().Name, a.Name, a.OnIdentify(e.Mobile));
                    }
                }
            }
        }
        [Usage("MobAtt")]
        [Description("Lists all mobile attachments.")]
        public static void ListMobileAttachments_OnCommand(CommandEventArgs e)
        {
            if (MobileAttachments == null) return;

            XmlAttach.FullDefrag(MobileAttachments);

            Mobile[] mobilearray = new Mobile[MobileAttachments.Count];

            MobileAttachments.Keys.CopyTo(mobilearray, 0);

            e.Mobile.SendMessage("{0} mobiles with attachments", MobileAttachments.Count);

            for (int i = 0; i < mobilearray.Length; i++)
            {
                e.Mobile.SendMessage("Attachments for {0} :", mobilearray[i]);
                ArrayList list = FindAttachments(mobilearray[i]);

                if (list != null)
                {
                    foreach (XmlAttachment a in list)
                    {
                        if (a != null && !a.Deleted)
                            e.Mobile.SendMessage("\t{0} : {1} : {2}", a.GetType().Name, a.Name, a.OnIdentify(e.Mobile));
                    }
                }
            }
        }
         * */

		private static void Match(Type matchtype, Type[] types, ArrayList results)
		{
			if (matchtype == null)
			{
				return;
			}

			for (int i = 0; i < types.Length; ++i)
			{
				Type t = types[i];

				if (t.IsSubclassOf(matchtype))
				{
					results.Add(t);
				}
			}
		}

		private static ArrayList Match(Type matchtype)
		{
			ArrayList results = new ArrayList();
			Type[] types;

			var asms = ScriptCompiler.Assemblies;

			for (int i = 0; i < asms.Length; ++i)
			{
				types = ScriptCompiler.GetTypeCache(asms[i]).Types;
				Match(matchtype, types, results);
			}

			types = ScriptCompiler.GetTypeCache(Core.Assembly).Types;
			Match(matchtype, types, results);

			results.Sort(new TypeNameComparer());

			return results;
		}

		private class TypeNameComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				Type a = x as Type;
				Type b = y as Type;

				return a.Name.CompareTo(b.Name);
			}
		}

		[Usage("AvailAtt")]
		[Description("Lists all available attachments.")]
		public static void ListAvailableAttachments_OnCommand(CommandEventArgs e)
		{
			ArrayList attachtypes = Match(typeof(XmlAttachment));

			string parmliststr = null;

			foreach (Type attachtype in attachtypes)
			{
				// get all constructors derived from the XmlAttachment class
				var ctors = attachtype.GetConstructors();

				for (int i = 0; i < ctors.Length; ++i)
				{
					ConstructorInfo ctor = ctors[i];

					if (!IsAttachable(ctor))
					{
						continue;
					}

					var paramList = ctor.GetParameters();

					if (paramList != null)
					{
						string parms = attachtype.Name;

						for (int j = 0; j < paramList.Length; j++)
						{
							parms += ", " + paramList[j].Name;
						}

						parmliststr += parms + "\n";
					}
				}
			}
			e.Mobile.SendGump(new ListAttachmentsGump(parmliststr, 20, 20));
		}

		private class ListAttachmentsGump : Gump
		{
			public ListAttachmentsGump(string attachmentlist, int X, int Y)
				: base(X, Y)
			{
				AddPage(0);

				AddBackground(20, 0, 330, 480, 5054);

				AddPage(1);

				AddImageTiled(20, 0, 330, 480, 0x52);

				AddLabel(27, 2, 0x384, "Available Attachments");
				AddHtml(25, 22, 320, 458, attachmentlist, false, true);
			}
		}

		private class DisplayAttachmentGump : Gump
		{
			public DisplayAttachmentGump(Mobile from, string text, int X, int Y)
				: base(X, Y)
			{
				// prepare the page
				AddPage(0);

				AddBackground(0, 0, 400, 150, 5054);
				AddAlphaRegion(0, 0, 400, 150);
				AddLabel(20, 2, 55, "Attachment Description(s)");

				AddHtml(20, 20, 360, 110, text, true, true);
			}
		}

		public static void RevealAttachments(Mobile from, IEntity o)
		{
			if (from == null || o == null)
			{
				return;
			}

			ArrayList plist = FindAttachments(o);

			if (plist == null)
			{
				return;
			}

			string msg = null;

			foreach (XmlAttachment p in plist)
			{
				if (p != null && !p.Deleted)
				{
					string pmsg = p.OnIdentify(from);
					if (pmsg != null)
					{
						msg += String.Format("\n{0}\n", pmsg);
					}
				}
			}
			if (msg != null)
			{
				from.CloseGump(typeof(DisplayAttachmentGump));
				from.SendMessage("Hidden attributes revealed!");

				from.SendGump(new DisplayAttachmentGump(from, msg, 0, 0));
			}
		}

		public static bool AttachTo(IEntity o, XmlAttachment attachment)
		{
			return AttachTo(null, o, attachment, true);
		}

		public static bool AttachTo(object from, IEntity o, XmlAttachment attachment)
		{
			return AttachTo(from, o, attachment, true);
		}

		public static bool AttachTo(IEntity o, XmlAttachment attachment, bool first)
		{
			return AttachTo(null, o, attachment, first);
		}

		private static bool AttachTo(object from, IEntity o, XmlAttachment attachment, bool first)
		{
			if (o == null || attachment == null)
			{
				return false;
			}

			if (EntityAttachments == null)
			{
				EntityAttachments = new Dictionary<int, ArrayList>();
			}

			if (o is AddonComponent)
			{
				// add the attachment to the parent addon instead
				AddonComponent component = (AddonComponent)o;
				if (component.Addon != null)
				{
					o = component.Addon;
				}
			}

			Defrag(o.Serial);

			// see if there is already an attachment list for the object
			ArrayList attachmententry = FindAttachments(o, true);

			if (attachmententry != null)
			{
				// if an existing entry list was found then just add the attachment to that list after making sure there is not a duplicate
				foreach (XmlAttachment i in attachmententry)
				{
					// and attachment is considered a duplicate if both the type and name match
					if (i != null && !i.Deleted && i.GetType() == attachment.GetType() && i.Name == attachment.Name)
					{
						// duplicate found so replace it
						i.Delete();
					}
				}

				attachmententry.Add(attachment);
			}
			else
			{
				// otherwise make a new entry list
				attachmententry = new ArrayList(1);

				// containing the attachment
				attachmententry.Add(attachment);

				// and add it to the hash table
				EntityAttachments.Add(o.Serial.Value, attachmententry);
			}

			attachment.AttachedTo = o;
			attachment.OwnedBy = o;

			if (from is Mobile)
			{
				attachment.SetAttachedBy(((Mobile)from).Name);
			}
			else if (from is Item)
			{
				attachment.SetAttachedBy(((Item)from).Name);
			}

			// if this is being attached for the first time, then call the OnAttach method
			// if it is being reattached due to deserialization then dont
			if (first)
			{
				attachment.OnAttach();
			}
			else
			{
				attachment.OnReattach();
			}

			return !attachment.Deleted;
		}

		public static ArrayList FindAttachments(IEntity o)
		{
			return FindAttachments(o, null, null, false);
		}

		public static ArrayList FindAttachments(IEntity o, bool original)
		{
			return FindAttachments(o, null, null, original);
		}

		public static ArrayList FindAttachments(IEntity o, Type type)
		{
			return FindAttachments(o, type, null, false);
		}

		public static ArrayList FindAttachments(IEntity o, Type type, string name)
		{
			return FindAttachments(o, type, name, false);
		}

		public static ArrayList FindAttachments(IEntity o, Type type, string name, bool original)
		{
			if (o == null || EntityAttachments == null)
			{
				return null;
			}

			if (o.Deleted)
			{
				return null;
			}

			ArrayList output;
			if (type == null && name == null)
			{
				EntityAttachments.TryGetValue(o.Serial.Value, out output);
				if (output != null)
				{
					if (original)
					{
						return output;
					}
					else
					{
						return (ArrayList)output.Clone();
					}
				}
				else
				{
					return null;
				}
			}
			else
			{
				// just get those of a particular type and/or name
				EntityAttachments.TryGetValue(o.Serial.Value, out output);
				if (output != null)
				{
					ArrayList newlist = new ArrayList();

					foreach (XmlAttachment i in output)
					{
						// see if it is deleted
						if (i == null || i.Deleted)
						{
							continue;
						}

						Type itype = i.GetType();

						if ((type == null || (itype != null && (itype == type || itype.IsSubclassOf(type)))) &&
							(name == null || (name == i.Name)))
						{
							newlist.Add(i);
						}
					}

					return newlist;
				}

				return null;
			}
		}

		public static XmlAttachment FindAttachment(IEntity o)
		{
			return FindAttachment(o, null, null);
		}

		public static XmlAttachment FindAttachment(IEntity o, Type type)
		{
			return FindAttachment(o, type, null);
		}

		public static XmlAttachment FindAttachment(IEntity o, Type type, string name)
		{
			if (o == null || EntityAttachments == null)
			{
				return null;
			}

			if (o.Deleted)
			{
				return null;
			}

			ArrayList list;
			EntityAttachments.TryGetValue(o.Serial.Value, out list);

			if (type == null && name == null)
			{
				if (list != null && list.Count > 0)
				{
					// return the first valid attachment
					foreach (XmlAttachment i in list)
					{
						if (i != null && !i.Deleted)
						{
							return i;
						}
					}
				}
			}
			else
			{
				// just get those of a particular type and/or name
				if (list != null)
				{
					foreach (XmlAttachment i in list)
					{
						// see if it is deleted
						if (i == null || i.Deleted)
						{
							continue;
						}

						Type itype = i.GetType();

						if ((type == null || (itype != null && (itype == type || itype.IsSubclassOf(type)))) &&
							(name == null || (name == i.Name)))
						{
							return i;
						}
					}
				}
			}
			return null;
		}

		public static XmlAttachment FindAttachmentBySerial(int serialno)
		{
			if (serialno <= 0)
			{
				return null;
			}
			XmlAttachment a;
			AllAttachments.TryGetValue(serialno, out a);
			return a;
		}

		private static void FullDefrag()
		{
			// defrag the mobile/item tables
			//FullDefrag(ItemAttachments);
			//FullDefrag(MobileAttachments);
			FullEntityDefrag();

			// defrag the serial table
			FullSerialDefrag();
		}

		private static void FullEntityDefrag()
		{
			// go through the item attachments
			var keyarray = new int[EntityAttachments.Count];

			EntityAttachments.Keys.CopyTo(keyarray, 0);
			for (int i = 0; i < keyarray.Length; i++)
			{
				Defrag(keyarray[i]);
			}
		}

		private static void FullSerialDefrag()
		{
			// go through the item attachments
			var keyarray = new int[AllAttachments.Count];

			AllAttachments.Keys.CopyTo(keyarray, 0);
			for (int i = 0; i < keyarray.Length; i++)
			{
				object o = AllAttachments[keyarray[i]];
				if (o is XmlAttachment)
				{
					XmlAttachment a = o as XmlAttachment;

					if (a == null || a.Deleted)
					{
						AllAttachments.Remove(keyarray[i]);
					}
				}
			}
		}

		private static void SerialDefrag(XmlAttachment a)
		{
			if (a != null && a.Deleted)
			{
				AllAttachments.Remove(a.Serial.Value);
			}
		}

		private static void Defrag(Serial serial)
		{
			if (serial == null || EntityAttachments == null)
			{
				return;
			}
			IEntity entity = World.FindEntity(serial);

			bool removeall = false;

			if (entity == null || entity.Deleted)
			{
				removeall = true;
			}

			// lookup the attachments for the given object
			ArrayList list;
			EntityAttachments.TryGetValue(serial.Value, out list);

			ArrayList defraglist = null;

			if (list != null)
			{
				foreach (XmlAttachment i in list)
				{
					// see if it is deleted
					if (i == null || i.Deleted || removeall)
					{
						// then flag for removal from the original list
						if (defraglist == null)
						{
							defraglist = new ArrayList();
						}

						defraglist.Add(i);
					}
				}

				if (defraglist != null)
				{
					foreach (XmlAttachment i in defraglist)
					{
						list.Remove(i);
					}
					// if the list is empty then remove the hashtable entry for the given object
					if (list.Count == 0 || removeall)
					{
						EntityAttachments.Remove(serial);
					}
				}
			}
			else
			{
				EntityAttachments.Remove(serial);
			}
		}

		public static bool CheckCanEquip(Item item, Mobile from)
		{
			// call the CanEquip method on any attachments on the item
			// look for attachments on the item
			ArrayList attachments = FindAttachments(item);

			if (attachments != null)
			{
				foreach (XmlAttachment a in attachments)
				{
					if (a != null && !a.Deleted)
					{
						if (!a.CanEquip(from))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public static void CheckOnEquip(Item item, Mobile from)
		{
			// look for attachments on the item
			ArrayList attachments = FindAttachments(item);

			if (attachments != null)
			{
				foreach (XmlAttachment a in attachments)
				{
					if (a != null && !a.Deleted)
					{
						a.OnEquip(from);
					}
				}
			}
		}

		public static void CheckOnRemoved(Item item, object parent)
		{
			// look for attachments on the item
			ArrayList attachments = FindAttachments(item);

			if (attachments != null)
			{
				foreach (XmlAttachment a in attachments)
				{
					if (a != null && !a.Deleted)
					{
						a.OnRemoved(parent);
					}
				}
			}
		}

		public static void OnWeaponHit(BaseWeapon weapon, Mobile attacker, Mobile defender, int damage)
		{
			// look for attachments on the weapon
			ArrayList attachments = FindAttachments(weapon);

			if (attachments != null)
			{
				foreach (XmlAttachment a in attachments)
				{
					if (a != null && !a.Deleted)
					{
						a.OnWeaponHit(attacker, defender, weapon, damage);
					}
				}
			}

			// also support OnWeaponHit for the mobile owner
			attachments = FindAttachments(attacker);

			if (attachments != null)
			{
				foreach (XmlAttachment a in attachments)
				{
					if (a != null && !a.Deleted)
					{
						a.OnWeaponHit(attacker, defender, weapon, damage);
					}
				}
			}
		}

		public static int OnArmorHit(Mobile attacker, Mobile defender, Item armor, BaseWeapon weapon, int damage)
		{
			int damageTaken = 0;

			// figure out who the attacker and defender are based upon who is carrying the armor/weapon

			// look for attachments on the armor
			if (armor != null)
			{
				ArrayList attachments = FindAttachments(armor);

				if (attachments != null)
				{
					foreach (XmlAttachment a in attachments)
					{
						if (a != null && !a.Deleted)
						{
							damageTaken += a.OnArmorHit(attacker, defender, armor, weapon, damage);
						}
					}
				}
			}

			return damageTaken;
		}

		public static void AddAttachmentProperties(IEntity parent, ObjectPropertyList list)
		{
			if (parent == null)
			{
				return;
			}

			string propstr = null;

			ArrayList plist = FindAttachments(parent);
			if (plist != null && plist.Count > 0)
			{
				for (int i = 0; i < plist.Count; i++)
				{
					XmlAttachment a = plist[i] as XmlAttachment;

					if (a != null && !a.Deleted)
					{
						// give the attachment an opportunity to modify the properties list of the parent
						a.AddProperties(list);

						// get any displayed properties on the attachment
						string str = a.DisplayedProperties(null);

						if (str != null)
						{
							propstr += str;

							if (i < plist.Count - 1)
							{
								propstr += "\n";
							}
						}
					}
				}
			}

			if (propstr != null && list != null)
			{
				list.Add(1062613, propstr);
			}
		}

		public static void UseReq(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;

			if (from.AccessLevel >= AccessLevel.GameMaster || DateTime.UtcNow >= from.NextActionTime)
			{
				int pos = pvSrc.Seek(0, SeekOrigin.Current);

				int value = pvSrc.ReadInt32();

				pvSrc.Seek(pos, SeekOrigin.Begin);

				if ((value & ~0x7FFFFFFF) != 0)
				{
					from.OnPaperdollRequest();
				}
				else
				{
					Serial s = value;

					bool blockdefaultonuse = false;

					if (s.IsMobile)
					{
						Mobile m = World.FindMobile(s);

						if (m != null && !m.Deleted)
						{
							blockdefaultonuse = (XmlScript.HasTrigger(m, TriggerName.onUse) &&
												 UberScriptTriggers.Trigger(m, from, TriggerName.onUse)) ||
												(XmlScript.HasTrigger(from, TriggerName.onUse) && UberScriptTriggers.Trigger(from, from, TriggerName.onUse));

							if (!blockdefaultonuse && !m.Deleted)
							{
								//from.Use(m);

								if (PacketHandlerOverrides.UseReqParent != null)
								{
									PacketHandlerOverrides.UseReqParent.OnReceive(state, pvSrc);
								}
								else
								{
									PacketHandlers.UseReq(state, pvSrc);
								}

								return;
							}
						}
					}
					else if (s.IsItem)
					{
						Item item = World.FindItem(s);

						if (item != null && !item.Deleted)
						{
							blockdefaultonuse = (XmlScript.HasTrigger(from, TriggerName.onUse) &&
												 UberScriptTriggers.Trigger(from, from, TriggerName.onUse)) ||
												(XmlScript.HasTrigger(item, TriggerName.onUse) && UberScriptTriggers.Trigger(item, from, TriggerName.onUse));

							// need to check the item again in case it was modified in the OnUse or OnUser method
							if (!blockdefaultonuse && !item.Deleted)
							{
								//from.Use(item);
								
								if (PacketHandlerOverrides.UseReqParent != null)
								{
									PacketHandlerOverrides.UseReqParent.OnReceive(state, pvSrc);
								}
								else
								{
									PacketHandlers.UseReq(state, pvSrc);
								}

								return;
							}
						}
					}
				}

				from.NextActionTime = DateTime.UtcNow + Mobile.ServerWideObjectDelay;
			}
			else
			{
				from.SendActionMessage();
			}
		}

		public static void LookReq(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;

			int pos = pvSrc.Seek(0, SeekOrigin.Current);

			Serial s = pvSrc.ReadInt32();

			pvSrc.Seek(pos, SeekOrigin.Begin);

			if (s.IsMobile)
			{
				Mobile m = World.FindMobile(s);

				if (m != null && from.CanSee(m) && Utility.InUpdateRange(from, m) &&
					XmlScript.HasTrigger(m, TriggerName.onSingleClick) &&
					UberScriptTriggers.Trigger(m, from, TriggerName.onSingleClick))
				{
					return;
				}
			}
			else if (s.IsItem)
			{
				Item item = World.FindItem(s);

				if (item != null && !item.Deleted && from.CanSee(item) &&
					Utility.InUpdateRange(from.Location, item.GetWorldLocation(from)) &&
					XmlScript.HasTrigger(item, TriggerName.onSingleClick) &&
					UberScriptTriggers.Trigger(item, from, TriggerName.onSingleClick, item))
				{
					return;
				}
			}

			if (PacketHandlerOverrides.LookReqParent != null)
			{
				PacketHandlerOverrides.LookReqParent.OnReceive(state, pvSrc);
			}
			else
			{
				PacketHandlers.LookReq(state, pvSrc);
			}
		}

		public static bool OnDragLift(Mobile from, Item item)
		{
			// look for attachments on the item
			if (item != null)
			{
				// true if return override encountered
				if (XmlScript.HasTrigger(item, TriggerName.onDragLift) &&
					UberScriptTriggers.Trigger(item, from, TriggerName.onDragLift, item))
				{
					return false;
				}
				ArrayList attachments = FindAttachments(item);

				if (attachments != null)
				{
					foreach (XmlAttachment a in attachments)
					{
						if (a != null && !a.Deleted && !a.OnDragLift(from, item))
						{
							return false;
						}
					}
				}
			}

			// allow lifts by default
			return true;
		}

		public class ErrorReporter
		{
			private static void SendEmail(string filePath)
			{
				Console.Write("XmlSpawner2 Attachment error: Sending email...");

				MailMessage message = null;
				try
				{
					message = new MailMessage("RunUO@localhost", Email.CrashAddresses);
				}
				catch
				{ }

				if (message == null)
				{
					Console.Write("Unable to send email.  Possible invalid email address.");
					return;
				}
				message.Subject = "Automated XmlSpawner2 Attachment Error Report";

				message.Body = "Automated XmlSpawner2 Attachment Report. See attachment for details.";

				message.Attachments.Add(new Attachment(filePath));

				if (Email.Send(message))
				{
					Console.WriteLine("done");
				}
				else
				{
					Console.WriteLine("failed");
				}
			}

			private static string GetRoot()
			{
				try
				{
					return Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
				}
				catch
				{
					return "";
				}
			}

			private static string Combine(string path1, string path2)
			{
				if (path1 == "")
				{
					return path2;
				}

				return Path.Combine(path1, path2);
			}

			private static void CreateDirectory(string path)
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
			}

			private static void CreateDirectory(string path1, string path2)
			{
				CreateDirectory(Combine(path1, path2));
			}

			private static void CopyFile(string rootOrigin, string rootBackup, string path)
			{
				string originPath = Combine(rootOrigin, path);
				string backupPath = Combine(rootBackup, path);

				try
				{
					if (File.Exists(originPath))
					{
						File.Copy(originPath, backupPath);
					}
				}
				catch
				{ }
			}

			public static void GenerateErrorReport(string error)
			{
				Console.Write("\nXmlSpawner2 Attachment Error:\n{0}\nGenerating report...", error);

				try
				{
					string timeStamp = GetTimeStamp();
					string fileName = String.Format("Attachment Error {0}.log", timeStamp);

					string root = GetRoot();
					string filePath = Combine(root, fileName);

					using (StreamWriter op = new StreamWriter(filePath))
					{
						Version ver = Core.Assembly.GetName().Version;

						op.WriteLine("XmlSpawner2 Attachment Error Report");
						op.WriteLine("===================");
						op.WriteLine();
						op.WriteLine("RunUO Version {0}.{1}.{3}, Build {2}", ver.Major, ver.Minor, ver.Revision, ver.Build);
						op.WriteLine("Operating System: {0}", Environment.OSVersion);
						op.WriteLine(".NET Framework: {0}", Environment.Version);
						op.WriteLine("XmlSpawner2: {0}", XmlSpawner.Version);
						op.WriteLine("Time: {0}", DateTime.UtcNow);

						op.WriteLine();

						op.WriteLine("Error:");
						op.WriteLine(error);

						op.WriteLine();
						op.WriteLine("Specific Attachment Errors:");
						foreach (DeserErrorDetails s in desererror)
						{
							op.WriteLine("{0} - {1}", s.Type, s.Details);
						}
					}

					Console.WriteLine("done");

					//if (Email.CrashAddresses != null)
					//    SendEmail(filePath);
				}
				catch
				{
					Console.WriteLine("failed");
				}
			}

			private static string GetTimeStamp()
			{
				DateTime now = DateTime.UtcNow;

				return String.Format("{0}-{1}-{2}-{3}-{4}-{5}", now.Day, now.Month, now.Year, now.Hour, now.Minute, now.Second);
			}
		}
	}
}