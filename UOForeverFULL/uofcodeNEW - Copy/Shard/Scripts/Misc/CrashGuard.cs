#region Header
//   Vorspire    _,-'/-'/  CrashGuard.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Poker;
using VitaNex;
using VitaNex.IO;
using VitaNex.Network;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Misc
{
	[CoreService("Crash Guard", "1.0.1", Priority = TaskPriority.Highest)]
	public static class CrashGuard
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		private static int _RestartTime;
		private static bool _RestartForce;
		private static bool _RestartIntercept;

		private static FileInfo _CrashState;
		private static FileInfo _Report;

		private static CrashedEventArgs _Event;
		private static DateTime _Stamp;

		private static List<PlayerMobile> _LastOnline;
		private static List<CrashNote> _CrashNotes;

		public static DirectoryInfo BackupSource { get; private set; }
		public static DirectoryInfo BackupTarget { get; private set; }
		public static DirectoryInfo ReportTarget { get; private set; }

		public static bool HadCrash { get; private set; }

		public static CrashGuardOptions CSOptions { get; private set; }

		static CrashGuard()
		{
			CSOptions = new CrashGuardOptions();
		}

		private static void CSConfig()
		{
			Core.CrashedHandler = OnServerCrashed;

			EventSink.Crashed += e =>
			{
				if (!World.Loading && !World.Saving)
				{
					NotifyPlayers();
				}
			};

			BackupSource = IOUtility.EnsureDirectory(Core.BaseDirectory + "/Saves/");
			BackupTarget = IOUtility.EnsureDirectory(Core.BaseDirectory + "/Backups/Crashed/");
			ReportTarget = IOUtility.EnsureDirectory(Core.BaseDirectory + "/Reports/");

			_CrashState = IOUtility.EnsureFile(VitaNexCore.CacheDirectory + "/CrashState.bin");

			_LastOnline = new List<PlayerMobile>();
			_CrashNotes = new List<CrashNote>();

			CommandUtility.Register(
				"CrashGuard",
				Access,
				e =>
				{
					if (e.Mobile is PlayerMobile)
					{
						SuperGump.Send(new CrashNoteListGump((PlayerMobile)e.Mobile));
					}
				});

			CommandUtility.Register(
				"Crash",
				AccessLevel.Administrator,
				e =>
				{
					if (e.Mobile is PlayerMobile)
					{
						SuperGump.Send(
							new ConfirmDialogGump((PlayerMobile)e.Mobile)
							{
								Title = "Force Crash",
								Html = "Click OK to force the server to crash.",
								AcceptHandler = b => { throw new Exception("Forced Crash Exception"); }
							});
					}
				});
		}

		private static void CSInvoke()
		{
			if (HadCrash)
			{
				EventSink.Login += e =>
				{
					if (e.Mobile is PlayerMobile)
					{
						OnLogin((PlayerMobile)e.Mobile);
					}
				};
			}
		}

		private static void CSLoad()
		{
			LoadState();
		}

		private static void CSSave()
		{
			SaveState(false);
		}

		private static void CSDisposed()
		{
			Core.CrashedHandler = null;
		}

		private static void NotifyPlayers()
		{
			VitaNexCore.TryCatch(
				() =>
				{
					CSOptions.ToConsole("Notifying Players...");

					var sb = new StringBuilder();
					sb.AppendLine(
						new[] {"Oops! {0} attempted to divide by zero, the Universe said NO!"}.GetRandom(), ServerList.ServerName);

					if (CSOptions.Email && !String.IsNullOrWhiteSpace(CSOptions.EmailOptions.From) &&
						!String.IsNullOrWhiteSpace(CSOptions.EmailOptions.To))
					{
						sb.AppendLine();
						sb.AppendLine("Relax, the staff will be notified.");
					}

					if (CSOptions.Restart)
					{
						sb.AppendLine();
						sb.AppendLine(
							"{0} will attempt to restart itself, {1} if it is successful.",
							ServerList.ServerName,
							CSOptions.RestartDebug ? "it could take a few minutes" : "it shouldn't take long");
						sb.AppendLine(
							"So... now would be a great time to {0}.",
							new[]
							{
								"solve world hunger", "visit the moon", "stare at this message", "practise licking your elbow",
								"clean your keyboard", "clear your history", "tweet an annoying status", "listen to a Bieber song",
								"poke dead things with a stick", "twerk", "plank", "laugh maniacally", "discover a heavy element",
								"avast your mateys", "spam login requests", "fing those fingers"
							}.GetRandom());
					}

					sb.AppendLine();
					sb.AppendLine("We are sorry for the inconvenience!");
					sb.AppendLine("- {0} Team", ServerList.ServerName);

					string html = sb.ToString();

					NetState.Instances.AsParallel()
							.Where(s => s != null && s.Socket != null && s.Mobile is PlayerMobile)
							.ForEach(
								s => SuperGump.Send(
									new NoticeDialogGump((PlayerMobile)s.Mobile)
									{
										Width = 500,
										Height = 350,
										CanMove = false,
										Title = "Oops!",
										Html = html
									}));

					CSOptions.ToConsole("Done");
				},
				CSOptions.ToConsole);
		}

		private static void Delta()
		{
			VitaNexCore.TryCatch(
				() =>
				{
					_LastOnline.Clear();
					_LastOnline.AddRange(
						NetState.Instances.Where(s => s != null && s.Mobile is PlayerMobile).Select(s => (PlayerMobile)s.Mobile));
					SaveState(true);
				},
				CSOptions.ToConsole);

			VitaNexCore.TryCatch(
				() =>
				{
					CSOptions.ToConsole("Disposing NetStates/Listeners...");
					NetState.Instances.Where(s => s != null && s.Socket != null).ForEach(s => s.Dispose());
					Core.MessagePump.Listeners.ForEach(l => l.Dispose());
					Core.MessagePump.Listeners = new Listener[0];
					CSOptions.ToConsole("Done");

					AutoSave.SavesEnabled = false;
				},
				CSOptions.ToConsole);
		}

		private static bool Report()
		{
			return VitaNexCore.TryCatchGet(
				() =>
				{
					_Report =
						IOUtility.EnsureFile(
							ReportTarget + "/" + IOUtility.GetSafeFileName("Crash " + _Stamp.ToSimpleString("D d M y - t@h-m@") + ".log"));

					Version ver = Core.Version;
					int clients = NetState.Instances.Count;

					var lines = new List<string>
					{
						String.Format("{0} Crash Report", ServerList.ServerName),
						String.Format("{0}=============", new String('=', ServerList.ServerName.Length)),
						String.Empty,
						String.Format("RunUO Version {0}.{1}, Build {2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision),
						String.Format("Operating System: {0}", Environment.OSVersion),
						String.Format(".NET Framework: {0}", Environment.Version),
						String.Format("Date: {0}", _Stamp.ToSimpleString("D d M y - t@h-m@")),
						String.Format("Mobiles: {0:#,#}", VitaNexCore.TryCatchGet(() => World.Mobiles.Count)),
						String.Format("Items: {0:#,#}", VitaNexCore.TryCatchGet(() => World.Items.Count)),
						String.Empty,
						String.Format("Exception:"),
						String.Format("{0}", _Event != null ? _Event.Exception : null),
						String.Empty,
						String.Format("Clients: {0:#,#}", clients),
						String.Empty
					};

					var breaker = new[] {lines[2], lines[2], lines[1], lines[2], lines[2]};

					lines.AddRange(breaker);

					int i = 0;

					lines.AddRange(
						NetState.Instances.Where(ns => ns != null && ns.Socket != null && ns.Mobile != null && ns.Account != null)
								.Select(
									ns =>
									String.Format(
										"{0:#,#}:\t [{1}]: \tAccount: '{2}' -> '{3}' (0x{4:X})",
										++i,
										ns,
										ns.Account.Username,
										ns.Mobile.RawName,
										ns.Mobile.Serial.Value)));

					lines.AddRange(breaker);

					_Report.AppendText(false, lines.ToArray());
					return true;
				},
				CSOptions.ToConsole);
		}

		private static bool Backup(bool restore)
		{
			if (restore)
			{
				return VitaNexCore.TryCatchGet(
					() =>
					{
						World.Save(false, false);
						//World.WaitForWriteCompletion();

						return true;
					},
					CSOptions.ToConsole);
			}

			return VitaNexCore.TryCatchGet(
				() =>
				{
					DirectoryInfo target = IOUtility.EnsureDirectory(BackupTarget + "/" + _Stamp.ToSimpleString("D d M y - t@h-m@"));

					Parallel.ForEach(
						GetFiles(BackupSource),
						file =>
						VitaNexCore.TryCatch(
							() =>
							file.CopyTo(IOUtility.EnsureFile(file.FullName.Replace(BackupSource.FullName, target.FullName)).FullName, true),
							CSOptions.ToConsole));

					return true;
				},
				CSOptions.ToConsole);
		}

		private static IEnumerable<FileInfo> GetFiles(DirectoryInfo dir)
		{
			foreach (FileInfo file in dir.EnumerateFiles())
			{
				yield return file;
			}

			foreach (FileInfo file in dir.EnumerateDirectories().SelectMany(GetFiles))
			{
				yield return file;
			}
		}

		private static bool Restart()
		{
			return VitaNexCore.TryCatchGet(
				() =>
				{
					Process.Start(
						Core.ExePath,
						!CSOptions.RestartDebug || Insensitive.Contains(Core.Arguments, "-debug")
							? Core.Arguments
							: Core.Arguments + " -debug");

					return true;
				},
				CSOptions.ToConsole);
		}

		private static bool Email()
		{
			return CSOptions.EmailOptions.Valid && VitaNexCore.TryCatchGet(
				() =>
				{
					using (SmtpClient smtp = CSOptions.EmailOptions)
					{
						string sub = String.Format("{0} Crash Report", ServerList.ServerName);
						string body = String.Format(
							"{0} has crashed! {1}Crash Report: {2}",
							ServerList.ServerName,
							CSOptions.ReportAttach ? "Attached " : String.Empty,
							_Report == null || !_Report.Exists ? "N/A" : _Report.Name);

						if (!CSOptions.ReportAttach && _Report != null && _Report.Exists)
						{
							using (StreamReader sr = _Report.OpenText())
							{
								body += VitaNexCore.TryCatchGet(sr.ReadToEnd, CSOptions.ToConsole);
							}
						}

						using (MailMessage message = CSOptions.EmailOptions)
						{
							return VitaNexCore.TryCatchGet(
								() =>
								{
									message.Subject = sub;
									message.Body = body;

									if (CSOptions.ReportAttach && _Report != null && _Report.Exists)
									{
										message.Attachments.Add(new Attachment(_Report.FullName, "text/plain; charset=utf-8"));
									}

									smtp.Send(message);
									return true;
								},
								CSOptions.ToConsole);
						}
					}
				},
				CSOptions.ToConsole);
		}

		private static void SaveState(bool hadCrash)
		{
			VitaNexCore.TryCatch(
				() => _CrashState.Serialize(
					w =>
					{
						w.Write(hadCrash);
						w.WriteBlockList(_LastOnline, w.Write);
						w.WriteBlockList(_CrashNotes, t => t.Serialize(w));
					}),
				CSOptions.ToConsole);
		}

		private static void LoadState()
		{
			VitaNexCore.TryCatch(
				() => _CrashState.Deserialize(
					r =>
					{
						HadCrash = r.ReadBool();

						_LastOnline.AddRange(r.ReadBlockList(r.ReadMobile<PlayerMobile>));
						_LastOnline.TrimExcess();

						_CrashNotes.AddRange(r.ReadBlockList(() => new CrashNote(r)));

						DateTime expire = DateTime.Now - TimeSpan.FromDays(7);

						_CrashNotes.Where(t => t.Mobile == null || t.Date <= expire || String.IsNullOrWhiteSpace(t.Note))
								   .ForEach(t => _CrashNotes.Remove(t));
						_CrashNotes.TrimExcess();
					}),
				CSOptions.ToConsole);
		}

		private static void OnServerCrashed(CrashedEventArgs e)
		{
			if (World.Loading || World.Saving)
			{
				return;
			}

		    PokerGame.EventSink_Crashed(e);

			_Event = e;
			_Stamp = DateTime.Now;

			Core.CrashedHandler = null;

			VitaNexCore.TryCatch(
				() =>
				{
					if (CSOptions.Report)
					{
						CSOptions.ToConsole("Writing Crash Report...");
						CSOptions.ToConsole(Report() ? "Done" : "Failed");
					}

					//World.WaitForWriteCompletion();

					if (CSOptions.Backup)
					{
						CSOptions.ToConsole("Backing Up Saves...");
						CSOptions.ToConsole(Backup(false) ? "Done" : "Failed");
					}

					if (CSOptions.Restore)
					{
						CSOptions.ToConsole("Creating Restore Saves...");
						CSOptions.ToConsole(Backup(true) ? "Done" : "Failed");
					}

					Delta();

					if (!CSOptions.Restart)
					{
						return;
					}

					if (CSOptions.RestartDelay > TimeSpan.Zero)
					{
						CSOptions.ToConsole("Press 'ESC' to cancel or 'Enter' to skip the restart delay...");
						CSOptions.ToConsole("{0} will restart in: ", ServerList.ServerName);

						_RestartTime = (int)Math.Ceiling(CSOptions.RestartDelay.TotalSeconds);

						int cX = Console.CursorLeft;
						int cY = Console.CursorTop;

						var wait = new EventWaitHandle(false, EventResetMode.ManualReset);

						ThreadPool.QueueUserWorkItem(
							cb =>
							{
								bool exit = false;

								do
								{
									lock (VitaNexCore.ConsoleLock)
									{
										Console.CursorLeft = cX;
										Console.CursorTop = cY;
										Console.Write(new String(' ', 40));
										Console.CursorLeft = cX;
										Console.CursorTop = cY;
										Console.Write("{0} second{1}", _RestartTime, _RestartTime != 1 ? "s" : String.Empty);
									}

									if (_RestartIntercept || _RestartForce)
									{
										exit = true;
									}

									if (_RestartTime-- < 0)
									{
										exit = _RestartForce = true;
									}

									if (!exit)
									{
										Thread.Sleep(1000);
									}

									wait.Set();
								}
								while (!exit);
							});

						ThreadPool.QueueUserWorkItem(
							cb =>
							{
								bool escape = false;

								do
								{
									if (_RestartForce || _RestartIntercept || _RestartTime <= 0)
									{
										escape = true;
									}

									if (!escape && !Console.KeyAvailable)
									{
										switch (Console.ReadKey(true).Key)
										{
											case ConsoleKey.Escape:
												escape = _RestartForce = _RestartIntercept = true;
												break;
											case ConsoleKey.Enter:
												escape = _RestartForce = true;
												break;
										}
									}

									if (!escape)
									{
										wait.WaitOne();
									}
								}
								while (!escape);
							});

						while (!_RestartForce && !_RestartIntercept && _RestartTime > 0)
						{
							Thread.Sleep(10);
						}

						lock (VitaNexCore.ConsoleLock)
						{
							Console.WriteLine();
						}
					}

					DeltaRestart();
				},
				CSOptions.ToConsole);
		}

		private static void DeltaRestart()
		{
			if (_RestartIntercept)
			{
				CSOptions.ToConsole("Restart Cancelled!");
				return;
			}

			CSOptions.ToConsole("Restarting {0}...", ServerList.ServerName);

			if (Restart())
			{
				Core.Kill();
			}

			_Event.Close = true;
		}

		private static void OnLogin(PlayerMobile m)
		{
			if (m == null || !_LastOnline.Contains(m))
			{
				return;
			}

			_LastOnline.Remove(m);

			if (!CSOptions.Notes || m.Deleted || !m.IsOnline())
			{
				return;
			}

			var sb = new StringBuilder();

			sb.AppendLine("{0} crashed while you were playing.", ServerList.ServerName);
			sb.AppendLine();
			sb.AppendLine("We know this can be frustrating, but we are working hard to keep the server bug-free.");
			sb.AppendLine();
			sb.AppendLine("If you have an idea as to what may have caused the last crash, please let us know!");
			sb.AppendLine();
			sb.AppendLine("Click OK to fill in a crash note, or Cancel to ignore this message.");

			string html = sb.ToString();

			VitaNexCore.TryCatch(
				() => Timer.DelayCall(
					TimeSpan.FromSeconds(3.0),
					() => SuperGump.Send(
						new ConfirmDialogGump(m)
						{
							Title = "Crash Note",
							Html = html,
							Modal = false,
							CanDispose = false,
							CanMove = false,
							AcceptHandler = b1 => SuperGump.Send(new CrashNoteSubmitGump(m))
						})),
				CSOptions.ToConsole);
		}

		private class CrashNoteSubmitGump : InputDialogGump
		{
			public CrashNoteSubmitGump(PlayerMobile user)
				: base(user)
			{
				Width = 420;
				Height = 420;

				Limit = 500;

				Title = "Crash Note";
				Icon = 7052;

				Modal = false;
				CanDispose = false;
				CanMove = false;
			}

			protected override void CompileLayout(SuperGumpLayout layout)
			{
				base.CompileLayout(layout);

				layout.Remove("html/body/info");

				layout.AddReplace("background/body/input", () => AddBackground(100, 70, Width - 120, Height - 130, 9350));

				layout.AddReplace(
					"textentry/body/input",
					() =>
					{
						if (Limited)
						{
							AddTextEntryLimited(105, 75, Width - 130, Height - 140, TextHue, InputText, Limit, ParseInput);
						}
						else
						{
							AddTextEntry(105, 75, Width - 130, Height - 140, TextHue, InputText, ParseInput);
						}
					});

				layout.Add("background/body/subtext", () => AddBackground(20, Height - 50, Width - 120, 30, 9350));
				layout.Add(
					"label/body/subtext",
					() => AddLabelCropped(25, Height - 45, Width - 130, 20, TextHue, DateTime.Now.ToSimpleString("D d M y - t@h-m@")));
			}

			protected override void OnAccept(GumpButton button)
			{
				base.OnAccept(button);

				if (String.IsNullOrWhiteSpace(InputText))
				{
					User.SendMessage("Your message can't be blank.");
					Refresh();
					return;
				}

				_CrashNotes.Add(new CrashNote(DateTime.Now, User, InputText));
				User.SendMessage(85, "Your note has been submitted! Enjoy your time while we take care of business!");

				NetState.Instances.Where(
					s => s != null && s.Socket != null && s.Mobile != null && s.Mobile != User && s.Mobile.AccessLevel >= Access)
						.ForEach(
							s => s.Mobile.SendMessage(85, "A new crash note was submitted by {0}. Use [CrashGuard to view it.", User.RawName));
			}
		}

		private class CrashNoteListGump : ListGump<CrashNote>
		{
			public CrashNoteListGump(PlayerMobile user)
				: base(user)
			{
				Title = "Crash Notes";
				EmptyText = "No notes to display.";

				Sorted = true;
				CanSearch = true;
				CanMove = false;
			}

			protected override void CompileList(List<CrashNote> list)
			{
				list.Clear();
				list.AddRange(_CrashNotes);

				base.CompileList(list);
			}

			protected override void CompileMenuOptions(MenuGumpOptions list)
			{
				list.AppendEntry(
					new ListGumpEntry("Options", () => User.SendGump(new PropertiesGump(User, CSOptions)), HighlightHue));

				if (_CrashNotes.Count > 0 && CSOptions.EmailOptions.Valid)
				{
					list.AppendEntry(
						new ListGumpEntry(
							"Email Notes",
							() => VitaNexCore.TryCatch(
								() =>
								{
									Refresh();

									using (SmtpClient smtp = CSOptions.EmailOptions)
									{
										using (MailMessage email = CSOptions.EmailOptions)
										{
											email.Subject = String.Format("{0} Crash Notes", ServerList.ServerName);
											email.IsBodyHtml = true;

											var lines = new String[_CrashNotes.Count * 2];
											CrashNote[] notes = _CrashNotes.ToArray();

											email.Body = String.Format("<h1>{0} Notes:</h1><hr/><br/>", _CrashNotes.Count.ToString("#,#"));

											notes.For(
												(i, t) =>
												{
													lines[i] = String.Format(
														"<a href=\"#{0}\">{1} {2}</a>", t.GetHashCode(), t.Date.ToSimpleString("D d M y - t@h-m@"), t.Mobile);
													lines[notes.Length - i] = String.Format(
														"<div id=\"{0}\"><h2>{1}</h2><h3>{2}</h3><p>{3}</p></div>",
														t.GetHashCode(),
														t.Date.ToSimpleString("D d M y - t@h-m@"),
														t.Mobile != null ? t.Mobile.ToString() : "Anon.",
														t.Note);
												});

											email.Body += String.Join("\n", lines);
											smtp.Send(email);

											User.SendMessage(85, "An email has been sent to {0}", CSOptions.EmailOptions.To);
										}
									}
								},
								CSOptions.ToConsole),
							HighlightHue));
				}
				else
				{
					list.RemoveEntry("Email Notes");
				}

				list.AppendEntry(
					new ListGumpEntry(
						"Mark All: Viewed",
						() =>
						{
							_CrashNotes.ForEach(t => t.Viewed = true);
							User.SendMessage("All notes have been marked as viewed.");
							Refresh(true);
						},
						TextHue));

				list.AppendEntry(
					new ListGumpEntry(
						"Mark All: Not Viewed",
						() =>
						{
							_CrashNotes.ForEach(t => t.Viewed = false);
							User.SendMessage("All notes have been marked as not viewed.");
							Refresh(true);
						},
						TextHue));

				list.AppendEntry(
					new ListGumpEntry(
						"Delete All",
						() =>
						{
							_CrashNotes.Clear();

							User.SendMessage("All notes have been deleted.");
							Refresh(true);
						},
						ErrorHue));

				list.AppendEntry(
					new ListGumpEntry(
						"Delete Old",
						() =>
						{
							DateTime expire = DateTime.Now - TimeSpan.FromDays(7);

							_CrashNotes.RemoveAll(t => t.Date <= expire);

							User.SendMessage("All old notes have been deleted.");
							Refresh(true);
						},
						ErrorHue));

				base.CompileMenuOptions(list);
			}

			protected override void SelectEntry(GumpButton button, CrashNote entry)
			{
				base.SelectEntry(button, entry);

				var opts = new MenuGumpOptions();

				opts.AppendEntry(
					new ListGumpEntry(
						"View",
						() =>
						{
							entry.Viewed = true;
							Send(
								new NoticeDialogGump(User, Refresh())
								{
									Title = "Crash Note",
									Html =
										String.Format(
											"Player: {0}\nAccount: {1}\nDate: {2} @ {3}\n\n{4}",
											entry.Mobile,
											entry.Mobile != null && entry.Mobile.Account != null ? entry.Mobile.Account.Username : "n/a",
											entry.Date.ToSimpleString("m-d-y"),
											entry.Date.ToSimpleString("t@h-m@"),
											entry.Note),
									Modal = false,
									CanMove = false,
								});
						},
						HighlightHue));

				opts.AppendEntry(
					!entry.Viewed
						? new ListGumpEntry("Mark Viewed", () => entry.Viewed = true)
						: new ListGumpEntry("Mark Not Viewed", () => entry.Viewed = false));

				opts.AppendEntry(new ListGumpEntry("Delete", () => _CrashNotes.Remove(entry), ErrorHue));

				Send(new MenuGump(User, Refresh(), opts, button));
			}

			protected override int GetLabelHue(int index, int pageIndex, CrashNote entry)
			{
				return entry != null ? entry == Selected ? HighlightHue : !entry.Viewed ? TextHue : ErrorHue : ErrorHue;
			}

			protected override string GetLabelText(int index, int pageIndex, CrashNote entry)
			{
				return entry != null ? entry.ToString() : String.Empty;
			}

			public override string GetSearchKeyFor(CrashNote key)
			{
				return key != null ? key.ToString() : String.Empty;
			}

			public override int SortCompare(CrashNote a, CrashNote b)
			{
				int result = 0;

				if (a.CompareNull(b, ref result))
				{
					return result;
				}

				return a.Date < b.Date ? -1 : a.Date > b.Date ? 1 : 0;
			}
		}

		public sealed class CrashNote : IEquatable<CrashNote>
		{
			public DateTime Date { get; private set; }
			public PlayerMobile Mobile { get; private set; }

			public string Note { get; set; }
			public bool Viewed { get; set; }

			public CrashNote(DateTime date, PlayerMobile m, string note, bool viewed = false)
			{
				Date = date;
				Mobile = m;
				Note = note;

				Viewed = viewed;
			}

			public CrashNote(GenericReader reader)
			{
				Deserialize(reader);
			}

			public override string ToString()
			{
				return String.Format(
					"[{0}]: {1}{2}",
					Date.ToSimpleString(@"m-d-y \@ t@h:m@"),
					Mobile != null ? String.Format("[{0}]: ", Mobile.RawName) : String.Empty,
					Note);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					int hashCode = (Mobile != null ? Mobile.Serial.Value : 0);
					hashCode = (hashCode * 397) ^ Date.GetHashCode();
					hashCode = (hashCode * 397) ^ (Note != null ? Note.GetHashCode() : 0);
					return hashCode;
				}
			}

			public override bool Equals(object obj)
			{
				return !ReferenceEquals(null, obj) && (obj is CrashNote && Equals((CrashNote)obj));
			}

			public bool Equals(CrashNote other)
			{
				return !ReferenceEquals(other, null) && Equals(Mobile, other.Mobile) && Equals(Date, other.Date) &&
					   String.Equals(Note, other.Note);
			}

			public void Serialize(GenericWriter writer)
			{
				writer.SetVersion(0);

				writer.Write(Date);
				writer.Write(Mobile);
				writer.Write(Note);
				writer.Write(Viewed);
			}

			public void Deserialize(GenericReader reader)
			{
				reader.GetVersion();

				Date = reader.ReadDateTime();
				Mobile = reader.ReadMobile<PlayerMobile>();
				Note = reader.ReadString();
				Viewed = reader.ReadBool();
			}

			public static bool operator ==(CrashNote left, CrashNote right)
			{
				if (ReferenceEquals(left, null))
				{
					return ReferenceEquals(right, null);
				}

				return left.Equals(right);
			}

			public static bool operator !=(CrashNote left, CrashNote right)
			{
				if (ReferenceEquals(left, null))
				{
					return !ReferenceEquals(right, null);
				}

				return !left.Equals(right);
			}
		}
	}

	public class CrashGuardOptions : CoreServiceOptions
	{
		[CommandProperty(CrashGuard.Access)]
		public bool Restart { get; set; }

		[CommandProperty(CrashGuard.Access)]
		public bool RestartDebug { get; set; }

		[CommandProperty(CrashGuard.Access)]
		public TimeSpan RestartDelay { get; set; }

		[CommandProperty(CrashGuard.Access)]
		public bool Backup { get; set; }

		[CommandProperty(CrashGuard.Access)]
		public bool Restore { get; set; }

		[CommandProperty(CrashGuard.Access)]
		public bool Report { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool ReportAttach { get; set; }

		[CommandProperty(CrashGuard.Access)]
		public bool Notes { get; set; }

		[CommandProperty(CrashGuard.Access)]
		public bool Email { get; set; }

		[CommandProperty(CrashGuard.Access)]
		public EmailOptions EmailOptions { get; set; }

		public CrashGuardOptions()
			: base(typeof(CrashGuard))
		{
			Restart = true;
			RestartDebug = true;
			RestartDelay = TimeSpan.FromSeconds(10);
			Backup = true;
			Restore = false;
			Report = true;
			ReportAttach = false;
			Notes = true;
			Email = true;
			EmailOptions = new EmailOptions();
		}

		public CrashGuardOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			Restart = false;
			RestartDebug = false;
			RestartDelay = TimeSpan.Zero;
			Backup = false;
			Restore = false;
			Report = false;
			ReportAttach = false;
			Notes = false;
			Email = false;
			EmailOptions.Clear();
		}

		public override void Reset()
		{
			Restart = true;
			RestartDebug = true;
			RestartDelay = TimeSpan.FromSeconds(10);
			Backup = true;
			Restore = false;
			Report = true;
			ReportAttach = false;
			Notes = true;
			Email = true;
			EmailOptions.Reset();
		}

		public override string ToString()
		{
			return String.Format("Crash Guard Options");
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.Write(Restart);
						writer.Write(RestartDebug);
						writer.Write(RestartDelay);
						writer.Write(Backup);
						writer.Write(Restore);
						writer.Write(Report);
						writer.Write(ReportAttach);
						writer.Write(Notes);
						writer.Write(Email);
						EmailOptions.Serialize(writer);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
						Restart = reader.ReadBool();
						RestartDebug = reader.ReadBool();
						RestartDelay = reader.ReadTimeSpan();
						Backup = reader.ReadBool();
						Restore = reader.ReadBool();
						Report = reader.ReadBool();
						ReportAttach = reader.ReadBool();
						Notes = reader.ReadBool();
						Email = reader.ReadBool();
						EmailOptions = new EmailOptions(reader);
					}
					break;
			}
		}
	}
}