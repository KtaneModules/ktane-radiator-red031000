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
			public Indicators()
			{
				WidgetType = WidgetType.Indicator;
			}
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

			public override string ToString()
			{
				if (IndicatorState == IndicatorState.Colored)
				{
					return IndicatorColor.ToString() + " " + IndicatorName.ToString();
				} else if (IndicatorState == IndicatorState.Lit)
				{
					return "lit " + IndicatorName.ToString();
				} else if (IndicatorState == IndicatorState.Unlit)
				{
					return "unlit " + IndicatorName.ToString();
				}
				else
					return base.ToString();
			}
		}

		public class Ports : Widget
		{
			public Ports()
			{
				WidgetType = WidgetType.Port;
			}
			/// <summary>
			/// The name of the port.
			/// </summary>
			/// <example>Port.Serial</example>
			public Port PortName;
			public override string ToString()
			{
				switch (PortName)
				{
					case Port.RJ45:
						return "RJ-45";
					case Port.StereoRCA:
						return "Stereo RCA";
					case Port.PS2:
						return "PS/2";
					case Port.ComponentVideo:
						return "Component Video";
					case Port.CompositeVideo:
						return "Composite Video";
					default:
						return PortName.ToString();
				}
			}
		}

		public class Batteries : Widget
		{
			public Batteries()
			{
				WidgetType = WidgetType.Batteries;
			}
			/// <summary>
			/// The name of the battery.
			/// </summary>
			/// <example>Battery.D</example>
			public Battery BatteryName;
			public override string ToString()
			{
				return BatteryName.ToString();
			}
		}

		public class TwoFactor : Widget
		{
			public TwoFactor()
			{
				WidgetType = WidgetType.TwoFactor;
			}

			public override string ToString()
			{
				return "Two Factor Code";
			}
		}

		public class Widget
		{
			/// <summary>
			/// The type of Widget.
			/// </summary>
			public WidgetType WidgetType;
		}

		public RadiatorRules(int seed)
		{
			switch (seed)
			{
				case 1:
					InitializeDefaults();
					break;
				default:
					//init seeds here
					break;
			}
		}

		private void InitializeDefaults()
		{
			RuleGeneratorSeed = 1;
			UnicornIndicator1 = new Indicators()
			{
				IndicatorName = Indicator.FRK,
				IndicatorState = IndicatorState.Lit
			};
			UnicornIndicator2 = new Indicators()
			{
				IndicatorName = Indicator.BOB,
				IndicatorState = IndicatorState.Lit
			};
			UnicornTempValue = 13;
			UnicornWaterValue = 37;
			SerialNumberLetters = SerialNumberOptions[0];
			TempSerialAmount = 10;
			AddToNumberObject = new Batteries()
			{
				BatteryName = Battery.AA
			};
			AddToNumberAmount = 5;
			TakeFromNumberObject = new Batteries()
			{
				BatteryName = Battery.D
			};
			TakeFromNumberAmount = 5;
			WaterInitialOperation = Operation.Division;
			WaterInitialAmount = 3;
			WaterAddToNumberLargeObject = new Ports()
			{
				PortName = Port.RJ45
			};
			WaterAddToNumberLAmount = 50;
			WaterAddToNumberSmallObject = new Indicators()
			{
				IndicatorState = IndicatorState.Lit
			};
			WaterAddToNumberSAmount = 20;
			TableObject = new Indicators()
			{
				IndicatorState = IndicatorState.Unlit
			};
			TableObject1 = new Indicators()
			{
				IndicatorName = Indicator.BOB,
				IndicatorState = IndicatorState.Unlit
			};
			TableObject2 = new Indicators()
			{
				IndicatorName = Indicator.NSA,
				IndicatorState = IndicatorState.Unlit
			};
			TableObject3 = new Indicators()
			{
				IndicatorName = Indicator.FRQ,
				IndicatorState = IndicatorState.Unlit
			};
			TableObject4 = new Indicators()
			{
				IndicatorName = Indicator.MSA,
				IndicatorState = IndicatorState.Unlit
			};
			TableObject5 = new Indicators()
			{
				IndicatorName = Indicator.FRK,
				IndicatorState = IndicatorState.Unlit
			};
			TableObject1Value = 40;
			TableObject2Value = -10;
			TableObject3Value = 2;
			TableObject4Value = 25;
			TableObject5Value = -1;
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
		Modulo,
		Squared,
		SquareRoot,
	}
}
