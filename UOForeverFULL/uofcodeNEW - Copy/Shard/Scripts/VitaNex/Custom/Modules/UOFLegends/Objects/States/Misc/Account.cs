#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Accounting;
using Server.Misc;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class AccountLegendState : LegendState<Account>
	{
		public override string TableName { get { return "accounts"; } }

		protected override void OnCompile(Account o, IDictionary<string, SimpleType> data)
		{
			if (o == null)
			{
				data.Clear();
				return;
			}

			data.Add("username", o.Username);

			string pass;

			switch (AccountHandler.ProtectPasswords)
			{
				case PasswordProtection.None:
					pass = o.PlainPassword ?? String.Empty;
					break;
				case PasswordProtection.Crypt:
					{
						pass = o.CryptPassword ?? String.Empty;
						pass = pass.Replace("-", String.Empty);
					}
					break;
				case PasswordProtection.NewCrypt:
					{
						pass = o.NewCryptPassword ?? String.Empty;
						pass = pass.Replace("-", String.Empty);
					}
					break;
				case PasswordProtection.SaltedCrypt:
					{
						pass = o.SaltedCryptPassword ?? String.Empty;
						pass = pass.Replace("-", String.Empty);
					}
					break;
				default:
					pass = String.Empty;
					break;
			}

			data.Add("password", pass);
			data.Add("passtype", AccountHandler.ProtectPasswords.ToString());
			data.Add("passchanged", o.LastPasswordChanged);

			data.Add("email", o.Email ?? String.Empty);

			data.Add("access", o.AccessLevel.ToString());

			data.Add("created", o.Created);
			data.Add("gametime", o.TotalGameTime);

			data.Add("language", o.Language ?? String.Empty);

			data.Add("inactive", o.Inactive);
			data.Add("banned", o.Banned);
			data.Add("young", o.Young);

			data.Add("charlimit", o.Limit);
			data.Add("charcount", o.Mobiles.Count(c => c != null));
			data.Add("characters", JoinData(o.Mobiles.Where(c => c != null).Select(c => c.Serial.Value)));
			
			data.Add("lastlogin", o.LastLogin);
			data.Add("ipcount", o.LoginIPs.Count(ip => ip != null));
			data.Add("loginips", JoinData(o.LoginIPs.Where(ip => ip != null)));

			var tags =
				o.Tags.Where(t => t != null && !String.IsNullOrWhiteSpace(t.Name) && !String.IsNullOrWhiteSpace(t.Value))
				 .Select(t => JoinSubData(t.Name, t.Value ?? String.Empty))
				 .ToArray();

			data.Add("tagcount", tags.Length);
			data.Add("tags", JoinData(tags));

			var comments =
				o.Comments.Where(c => c != null && !String.IsNullOrWhiteSpace(c.AddedBy) && !String.IsNullOrWhiteSpace(c.Content))
				 .Select(c => JoinSubData(c.AddedBy, c.Content)).ToArray();

			data.Add("commentcount", comments.Length);
			data.Add("comments", JoinData(comments));
		}
	}
}