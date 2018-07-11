using System;

namespace Assets.Scripts.RuleGenerator
{
	public partial class RadiatorRuleGenerator : AbstractRuleGenerator
	{
		public static RadiatorRuleGenerator Instance
		{
			get { return (RadiatorRuleGenerator)GetInstance<RadiatorRuleGenerator>(); }
		}

		public override string GetModuleType()
		{
			return "radiator";
		}

		public override void CreateRules()
		{
			if (!Initialized)
				throw new Exception("You must initialize the Random number generator first");

			/*switch (Seed)
			{
				case 1:

			}*/
		}

		public override string GetHTMLManual(out string filename)
		{
			filename = "Radiator.html";
			if (!Initialized)
				throw new Exception("You must Initialize the Random number generator first.");
			if (!RulesGenerated)
				throw new Exception("You must generate the rules first");

			return new RadiatorManual().Manual.Replace("RULEGENERATORSEED", Seed.ToString()); //add replaces here once RNG has been implemented
		}

		public override string[] GetTextFiles(out string[] textFilePaths)
		{
			textFilePaths = RadiatorManual.TextAssetPaths;
			return RadiatorManual.TextAssets;
		}
	}
}