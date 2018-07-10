﻿using UnityEngine;
using Radiator;
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

    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;
    private int SerialOccurances = 0;

    private bool _isSolved = false, _lightsOn = false;

    #endregion

    #region Answer Calculation
    private int GetSerialOccurances()
    {
        int occur = Info.GetSerialNumber().Count("RADI4T07".Contains); //get every occurance of the string RADI4T07 in the serial number
        return occur;
    }


    // Use this for initialization
    void Start () {
        _moduleId = _moduleIdCounter++;
        Module.OnActivate += Activate;
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

        if (KMBombInfoExtensions.IsIndicatorOn(Info, Indicator.FRK) && KMBombInfoExtensions.IsIndicatorOn(Info, Indicator.BOB))
        { //unicorn
            TemperatureAns = 13;
            WaterAns = 37;
            Debug.LogFormat("[Radiator #{0}] Answer is unicorn. Horay!", _moduleId);
            Debug.LogFormat("[Radiator #{0}] Temperature answer: {1}.", _moduleId, TemperatureAns);
            Debug.LogFormat("[Radiator #{0}] Water answer: {1}.", _moduleId, WaterAns);
        }
        else
        {
            if (SerialOccurances != 0)
            {

                TemperatureAns += (10 * SerialOccurances); //find every occurance of letters RADITO and add that * 10 to the number
                Debug.LogFormat("[Radiator #{0}] Added " + (10 * SerialOccurances) + " to the temperature answer (serial occurances).", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Temperature is now {1}.", _moduleId, TemperatureAns);
            }

            if (!KMBombInfoExtensions.GetBatteryCount(Info).Equals(0))
            {
                int NumAdded = 0;
                int NumTaken = 0;
                int AAnum = Info.GetBatteryHolderCount(Battery.AA);
                int Dnum = Info.GetBatteryHolderCount(Battery.D);
                if (AAnum > 0)
                {
                    for (int i = 0; i < AAnum; i++) //add for AA
                    {
                        TemperatureAns += 5;
                        NumAdded += 5;
                    }
                    Debug.LogFormat("[Radiator #{0}] Added {1} to the temperature answer (AA batteries).", _moduleId, NumAdded);
                    Debug.LogFormat("[Radiator #(0)] Temperature is now {1}.", _moduleId, TemperatureAns);
                }

                if (Dnum > 0)
                {
                    for (int i = 0; i < Dnum; i++) //subtract for D
                    {
                        TemperatureAns -= 5;
                        NumTaken += 5;
                    }
                    Debug.LogFormat("[Radiator #{0}] Taken {1} from the temperature answer (D batteries).", _moduleId, NumTaken);
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

            WaterAns = (int) (TemperatureAns / 3); //for the initial water answer, cast to int to avoid horrible decimals
            Debug.LogFormat("[Radiator #{0}] Initial water value is {1}.", _moduleId, WaterAns);

            if(KMBombInfoExtensions.IsPortPresent(Info, Port.RJ45))
            {
                WaterAns += 50; //add 50 for RJ-45
                Debug.LogFormat("[Radiator #{0}] Adding 50 to the water value (RJ-45).", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }

            if(Info.GetOnIndicators().Any())
            {
                WaterAns += 20; //add 20 for any lit indicators
                Debug.LogFormat("[Radiator #{0}] Adding 20 to the water value (lit indicators).", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }

            //indicator table
            if(Info.IsIndicatorOff(Indicator.BOB))
            {
                WaterAns += 40; //add 40 for unlit bob
                Debug.LogFormat("[Radiator #{0}] Adding 40 to the water value (unlit BOB).", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }
            if (Info.IsIndicatorOff(Indicator.NSA))
            {
                WaterAns -= 10; //subtract 10 for unlit nsa
                Debug.LogFormat("[Radiator #{0}] Taking 10 from the water value (unlit BOB).", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }
            if (Info.IsIndicatorOff(Indicator.FRQ))
            {
                WaterAns += 2; //add 2 for unlit frq
                Debug.LogFormat("[Radiator #{0}] Adding 2 to the water value (unlit FRQ).", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }
            if (Info.IsIndicatorOff(Indicator.MSA))
            {
                WaterAns += 25; //add 25 for unlit msa
                Debug.LogFormat("[Radiator #{0}] Adding 25 to the water value (unlit MSA).", _moduleId);
                Debug.LogFormat("[Radiator #{0}] Water answer is now {1}.", _moduleId, WaterAns);
            }
            if (Info.IsIndicatorOff(Indicator.FRK))
            {
                WaterAns -= 1; //subtract 1 for unlit frk
                Debug.LogFormat("[Radiator #{0}] Taking 1 from the water value (unlit FRK).", _moduleId);
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
    private string TwitchHelpMessage = "Submit the temperature and water together with !{0} submit 12 34. Reset with !{0} reset.";
    private string TwitchManualCode = "Radiator";
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
