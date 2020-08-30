using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DragonsClaw.Models
{
    public class PlayerViewModel
    {
        public int PlayerId { get; set; }
        public bool IsAdmin { get; set; }
        public string PlayerName { get; set; }
        public string SectorName { get; set; }
        public int Credits { get; set; }
        public int Food { get; set; }
        public int Dilithium { get; set; }
        public int Ore { get; set; }
        public int Planets { get; set; }
        public int Explore { get; set; }
        public int PsionicEnergy { get; set; }
        public int Population { get; set; }
        public int Employed { get; set; }
        public int Cadets { get; set; }
        public int OffensiveTroops { get; set; }
        public int DefensiveTroops { get; set; }
        public int SpecialtyTroops { get; set; }
        public int Mercenaries { get; set; }
        public int Spies { get; set; }
        public int Raiders { get; set; }
        public int Fighters { get; set; }
        public int Bombers { get; set; }
        public int Cruisers { get; set; }
        public int Destroyers { get; set; }
        public int Dreadnaughts { get; set; }
        public int Psionists { get; set; }
        public int PsionicCrystals { get; set; }
        public int Gender { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int RaceId { get; set; }
        public string RaceName { get; set; }
        public int Admirals { get; set; }
        public bool Observer { get; set; }
        public int NetWorth { get; set; }
        public int NetWorthPerPlanet { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int Stealth { get; set; }
        public int Terabytes { get; set; }
        public int Happiness { get; set; }
        public long DefenseAtHome { get; internal set; }
        public long OffenseAtHome { get; internal set; }
        public long Defense { get; internal set; }
        public long Offense { get; internal set; }
        public float DefEfficiency { get; internal set; }
        public float OffEfficiency { get; internal set; }
        public int Exploring { get; internal set; }
    }
}