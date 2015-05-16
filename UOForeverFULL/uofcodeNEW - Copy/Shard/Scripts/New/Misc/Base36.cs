using System;
using System.Text;

namespace System
{
	/// <summary>
	/// Class representing a Base36 number
	/// </summary>
	public struct Base36
	{
		#region Constants (and pseudo-constants)

		/// <summary>
		/// Base36 containing the maximum supported value for this type
		/// </summary>
		public static readonly Base36 MaxValue = new Base36(ulong.MaxValue);
		/// <summary>
		/// Base36 containing the minimum supported value for this type
		/// </summary>
		public static readonly Base36 MinValue = new Base36(ulong.MinValue);
		#endregion

		#region Fields

		private ulong numericValue;
		#endregion

		#region Constructor

		/// <summary>
		/// Instantiate a Base36 number from a ulong value
		/// </summary>
		/// <param name="NumericValue">The ulong value to give to the Base36 number</param>
		public Base36(ulong NumericValue)
		{
			numericValue = 0; //required by the struct.
			this.NumericValue = NumericValue;
		}

		/// <summary>
		/// Instantiate a Base36 number from a Base36 string
		/// </summary>
		/// <param name="Value">The value to give to the Base36 number</param>
		public Base36(string Value)
		{
			numericValue = 0; //required by the struct.
			this.Value = Value;
		}
		#endregion

		#region Properties

		/// <summary>
		/// Get or set the value of the type using a base-10 ulong integer
		/// </summary>
		public ulong NumericValue
		{
			get
			{
				return numericValue;
			}
			set
			{
				numericValue = value;
			}
		}

		/// <summary>
		/// Get or set the value of the type using a Base36 string
		/// </summary>
		public string Value
		{
			get
			{
				return Base36.NumberToBase36(numericValue);
			}
			set
			{
				try
				{
					numericValue = Base36.Base36ToNumber(value);
				}
				catch
				{
					//Catch potential errors
					throw new InvalidBase36StringException(value);
				}
			}
		}
		#endregion

		#region Public Static Methods

		/// <summary>
		/// Static method to convert a Base36 string to a ulong integer (base-10)
		/// </summary>
		/// <param name="Base36Value">The number to convert from</param>
		/// <returns>The ulong integer</returns>
		public static ulong Base36ToNumber(string Base36Value)
		{
			//Make sure we have passed something
			if(Base36Value == "")
			{
				throw new InvalidBase36StringException(Base36Value);
			}

			//Make sure the number is in upper case:
			Base36Value = Base36Value.ToUpper();

			//Account for negative values:
			//bool isNegative = false;

			if(Base36Value[0] == '-')
			{
				Base36Value = Base36Value.Substring(1);
				//isNegative = true;
			}

			//Loop through our string and calculate its value
			try
			{
				//Keep a running total of the value
				ulong returnValue = Base36DigitToNumber(Base36Value[Base36Value.Length - 1]);

				//Loop through the character in the string (right to left) and add
				//up increasing powers as we go.
				for(int i = 1; i < Base36Value.Length; i++)
				{
					returnValue += ((ulong)Math.Pow(36, i) * Base36DigitToNumber(Base36Value[Base36Value.Length - (i + 1)]));
				}

				//Do negative correction if required:
				//if(isNegative)
				//{
				//	return returnValue * -1;
				//}
				//else
				//{
					return returnValue;
				//}
			}
			catch
			{
				//If something goes wrong, this is not a valid number
				throw new InvalidBase36StringException(Base36Value);
			}
		}

		/// <summary>
		/// Public static method to convert a ulong integer (base-10) to a Base36 number
		/// </summary>
		/// <param name="NumericValue">The base-10 ulong integer</param>
		/// <returns>A Base36 representation</returns>
		public static string NumberToBase36(ulong NumericValue)
		{
			try
			{
				//Handle negative values:
				//if(NumericValue < 0)
				//{
				//	return string.Concat("-", PositiveNumberToBase36(Math.Abs(NumericValue)));
				//}
				//else
				//{
					return PositiveNumberToBase36(NumericValue);
				//}
			}
			catch
			{
				throw new InvalidBase36NumberException(NumericValue);
			}
		}
		#endregion

		#region Private Static Methods

		private static string PositiveNumberToBase36(ulong NumericValue)
		{
			//This is a clever recursively called function that builds
			//the base-36 string representation of the ulong base-10 value
			if(NumericValue < 36)
			{
				//The get out clause; fires when we reach a number less than
				//36 - this means we can add the last digit.
				return NumberToBase36Digit((byte)NumericValue).ToString();
			}
			else
			{
				//Add digits from left to right in powers of 36
				//(recursive)
				return string.Concat(PositiveNumberToBase36(NumericValue / 36), NumberToBase36Digit((byte)(NumericValue % 36)).ToString());
			}
		}

		private static byte Base36DigitToNumber(char Base36Digit)
		{
			//Converts one base-36 digit to it's base-10 value
			if(!char.IsLetterOrDigit(Base36Digit))
			{
				throw new InvalidBase36CharacterValueException(Base36Digit);
			}

			if(char.IsDigit(Base36Digit))
			{
				//Handles 0 - 9
				return byte.Parse(Base36Digit.ToString());
			}
			else
			{
				//Handles A - Z
				return (byte)((int)Base36Digit - 55);
			}
		}

		private static char NumberToBase36Digit(byte NumericValue)
		{
			//Converts a number to it's base-36 value.
			//Only works for numbers <= 35.
			if(NumericValue > 35)
			{
				throw new InvalidBase36DigitValueException(NumericValue);
			}

			//Numbers:
			if(NumericValue <= 9)
			{
				return NumericValue.ToString()[0];
			}
			else
			{
				//Note that A is code 65, and in this
				//scheme, A = 10.
				return (char)(NumericValue + 55);
			}
		}
		#endregion

		#region Operator Overloads

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="LHS"></param>
		/// <param name="RHS"></param>
		/// <returns></returns>
		public static bool operator > (Base36 LHS, Base36 RHS)
		{
			return LHS.numericValue > RHS.numericValue;
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="LHS"></param>
		/// <param name="RHS"></param>
		/// <returns></returns>
		public static bool operator < (Base36 LHS, Base36 RHS)
		{
			return LHS.numericValue < RHS.numericValue;
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="LHS"></param>
		/// <param name="RHS"></param>
		/// <returns></returns>
		public static bool operator >= (Base36 LHS, Base36 RHS)
		{
			return LHS.numericValue >= RHS.numericValue;
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="LHS"></param>
		/// <param name="RHS"></param>
		/// <returns></returns>
		public static bool operator <= (Base36 LHS, Base36 RHS)
		{
			return LHS.numericValue <= RHS.numericValue;
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="LHS"></param>
		/// <param name="RHS"></param>
		/// <returns></returns>
		public static bool operator == (Base36 LHS, Base36 RHS)
		{
			return LHS.numericValue == RHS.numericValue;
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="LHS"></param>
		/// <param name="RHS"></param>
		/// <returns></returns>
		public static bool operator != (Base36 LHS, Base36 RHS)
		{
			return !(LHS == RHS);
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="LHS"></param>
		/// <param name="RHS"></param>
		/// <returns></returns>
		public static Base36 operator + (Base36 LHS, Base36 RHS)
		{
			return new Base36(LHS.numericValue + RHS.numericValue);
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="LHS"></param>
		/// <param name="RHS"></param>
		/// <returns></returns>
		public static Base36 operator - (Base36 LHS, Base36 RHS)
		{
			return new Base36(LHS.numericValue - RHS.numericValue);
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static Base36 operator ++ (Base36 Value)
		{
			return new Base36(Value.numericValue++);
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static Base36 operator -- (Base36 Value)
		{
			return new Base36(Value.numericValue--);
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="LHS"></param>
		/// <param name="RHS"></param>
		/// <returns></returns>
		public static Base36 operator * (Base36 LHS, Base36 RHS)
		{
			return new Base36(LHS.numericValue * RHS.numericValue);
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="LHS"></param>
		/// <param name="RHS"></param>
		/// <returns></returns>
		public static Base36 operator / (Base36 LHS, Base36 RHS)
		{
			return new Base36(LHS.numericValue / RHS.numericValue);
		}

		/// <summary>
		/// Operator overload
		/// </summary>
		/// <param name="LHS"></param>
		/// <param name="RHS"></param>
		/// <returns></returns>
		public static Base36 operator % (Base36 LHS, Base36 RHS)
		{
			return new Base36(LHS.numericValue % RHS.numericValue);
		}

		/// <summary>
		/// Converts type Base36 to a base-10 ulong
		/// </summary>
		/// <param name="Value">The Base36 object</param>
		/// <returns>The base-10 ulong integer</returns>
		public static implicit operator ulong (Base36 Value)
		{
			return Value.numericValue;
		}

		/// <summary>
		/// Converts type Base36 to a base-10 integer
		/// </summary>
		/// <param name="Value">The Base36 object</param>
		/// <returns>The base-10 integer</returns>
		public static implicit operator int (Base36 Value)
		{
			try
			{
				return (int)Value.numericValue;
			}
			catch
			{
				throw new OverflowException("Overflow: Value too large to return as an integer");
			}
		}

		/// <summary>
		/// Converts type Base36 to a base-10 short
		/// </summary>
		/// <param name="Value">The Base36 object</param>
		/// <returns>The base-10 short</returns>
		public static implicit operator short (Base36 Value)
		{
			try
			{
				return (short)Value.numericValue;
			}
			catch
			{
				throw new OverflowException("Overflow: Value too large to return as a short");
			}
		}

		/// <summary>
		/// Converts a ulong (base-10) to a Base36 type
		/// </summary>
		/// <param name="Value">The ulong to convert</param>
		/// <returns>The Base36 object</returns>
		public static implicit operator Base36 (ulong Value)
		{
			return new Base36(Value);
		}

		/// <summary>
		/// Converts type Base36 to a string; must be explicit, since
		/// Base36 > string is dangerous!
		/// </summary>
		/// <param name="Value">The Base36 type</param>
		/// <returns>The string representation</returns>
		public static explicit operator string (Base36 Value)
		{
			return Value.Value;
		}

		/// <summary>
		/// Converts a string to a Base36
		/// </summary>
		/// <param name="Value">The string (must be a Base36 string)</param>
		/// <returns>A Base36 type</returns>
		public static implicit operator Base36 (string Value)
		{
			return new Base36(Value);
		}
		#endregion

		#region Public Override Methods

		/// <summary>
		/// Returns a string representation of the Base36 number
		/// </summary>
		/// <returns>A string representation</returns>
		public override string ToString()
		{
			return Base36.NumberToBase36(numericValue);
		}

		/// <summary>
		/// A unique value representing the value of the number
		/// </summary>
		/// <returns>The unique number</returns>
		public override int GetHashCode()
		{
			return numericValue.GetHashCode();
		}

		/// <summary>
		/// Determines if an object has the same value as the instance
		/// </summary>
		/// <param name="obj">The object to compare</param>
		/// <returns>True if the values are the same</returns>
		public override bool Equals(object obj)
		{
			if ( obj is Base36 )
				return this == (Base36)obj;
			else
				return false;
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// Returns a string representation padding the leading edge with
		/// zeros if necessary to make up the number of characters
		/// </summary>
		/// <param name="MinimumDigits">The minimum number of digits that the string must contain</param>
		/// <returns>The padded string representation</returns>
		public string ToString(int MinimumDigits)
		{
			string base36Value = Base36.NumberToBase36(numericValue);

			if(base36Value.Length >= MinimumDigits)
			{
				return base36Value;
			}
			else
			{
				string padding = new string('0', (MinimumDigits - base36Value.Length));
				return string.Format("{0}{1}", padding, base36Value);
			}
		}
		#endregion
	}

	public class InvalidBase36CharacterValueException : Exception
	{
		#region Private Fields

		private char value;

		#endregion

		#region Internal Constructor

		internal InvalidBase36CharacterValueException(char value)
		{
			this.value = value;
		}
		#endregion

		#region Public Override Methods

		public override string Message
		{
			get
			{
				return string.Format("The character {0} is not a valid base-36 character", value);
			}
		}
		#endregion
	 }

	public class InvalidBase36DigitValueException : Exception
	{
		#region Private Fields

		private byte value;

		#endregion

		#region Internal Constructor

		internal InvalidBase36DigitValueException(byte value)
		{
			this.value = value;
		}

		#endregion

		#region Public Override Methods

		public override string Message
		{
			get
			{
				return string.Format("The value {0} could not be converted to a single base-36 digit", value);
			}
		}

		#endregion
	}

	public class InvalidBase36NumberException : Exception
	{
		#region Private Fields
		private ulong value;
		#endregion

		#region Internal Constructor

		internal InvalidBase36NumberException(ulong value)
		{
			this.value = value;
		}
		#endregion

		#region Public Override Methods

		public override string Message
		{
			get
			{
				return string.Format("The number {0} could not be converted to a valid base-36 string", value);
			}
		}
		#endregion
	}

	public class InvalidBase36StringException : Exception
	{
		#region Private Fields
		private string value;
		#endregion

		#region Internal Constructor

		internal InvalidBase36StringException(string value)
		{
			this.value = value;
		}

		#endregion
		#region Public Override Methods

		public override string Message
		{
			get
			{
				return string.Format("The string {0} is not a valid base-36 number", value);
			}
		}
		#endregion
	}
}