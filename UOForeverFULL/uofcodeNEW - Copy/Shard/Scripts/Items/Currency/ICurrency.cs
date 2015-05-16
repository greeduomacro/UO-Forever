namespace Server.Items
{
	public interface ICurrency
	{
		int Amount { get; set; }

		bool Deleted { get; }

		void Consume();
		void Consume(int amount);

		void Delete();

		void Serialize(GenericWriter writer);
		void Deserialize(GenericReader reader);
	}
}