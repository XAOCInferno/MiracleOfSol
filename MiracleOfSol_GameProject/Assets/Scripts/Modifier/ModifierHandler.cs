using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierHandler : MonoBehaviour
{
    public string ModUniqueName = "__Default";
    public string ApplicationType = ""; //Entity, Squad, EntityType, SquadType, WeaponType, SquadWeapons, SquadWeaponsType
    public float ProbabilityOfApplying = 9999999;
    public bool Exclusive = false;
    public string TargetTypeName = ""; //Only if apply to a 'Type'
    public string UsageType = ""; //Enabled, Multiplication, Addition
    public float Value = 9999999;
    public int UniqueID = 0; //DO NOT EDIT ME!
    public bool ThisModifierIsBroken = false;
    private string ModFilePath = "";

    public void InitiateModValues(string l_ModName = "__Default", string l_ApplicationType = "", bool l_Exclusive = false, float l_ProbabilityOfApplying = 9999999, string l_TargetTypeName = "", string l_UsageType = "", float l_Value = 9999999)
    {
        //This is to import values to custom modify your modifier :) eg you want to halve max HP, bring name as MaxHP, application type as Multiplication and value as 0.5
        ModUniqueName = l_ModName; ApplicationType = l_ApplicationType; Exclusive = l_Exclusive; ProbabilityOfApplying = l_ProbabilityOfApplying; TargetTypeName = l_TargetTypeName; UsageType = l_UsageType; Value = l_Value;

        //This is used to check if the file basic settings exist
        GetFilePath();
        /*print("ABILITY IS DONE!\n ModUniqueName: " + ModUniqueName + "\n" + "ApplicationType: " + ApplicationType + "\n" + "Probability: " +
            ProbabilityOfApplying + "\n" + "Exclusive: " + Exclusive + "\n" + "UsageType: " + UsageType + "\n" + "Value: " + Value);*/
    }

    private void GetFilePath()
    {
        ModFilePath = "Assets/attrib/modifiers/" + ModUniqueName + ".txt";

        if (System.IO.File.Exists(ModFilePath))
        {
            //This is used to import default values, if something was not defined in the import attributes.Eg you can just change name to
            CheckAndImportDefaultSettings();
        }
        else
        {
            //The file blueprint does not exist, therefore there is no code for that file in the game and it cannot be used
            Debug.LogError("ERROR: in ModifierHandler/GetFilePath. The file '" + ModFilePath + "' does not exist! The modifier cannot be created or implemented.");
            ThisModifierIsBroken = true;
        }

    }

    public void CheckAndImportDefaultSettings()
    {
        string[] lines = System.IO.File.ReadAllLines(ModFilePath);
        float tmp_ProbOfApply = ProbabilityOfApplying;
        float tmp_Value = Value;

        UniqueID = 0;
        bool NextLineIsProbability = false;
        bool NextLineIsValue = false;
        foreach (string line in lines)
        {
            string new_word = "";

            if (NextLineIsProbability)
            {
                if (ProbabilityOfApplying == 9999999)
                {
                    tmp_ProbOfApply = float.Parse(line);
                    print("WE ARE DOING PROBABILITY!!!" + "\n" + line + "\n" + tmp_ProbOfApply);
                    if(tmp_ProbOfApply >= 1) { tmp_ProbOfApply = 1; }else if(tmp_ProbOfApply <= 0) { tmp_ProbOfApply = 0; }
                    NextLineIsProbability = false;
                }
            }
            else if (NextLineIsValue)
            {
                if (tmp_Value == 9999999)
                {
                    tmp_Value = float.Parse(line);
                    NextLineIsValue = false;
                }
            }
            else 
            {
                foreach (char letter in line)
                {
                    if (letter != '#')
                    {
                        new_word += letter;
                        new_word = new_word.ToUpper();
                        if (new_word == "PROBABILITYOFAPPLYING") { NextLineIsProbability = true; }
                        else if (new_word == "VALUE") { NextLineIsValue = true; };
                    }
                    else
                    {
                        break;
                    }
                }

                CheckWordIsImportant(new_word);
            }
        }

        SetNumericalInfo(tmp_ProbOfApply, tmp_Value);
    }

    private void SetNumericalInfo(float ApplicationProb, float ModValue)
    {
        ProbabilityOfApplying = ApplicationProb;
        Value = ModValue;
    }

    private void CheckWordIsImportant(string word)
    {
        if (UniqueID == 0)
        {
            char[] WordToChar = word.ToCharArray();
            if (WordToChar.Length > 0)
            {

                if (WordToChar[0] == '$')
                {
                    word = "";
                    for (int letter = 1; letter < WordToChar.Length; letter++)
                    {
                        word += WordToChar[letter];
                    }

                    UniqueID = int.Parse(word);
                }
            }
        }

        if (word == "TRUE") { Exclusive = true; }
        else if (word == "FALSE") { Exclusive = false; }
        else if (word == "ENTITY" || word == "SQUAD" || word == "ENTITYTYPE" || word == "SQUADTYPE" || word == "WEAPONTYPE" || word == "SQUADWEAPONS" || word == "SQUADWEAPONSTYPE") { if (ApplicationType == "") { ApplicationType = word; print("WE ARE APPLYING APPLICATION TYPE"); } /*else { print("APPLICATION TYPE HAS BEEN APPLIED!"); }*/ }
        else if (word == "ENABLED" || word == "MULTIPLICATION" || word == "ADDITION") { { if (UsageType == "") { UsageType = word; } } }

    }
}
