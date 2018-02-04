using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Radiator;
using System.Linq;
using System.Text.RegularExpressions;

public class radiatorRed : MonoBehaviour {

    public KMBombModule Module;
    public KMBombInfo Info;
    public KMSelectable[] NumpadPress;
    public KMSelectable Reset, Submit;
    public TextMesh Screen;
    public KMAudio Audio;
    public Material digitsMat;
    public Font digitsFont;

    private int WaterAns = 0;
    private int TemperatureAns = 0;
    private int WaterInput = 0;
    private int TemperatureInput = 0;
    private int stage = 1;
    private int digits = 0;
    private bool generated = false;

    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;
    private int serialOccurances = 0;

    private bool _isSolved = false, _lightsOn = false;

    private int getSerialOccurances()
    {
        int occur = Info.GetSerialNumber().Count("RADI4TO7".Contains); //get every occurance of the string RADI4TO7 in the serial number
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
            resetHandler();
            return false;
        };
        Submit.OnInteract += delegate ()
        {
            submitHandler();
            return false;
        };
        for (int i = 0; i < 10; i++)
        {
            int b = i;
            NumpadPress[i].OnInteract += delegate ()
            {
                numPadHandler(b);
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
        if (!generated)
        {
            serialOccurances = getSerialOccurances();
            Debug.LogFormat("[Radiator #{0}] Serial occurances: {1}.", _moduleId, serialOccurances);
            generateAnswer();
            Screen.GetComponent<Renderer>().material = digitsMat;
            Screen.font = digitsFont;
        }
        //reset
        WaterInput = 0;
        TemperatureInput = 0;
        stage = 1;
        Screen.color = new Color32(255, 0, 0, 255); //Set text back to red
        Screen.text = "";
        digits = 0;
    }

    void generateAnswer() //generate the answer
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
            if (serialOccurances != 0)
            {

                TemperatureAns += (10 * serialOccurances); //find every occurance of letters RADITO and add that * 10 to the number
                Debug.LogFormat("[Radiator #{0}] Added " + (10 * serialOccurances) + " to the temperature answer (serial occurances).", _moduleId);
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

            generated = true;
        }
    }
    
    void resetHandler()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Reset.transform);
        Reset.AddInteractionPunch();

        if (!_lightsOn || _isSolved) return;

        if (stage == 1)
        {
            TemperatureInput = 0;
            Screen.text = "";
            digits = 0;
        } else if (stage == 2)
        {
            WaterInput = 0;
            Screen.text = "";
            digits = 0;
        }
    }
    void submitHandler()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Submit.transform);
        Submit.AddInteractionPunch();

        if (!_lightsOn || _isSolved) return;
        if (stage == 1)
        {
            if (digits == 1) //if there is only one digit, interpret it as a 1 rather than 10, for example
            {
                TemperatureInput /= 10;
            }
            Debug.LogFormat("[Radiator #{0}] Temperature: recieved {1}, expected {2}.", _moduleId, TemperatureInput, TemperatureAns);
            if (TemperatureInput == TemperatureAns)
            {
                Debug.LogFormat("[Radiator #{0}] Temperature correct!", _moduleId);
                stage = 2;
                Screen.text = "";
                Screen.color = new Color32(0, 255, 255, 255); //Set text to cyan for water
                digits = 0;
            } else
            {
                Debug.LogFormat("[Radiator #{0}] Temperature incorrect, Strike.", _moduleId);
                Module.HandleStrike();
                Init();
            }
        } else if (stage == 2)
        {
            if (digits == 1)
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
    void numPadHandler(int b)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, NumpadPress[b].transform);
        NumpadPress[b].AddInteractionPunch();

        if (!_lightsOn || _isSolved || digits > 1) return;

        if (stage == 1)
        {
            if (digits == 0)
            {
                TemperatureInput += (b * 10);
                Screen.text = (TemperatureInput / 10).ToString();
            } else if (digits == 1)
            {
                TemperatureInput += b;
                Screen.text = TemperatureInput.ToString();
            }
        } else if (stage == 2)
        {
            if (digits == 0)
            {
                WaterInput += (b * 10);
                Screen.text = (WaterInput / 10).ToString();
            }
            else if (digits == 1)
            {
                WaterInput += b;
                Screen.text = WaterInput.ToString();
            }
        }

        digits++;
    }


    public string TwitchHelpMessage = "Submit the temperature and water together with !{0} submit 12 34. Reset with !{0} reset. NOTE: add a 0 before the number if the number is less than 10. e.g. 09";
    public string TwitchManualCode = "https://ktane.timwi.de/HTML/Radiator.html";
    KMSelectable[] ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();

        if (Regex.IsMatch(command, @"^submit +\d\d +\d\d$")) //# Submit 12 34
        {
            command = command.Substring(7).Trim();
            return new[] { NumpadPress[int.Parse(command[0].ToString())], NumpadPress[int.Parse(command[1].ToString())], Submit, NumpadPress[int.Parse(command[3].ToString())], NumpadPress[int.Parse(command[4].ToString())], Submit };
        } else if (command == "reset")
        {
            return new[] { Reset };
        }

        return null;
    }
    // Update is called once per frame
    void Update () { /*TODO remove*/
		
	}
}
