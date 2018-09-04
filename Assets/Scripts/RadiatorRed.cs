using UnityEngine;
using KModkit;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Collections;

public class RadiatorRed : MonoBehaviour {
    #region GlobalVariables
    public KMBombModule Module;
    public KMBombInfo Info;
    public KMSelectable[] NumpadPress;
    public KMSelectable Reset, Submit;
	public KMRuleSeedable RuleSeedable;
    public TextMesh Screen;
    public KMAudio Audio;
    public Material DigitsMat;
    public Font DigitsFont;

    private int WaterAns = 0;
    private int TemperatureAns = 0;
    private int WaterInput = 0;
    private int TemperatureInput = 0;
    private int Stage = 1;
    private int Digits = 0;
    private bool Generated = false;
	MonoRandom rnd;

	private static int _moduleIdCounter = 1;
    private int _moduleId = 0;
    private int SerialOccurances = 0;

    private bool _isSolved = false, _lightsOn = false;

	#endregion

	#region Rule Seed suppport

	private Indicator[] LitIndicators;
	private Indicator[] UnlitIndicators;
	private string SerialString;
	private int TempMultiplier, BatteryAddAmount, BatterySubtractAmount, UnicornTempAmount, UnicornWaterAmount, DivideAmount;
	private Battery[] BatteryOrder;
	private Port[] PortOrder;
	private int[] GeneralAmounts;

	#endregion

	#region Answer Calculation
	private int GetSerialOccurances()
    {
        int occur = Info.GetSerialNumber().Count(SerialString.Contains); //get every occurance of the string RADI4T07 in the serial number
        return occur;
    }

    // Use this for initialization
    void Start () {
        _moduleId = _moduleIdCounter++;
		rnd = RuleSeedable.GetRNG();
		UsesVanillaRuleModifierAPI = rnd.Seed != 1;
		if (rnd.Seed == 1)
		{
			LitIndicators = new Indicator[] { Indicator.FRK, Indicator.BOB, Indicator.CAR, Indicator.CLR, Indicator.FRQ, Indicator.IND, Indicator.MSA, Indicator.NLL, Indicator.NSA, Indicator.SIG, Indicator.SND, Indicator.TRN };
			UnlitIndicators = new Indicator[] { Indicator.BOB, Indicator.NSA, Indicator.FRQ, Indicator.MSA, Indicator.FRK, Indicator.CAR, Indicator.CLR, Indicator.IND, Indicator.NLL, Indicator.SIG, Indicator.SND, Indicator.TRN };
			SerialString = "RADI4T07";
			TempMultiplier = 10;
			BatteryAddAmount = 5;
			BatterySubtractAmount = 5;
			UnicornTempAmount = 13;
			DivideAmount = 3;
			UnicornWaterAmount = 37;
			BatteryOrder = new Battery[] { Battery.D, Battery.AA, Battery.AAx3, Battery.AAx4, Battery.Empty };
			PortOrder = new Port[] { Port.RJ45, Port.HDMI, Port.Parallel, Port.AC, Port.ComponentVideo, Port.CompositeVideo, Port.DVI, Port.PCMCIA, Port.PS2, Port.Serial, Port.StereoRCA, Port.USB, Port.VGA };
			GeneralAmounts = new int[] { 50, 20, 40, 10, 2, 25, 1 };
		}
		else
		{
			LitIndicators = new Indicator[] { Indicator.FRK, Indicator.BOB, Indicator.CAR, Indicator.CLR, Indicator.FRQ, Indicator.IND, Indicator.MSA, Indicator.NLL, Indicator.NSA, Indicator.SIG, Indicator.SND, Indicator.TRN }.OrderBy(x => rnd.NextDouble()).ToArray();
			UnlitIndicators = new Indicator[] { Indicator.BOB, Indicator.NSA, Indicator.FRQ, Indicator.MSA, Indicator.FRK, Indicator.CAR, Indicator.CLR, Indicator.IND, Indicator.NLL, Indicator.SIG, Indicator.SND, Indicator.TRN }.OrderBy(x => rnd.NextDouble()).ToArray();
			SerialString = CreateRandomString(8, rnd);
			TempMultiplier = rnd.Next(5, 16);
			BatteryAddAmount = rnd.Next(0, 8);
			BatterySubtractAmount = rnd.Next(0, 7);
			UnicornTempAmount = rnd.Next(0, 100);
			DivideAmount = rnd.Next(2, 6);
			UnicornWaterAmount = rnd.Next(1, 100);
			BatteryOrder = new Battery[] { Battery.D, Battery.AA, Battery.AAx3, Battery.AAx4, Battery.Empty }.OrderBy(x => rnd.NextDouble()).ToArray();
			PortOrder = new Port[] { Port.RJ45, Port.HDMI, Port.Parallel, Port.AC, Port.ComponentVideo, Port.CompositeVideo, Port.DVI, Port.PCMCIA, Port.PS2, Port.Serial, Port.StereoRCA, Port.USB, Port.VGA }.OrderBy(x => rnd.NextDouble()).ToArray();
			GeneralAmounts = new int[] { 50, 20, 40, 10, 2, 25, 1 }.OrderBy(x => rnd.NextDouble()).ToArray();
		}
		Module.OnActivate += Activate;
	}

	private static string CreateRandomString(int stringLength, MonoRandom rnd)
	{
		const string possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		char[] chars = new char[stringLength];
		for (int i = 0; i < stringLength; i++)
		{
			char character = possibleChars[rnd.Next(0, possibleChars.Length)];
			while (chars.Contains(character))
			{
				character = possibleChars[rnd.Next(0, possibleChars.Length)];
			}
			chars[i] = character;
		}
		return new string(chars);
	}

	private void Awake()
    {
        Reset.OnInteract += delegate ()
        {
            ResetHandler();
            return false;
        };
        Submit.OnInteract += delegate ()
        {
            SubmitHandler();
            return false;
        };
        for (int i = 0; i < 10; i++)
        {
            int b = i;
            NumpadPress[i].OnInteract += delegate ()
            {
                NumPadHandler(b);
                return false;
            };
        }
    }

    void Activate()
    {
        Init();
        _lightsOn = true;
    }


    void Init()
    {
        if (!Generated)
        {
            SerialOccurances = GetSerialOccurances();
            Debug.LogFormat("[Radiator #{0}] Serial occurances: {1}.", _moduleId, SerialOccurances);
            GenerateAnswer();
            Screen.GetComponent<Renderer>().material = DigitsMat;
            Screen.font = DigitsFont;
        }
        //reset
        WaterInput = 0;
        TemperatureInput = 0;
        Stage = 1;
        Screen.color = new Color32(255, 0, 0, 255); //Set text back to red
        Screen.text = "";
        Digits = 0;
    }

    void GenerateAnswer() //generate the answer
    {
        
        //temperature answer

        if (KMBombInfoExtensions.IsIndicatorOn(Info, LitIndicators[0]) && KMBombInfoExtensions.IsIndicatorOn(Info, LitIndicators[1]))
        { //unicorn
            TemperatureAns = UnicornTempAmount;
            WaterAns = UnicornWaterAmount;
            Debug.LogFormat("[Radiator #{0}] Answer is unicorn. Horay!", _moduleId);
            Debug.LogFormat("[Radiator #{0}] Temperature answer: {1}.", _moduleId, TemperatureAns);
            Debug.LogFormat("[Radiator #{0}] Water answer: {1}.", _moduleId, WaterAns);
        }
        else
        {
            if (SerialOccurances != 0)
            {

                TemperatureAns += (TempMultiplier * SerialOccurances); //find every occurance of letters RADITO and add that * 10 to the number
                Debug.LogFormat("[Radiator #{0}] Added " + (TempMultiplier * SerialOccurances) + " to the temperature answer (serial occurances).", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Temperature is now {1}.", _moduleId, TemperatureAns);
            }

            if (!KMBombInfoExtensions.GetBatteryCount(Info).Equals(0))
            {
                int NumAdded = 0;
                int NumTaken = 0;
                int AAnum = Info.GetBatteryHolderCount(BatteryOrder[1]);
                int Dnum = Info.GetBatteryHolderCount(BatteryOrder[0]);
                if (AAnum > 0)
                {
                    for (int i = 0; i < AAnum; i++) //add for AA
                    {
                        TemperatureAns += BatteryAddAmount;
                        NumAdded += BatteryAddAmount;
                    }
                    Debug.LogFormat("[Radiator #{0}] Added {1} to the temperature answer ({2} batteries).", _moduleId, NumAdded, BatteryOrder[1].ToString());
                    Debug.LogFormat("[Radiator #(0)] Temperature is now {1}.", _moduleId, TemperatureAns);
                }

                if (Dnum > 0)
                {
                    for (int i = 0; i < Dnum; i++) //subtract for D
                    {
                        TemperatureAns -= BatterySubtractAmount;
                        NumTaken += BatterySubtractAmount;
                    }
                    Debug.LogFormat("[Radiator #{0}] Taken {1} from the temperature answer ({2} batteries).", _moduleId, NumTaken, BatteryOrder[1].ToString());
                    Debug.LogFormat("[Radiator #{0}] Temperature is now {1}.", _moduleId, TemperatureAns);
                }

            }

            if (TemperatureAns < 0)
            {
                TemperatureAns *= -1; //multiply by -1 to make it positive
                Debug.LogFormat("[Radiator #{0}] Temperature answer is negative, multiplying by -1 to make it positive.", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Temparature answer is now {1}.", _moduleId, TemperatureAns);
   
            }

            //water answer

            WaterAns = TemperatureAns / DivideAmount; //cast it automatic, as we're not dividing by a float
            Debug.LogFormat("[Radiator #{0}] Initial water value is {1}.", _moduleId, WaterAns);

            if(KMBombInfoExtensions.IsPortPresent(Info, PortOrder[0]))
            {
                WaterAns += GeneralAmounts[0]; //add 50 for RJ-45
                Debug.LogFormat("[Radiator #{0}] Adding " + GeneralAmounts[0].ToString() + " to the water value (" + PortOrder[0].ToString() + ").", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }

            if(Info.GetOnIndicators().Any())
            {
                WaterAns += GeneralAmounts[1]; //add 20 for any lit indicators
                Debug.LogFormat("[Radiator #{0}] Adding " + GeneralAmounts[1] + " to the water value (lit indicators).", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }

            //indicator table
            if(Info.IsIndicatorOff(UnlitIndicators[0]))
            {
                WaterAns += GeneralAmounts[2]; //add 40 for unlit bob
                Debug.LogFormat("[Radiator #{0}] Adding " + GeneralAmounts[2] + " to the water value (unlit " + UnlitIndicators[0].ToString() + ").", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }
            if (Info.IsIndicatorOff(UnlitIndicators[1]))
            {
                WaterAns -= GeneralAmounts[3]; //subtract 10 for unlit nsa
                Debug.LogFormat("[Radiator #{0}] Taking " + GeneralAmounts[3] + " from the water value (unlit " + UnlitIndicators[1].ToString() + ").", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }
            if (Info.IsIndicatorOff(UnlitIndicators[2]))
            {
                WaterAns += GeneralAmounts[4]; //add 2 for unlit frq
                Debug.LogFormat("[Radiator #{0}] Adding " + GeneralAmounts[4] + " to the water value (unlit " + UnlitIndicators[2].ToString() + ").", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }
            if (Info.IsIndicatorOff(UnlitIndicators[3]))
            {
                WaterAns += GeneralAmounts[5]; //add 25 for unlit msa
                Debug.LogFormat("[Radiator #{0}] Adding " + GeneralAmounts[5] + " to the water value (unlit " + UnlitIndicators[3].ToString() + ").", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }
            if (Info.IsIndicatorOff(UnlitIndicators[4]))
            {
                WaterAns -= GeneralAmounts[6]; //subtract 1 for unlit frk
                Debug.LogFormat("[Radiator #{0}] Taking 1 from the water value (unlit " + UnlitIndicators[4].ToString() + ").", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }

            if (WaterAns < 0)
            {
                WaterAns *= -1; //multiply by -1 to make it positive
                Debug.LogFormat("[Radiator #{0}] Water answer is negative, multiplying by -1 to make it positive.", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }

            //final clean up

            if (WaterAns > 99)
            {
                WaterAns %= 100;
            }
            if (TemperatureAns > 99)
            {
                TemperatureAns %= 100;
            }
            Debug.LogFormat("[Radiator #{0}] Final answer for temperature is {1}.", _moduleId, TemperatureAns);
            Debug.LogFormat("[Radiator #{0}] Final answer for water is {1}.", _moduleId, WaterAns);

            Generated = true;
        }
    }
    #endregion

    #region Button Handlers
    void ResetHandler()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Reset.transform);
        Reset.AddInteractionPunch();

        if (!_lightsOn || _isSolved) return;

        if (Stage == 1)
        {
            TemperatureInput = 0;
            Screen.text = "";
            Digits = 0;
        } else if (Stage == 2)
        {
            WaterInput = 0;
            Screen.text = "";
            Digits = 0;
        }
    }
    void SubmitHandler()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Submit.transform);
        Submit.AddInteractionPunch();

        if (!_lightsOn || _isSolved) return;
        if (Stage == 1)
        {
            if (Digits == 1) //if there is only one digit, interpret it as a 1 rather than 10, for example
            {
                TemperatureInput /= 10;
            }
            Debug.LogFormat("[Radiator #{0}] Temperature: recieved {1}, expected {2}.", _moduleId, TemperatureInput, TemperatureAns);
            if (TemperatureInput == TemperatureAns)
            {
                Debug.LogFormat("[Radiator #{0}] Temperature correct!", _moduleId);
                Stage = 2;
                Screen.text = "";
                Screen.color = new Color32(0, 255, 255, 255); //Set text to cyan for water
                Digits = 0;
            } else
            {
                Debug.LogFormat("[Radiator #{0}] Temperature incorrect, Strike.", _moduleId);
                Module.HandleStrike();
                Init();
            }
        } else if (Stage == 2)
        {
            if (Digits == 1)
            {
                WaterInput /= 10;
            }
            Debug.LogFormat("[Radiator #{0}] Water: recieved {1}, expected {2}.", _moduleId, WaterInput, WaterAns);

            if (WaterInput == WaterAns)
            {
                Debug.LogFormat("[Radiator #{0}] Water correct!", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Module passed.", _moduleId);
                Screen.text = "";
                Module.HandlePass();
                _isSolved = true;
            } else
            {
                Debug.LogFormat("[Radiator #{0}] Water incorrect, Strike.", _moduleId);
                Module.HandleStrike();
                Init();
            }
        }
    }
    void NumPadHandler(int b)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, NumpadPress[b].transform);
        NumpadPress[b].AddInteractionPunch();

        if (!_lightsOn || _isSolved || Digits > 1) return;

        if (Stage == 1)
        {
            if (Digits == 0)
            {
                TemperatureInput += (b * 10);
                Screen.text = (TemperatureInput / 10).ToString();
            } else if (Digits == 1)
            {
                TemperatureInput += b;
                Screen.text = TemperatureInput.ToString();
            }
        } else if (Stage == 2)
        {
            if (Digits == 0)
            {
                WaterInput += (b * 10);
                Screen.text = (WaterInput / 10).ToString();
            }
            else if (Digits == 1)
            {
                WaterInput += b;
                Screen.text = WaterInput.ToString();
            }
        }

        Digits++;
    }
    #endregion

    #region Twitch Plays
#pragma warning disable 414
    private readonly string TwitchHelpMessage = "Submit the temperature and water together with !{0} submit 12 34. Reset with !{0} reset.";
    private readonly string TwitchManualCode = "Radiator";
	private bool UsesVanillaRuleModifierAPI;
#pragma warning restore 414
	KMSelectable[] ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();
        string[] split = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //split the string so we can handle temperature and water seperately

        if (Regex.IsMatch(command, @"^submit [0-9]{1,2} [0-9]{1,2}$")) //!# Submit 12 34
        {
            KMSelectable[] TempButtons;
            KMSelectable[] WaterButtons;
            command = command.Substring(7).Trim();
            if (split[1].Length == 2)
            {
                TempButtons = new KMSelectable[] { NumpadPress[int.Parse(split[1][0].ToString())], NumpadPress[int.Parse(split[1][1].ToString())], Submit };
            } else
            {
                TempButtons = new KMSelectable[] { NumpadPress[int.Parse(split[1][0].ToString())], Submit };
            }

            if (split[2].Length == 2)
            {
                WaterButtons = new KMSelectable[] { NumpadPress[int.Parse(split[2][0].ToString())], NumpadPress[int.Parse(split[2][1].ToString())], Submit };
            } else
            {
                WaterButtons = new KMSelectable[] { NumpadPress[int.Parse(split[2][0].ToString())], Submit };
            }
            KMSelectable[] presses = TempButtons.Concat(WaterButtons).ToArray(); //join the two arrays using linq
            return presses;
        } else if (command == "reset")
        {
            return new[] { Reset };
        }

        return null;
    }
	private IEnumerator TwitchHandleForcedSolve()
	{
		if (!_isSolved)
		{
			Debug.LogFormat("[Radiator #{0}] Module forcibly solved", _moduleId);
			yield return null;
			IEnumerable<int> TempAnsEnumberable = TemperatureAns.ToString().Select(digit => int.Parse(digit.ToString())); //take advantage of the fact that string has an enumerator
			IEnumerable<int> WaterAnsEnumberable = WaterAns.ToString().Select(digit => int.Parse(digit.ToString()));
			foreach (int digit in TempAnsEnumberable)
			{
				NumPadHandler(digit);
				yield return new WaitForSeconds(0.1f); //0.1 seconds is the normal delay
			}
			SubmitHandler();
			foreach (int digit in WaterAnsEnumberable)
			{
				NumPadHandler(digit);
				yield return new WaitForSeconds(0.1f);
			}
			SubmitHandler();
		}
	}
	#endregion
}
