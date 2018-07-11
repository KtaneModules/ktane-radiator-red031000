using Radiator;

namespace Assets.Scripts.RuleGenerator
{
	public class RadiatorRules
	{
		#region Tracked Variables
		/// <summary>
		/// The current seed.
		/// </summary>
		public int RuleGeneratorSeed;

		/// <summary>
		/// The first unicorn indicator.
		/// </summary>
		public Indicators UnicornIndicator1;

		/// <summary>
		/// The second unicorn indicator.
		/// </summary>
		public Indicators UnicornIndicator2;

		/// <summary>
		/// The value to submit for temperature if the unicorn is chosen.
		/// </summary>
		public int UnicornTempValue;

		/// <summary>
		/// The value to submit for water if the unicorn is chosen.
		/// </summary>
		public int UnicornWaterValue;

		/// <summary>
		/// The letters in the serial number to look for.
		/// </summary>
		public string SerialNumberLetters;

		/// <summary>
		/// The amount to add for each serial number occurance.
		/// </summary>
		public int TempSerialAmount;

		/// <summary>
		/// The widget to trigger adding during the temperature stage.
		/// </summary>
		public Widget AddToNumberObject;

		/// <summary>
		/// The amount to add to the number during the temperature stage.
		/// </summary>
		public int AddToNumberAmount;

		/// <summary>
		/// The widget to trigger taking during the temperature stage.
		/// </summary>
		public Widget TakeFromNumberObject;

		/// <summary>
		/// The amount to take from the number during the temperature stage.
		/// </summary>
		public int TakeFromNumberAmount;

		/// <summary>
		/// The operation to apply to the temperature value for water.
		/// </summary>
		public Operation WaterInitialOperation;

		/// <summary>
		/// The amount to apply the operation by.
		/// </summary>
		public int WaterInitialAmount;

		/// <summary>
		/// The widget to trigger adding the larger amount during the water stage.
		/// </summary>
		public Widget WaterAddToNumberLargeObject;

		/// <summary>
		/// The amount to add to the number during the water stage.
		/// </summary>
		/// <remarks>This should be larger than WaterAddToNumberSAmount, but that's not entirely necessary</remarks>
		public int WaterAddToNumberLAmount;

		/// <summary>
		/// The widget to trigger adding the smaller amount during the water stage.
		/// </summary>
		public Widget WaterAddToNumberSmallObject;

		/// <summary>
		/// The amount to add to the number during the water stage.
		/// </summary>
		/// <remarks>This should be smaller than WaterAddToNumberLAmount, but that's not entirely necessary</remarks>
		public int WaterAddToNumberSAmount;

		//table object with prefix (pretty name of the object below)

		/// <summary>
		/// The Object to formulate a table on.
		/// </summary>
		/// <remarks>NOTE: Indicators will have the IndicatorName ignored, and instead the IndicatorStatus will be used</remarks>
		public Widget TableObject;

		/// <summary>
		/// The first object in a table
		/// </summary>
		/// <remarks>This has been left abstract intentionally, as it depends on the above Widget</remarks>
		public object TableObject1;

		/// <summary>
		/// The first value to add to the number
		/// </summary>
		public int TableObject1Value;

		/// <summary>
		/// The second object in a table
		/// </summary>
		/// <remarks>This has been left abstract intentionally, as it depends on the above Widget</remarks>
		public object TableObject2;

		/// <summary>
		/// The second value to add to the number
		/// </summary>
		public int TableObject2Value;

		/// <summary>
		/// The third object in a table
		/// </summary>
		/// <remarks>This has been left abstract intentionally, as it depends on the above Widget</remarks>
		public object TableObject3;

		/// <summary>
		/// The third value to add to the number
		/// </summary>
		public int TableObject3Value;

		/// <summary>
		/// The fourth object in a table
		/// </summary>
		/// <remarks>This has been left abstract intentionally, as it depends on the above Widget</remarks>
		public object TableObject4;

		/// <summary>
		/// The fourth value to add to the number
		/// </summary>
		public int TableObject4Value;

		/// <summary>
		/// The fifth object in a table
		/// </summary>
		/// <remarks>This has been left abstract intentionally, as it depends on the above Widget</remarks>
		public object TableObject5;

		/// <summary>
		/// The fifth value to add to the number
		/// </summary>
		public int TableObject5Value;
		#endregion


		private static readonly string[] SerialNumberOptions = new string[]
		{
			"RADI4TØ7"
		};

		public class Indicators : Widget
		{
			/// <summary>
			/// The name of the indicator.
			/// </summary>
			/// <example>Indicator.NLL</example>
			public Indicator IndicatorName;

			/// <summary>
			/// The state of the indicator.
			/// </summary>
			/// <example>IndicatorState.Lit</example>
			public IndicatorState IndicatorState;

			/// <summary>
			/// The color of the indicator.
			/// </summary>
			/// <remarks>This will be white if the indicator is lit, and it will be black if it is unlit.</remarks>
			public IndicatorColor IndicatorColor;
		}

		public class Ports : Widget
		{
			/// <summary>
			/// The name of the port.
			/// </summary>
			/// <example>Port.Serial</example>
			public Port PortName;
		}

		public class Batteries : Widget
		{
			/// <summary>
			/// The name of the battery.
			/// </summary>
			/// <example>Battery.D</example>
			public Battery BatteryName;
		}

		public class TwoFactor : Widget
		{
			//no extra stuff needed, as we don't need the two factor code.
		}

		public class Widget
		{
			/// <summary>
			/// The type of Widget.
			/// </summary>
			public WidgetType WidgetType;
		}
	}

	/// <summary>
	/// The possible states of the indicator.
	/// </summary>
	public enum IndicatorState
	{
		Colored,
		Lit,
		Unlit
	}

	/// <summary>
	/// The possible types of a Widget.
	/// </summary>
	public enum WidgetType
	{
		Port,
		Indicator,
		Batteries,
		TwoFactor
	}

	/// <summary>
	/// The possible mathematical operations that can be applied
	/// </summary>
	public enum Operation
	{
		Addition,
		Subtraction,
		Multiplication,
		Division,
		Modulo
	}
}
