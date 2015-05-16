#region References
using System;
using System.Collections;
using System.Data.Odbc;
using System.Threading;
#endregion

namespace Server.Engines.MyRunUO
{
	public class DatabaseCommandQueue
	{
		private readonly Queue m_Queue;
		private readonly ManualResetEvent m_Sync;

		private bool m_HasCompleted;

		private readonly string m_CompletionString;
		private readonly string m_ConnectionString;

		public bool HasCompleted { get { return m_HasCompleted; } }

		public void Enqueue(object obj)
		{
			lock (m_Queue.SyncRoot)
			{
				m_Queue.Enqueue(obj);

				try
				{
					m_Sync.Set();
				}
				catch
				{ }
			}
		}

		public DatabaseCommandQueue(string completionString, string threadName)
			: this(Config.CompileConnectionString(), completionString, threadName)
		{ }

		public DatabaseCommandQueue(string connectionString, string completionString, string threadName)
		{
			m_CompletionString = completionString;
			m_ConnectionString = connectionString;

			m_Queue = Queue.Synchronized(new Queue());

			m_Queue.Enqueue(null); // signal connect

			/*m_Queue.Enqueue( "DELETE FROM myrunuo_characters" );
			m_Queue.Enqueue( "DELETE FROM myrunuo_characters_layers" );
			m_Queue.Enqueue( "DELETE FROM myrunuo_characters_skills" );
			m_Queue.Enqueue( "DELETE FROM myrunuo_guilds" );
			m_Queue.Enqueue( "DELETE FROM myrunuo_guilds_wars" );*/

			m_Sync = new ManualResetEvent(true);

			var mThread = new Thread(Thread_Start) {
				Name = threadName,
				Priority = Config.Options.Priority
			};
			mThread.Start();
		}

		private void Thread_Start()
		{
			bool connected = false;

			OdbcConnection connection = null;
			OdbcCommand command = null;
			OdbcTransaction transact = null;

			DateTime start = DateTime.UtcNow;

			bool shouldWriteException = true;

			while (true)
			{
				m_Sync.WaitOne();

				while (m_Queue.Count > 0)
				{
					try
					{
						object obj = m_Queue.Dequeue();

						if (obj == null)
						{
							if (connected)
							{
								if (transact != null)
								{
									try
									{
										transact.Commit();
									}
									catch (Exception commitException)
									{
										Console.WriteLine("MyRunUO: Exception caught when committing transaction");
										Console.WriteLine(commitException);

										try
										{
											transact.Rollback();
											Console.WriteLine("MyRunUO: Transaction has been rolled back");
										}
										catch (Exception rollbackException)
										{
											Console.WriteLine("MyRunUO: Exception caught when rolling back transaction");
											Console.WriteLine(rollbackException);
										}
									}
								}

								if (connection != null)
								{
									try
									{
										connection.Close();
									}
									catch
									{ }

									try
									{
										connection.Dispose();
									}
									catch
									{ }
								}

								if (command != null)
								{
									try
									{
										command.Dispose();
									}
									catch
									{ }
								}

								try
								{
									m_Sync.Close();
								}
								catch
								{ }

								Console.WriteLine(m_CompletionString, (DateTime.UtcNow - start).TotalSeconds);
								m_HasCompleted = true;

								return;
							}

							try
							{
								connected = true;
								connection = new OdbcConnection(m_ConnectionString);
								connection.Open();

								connection.ChangeDatabase(Config.Options.ConnectionInfo.Database);
								
								command = connection.CreateCommand();

								if (Config.Options.UseTransactions)
								{
									transact = connection.BeginTransaction();
									command.Transaction = transact;
								}
							}
							catch (Exception e)
							{
								try
								{
									if (transact != null)
									{
										transact.Rollback();
									}
								}
								catch
								{ }

								try
								{
									if (connection != null)
									{
										connection.Close();
									}
								}
								catch
								{ }

								try
								{
									if (connection != null)
									{
										connection.Dispose();
									}
								}
								catch
								{ }

								try
								{
									if (command != null)
									{
										command.Dispose();
									}
								}
								catch
								{ }

								try
								{
									m_Sync.Close();
								}
								catch
								{ }

								Console.WriteLine("MyRunUO: Unable to connect to the database");
								Console.WriteLine(e);
								m_HasCompleted = true;
								return;
							}
						}
						else if (obj is string)
						{
							if (command != null)
							{
								command.CommandText = (string)obj;
								command.ExecuteNonQuery();
							}
						}
						else
						{
							if (command != null)
							{
								var parms = (string[])obj;

								command.CommandText = parms[0];

								if (command.ExecuteScalar() == null)
								{
									command.CommandText = parms[1];
									command.ExecuteNonQuery();
								}
							}
						}
					}
					catch (Exception e)
					{
						if (!shouldWriteException)
						{
							continue;
						}

						Console.WriteLine("MyRunUO: Exception caught in database thread");
						Console.WriteLine(e);

						shouldWriteException = false;
					}
				}

				lock (m_Queue.SyncRoot)
				{
					if (m_Queue.Count == 0)
					{
						m_Sync.Reset();
					}
				}
			}
		}
	}
}