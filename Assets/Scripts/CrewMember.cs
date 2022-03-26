using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CrewMember : MonoBehaviour
{
    Sprite sprite;
    Player assignedShip;
    public Dictionary<String, String> StatsList = new Dictionary<String, String>();
    List<string> names = new List<string>()
    {
        "Borb", 
        "Gollobie", 
        "Appa", 
        "Zerde", 
        "Wop", 
        "Hiluie", 
        "Cypizi", 
        "Umpadumpa", 
        "Juicepy",
        "Tereki",
        "Noodaloo",
        "Boll",
        "Pisti",
        "Habbagon",
        "Ifding",
        "Vistero",
        "Gustung",
        "Grababab",
        "Illiomi",
        "Brastabor",
        "Simmellulum",
        "Eck",
        "Eppenef",
        "Ell ell",
        "Owow",
        "Rakleb",
        "Garbagio",
        "Hematet",
        "Sal",
        "Danswig",
        "Saskahold",
        "Ooboboo",
        "Rafeefa"
    };
    new string name;
    ExperienceCounter experienceCounter;
    public int RoundsSurvived = 0;
    public Sprite Sprite;
    //private void Start()
    //{
    //    name = GetName();
    //    experienceCounter = gameObject.GetComponent<ExperienceCounter>();
    //    Sprite = GetCrewSprite();
    //}
    public void Initialize()
    {
        name = GetName();
        experienceCounter = gameObject.GetComponent<ExperienceCounter>();
        Sprite = GetCrewSprite();
    }
    private string GetName()
    {
        int rand = UnityEngine.Random.Range(0, names.Count);
        return names[rand];
    }
    public Sprite GetCrewSprite()
    {
        if(gameObject.GetComponent<SpriteRenderer>() != null)
        {
            return gameObject.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            return gameObject.GetComponent<Image>().sprite;
        }
    }
    public Sprite GetShipSprite()
    {
        Player player = GetCrewMemberPlayerShip();
        return player.GetShipSprite();
    }
    public Player GetCrewMemberPlayerShip()
    {
        Player player = gameObject.GetComponentInParent<Player>(true);
        return player;
    }
    public void ReassignToShip(Player ship)
    {
        assignedShip = ship;
        gameObject.transform.SetParent(assignedShip.GetFirstCompartment());
    }
    public void AddToRounds()
    {
        RoundsSurvived++;
    }
    public Dictionary<String, String> GetStats()
    {
        UpdateStatsList();
        return StatsList;
    }
    private void UpdateStatsList()
    {
        StatsList.Clear();
        StatsList.Add("Name", this.name);
        StatsList.Add("Active Time", (this.experienceCounter.GetActiveTimeInMinutesAndSeconds()));
        StatsList.Add("Level", this.experienceCounter.GetExperienceLevel().ToString());
        StatsList.Add("Experience", this.experienceCounter.GetExperience().ToString("F0"));
        StatsList.Add("Assigned Ship", this.GetCrewMemberPlayerShip().name);
        StatsList.Add("Rounds Survived", this.RoundsSurvived.ToString("F0"));
        //StatsList.Add("Crew", this.currentCrewSize.ToString("F0") + " / " + this.maxCrewSize.ToString("F0"));
        //GetProjectileStats();
        //GetWeaponModStats();
        //CrewMembers = GetCrewMembersList();
    }
}
