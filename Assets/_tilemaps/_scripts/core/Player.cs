using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Player {

    public Player(string playerName) {
        this.PlayerName = playerName;
        units = new HashSet<Unit>();
        cities = new HashSet<City>();
    }

    public string PlayerName {
        get ;
        protected set ;
    }

    private HashSet<Unit> units;
    private HashSet<City> cities;

    public enum PlayerType { LOCAL, AI, REMOTE };
    public PlayerType Type = PlayerType.LOCAL;

    public Unit[] Units {
        get {
            return units.ToArray();
        }
    }

    public City[] Cities {
        get {
            return cities.ToArray();
        }
    }

    public void AddUnit( Unit u ) {
        units.Add( u );
        u.OnObjectDestroyed += OnUnitDestroyed;
    }

    public void AddCity( City c ) {
        cities.Add( c );
    }

    public void OnUnitDestroyed( MapObject mo ) {
        units.Remove( ( Unit ) mo );
    }

    public void OnCityDestroyed( MapObject mo ) {
        cities.Remove( ( City ) mo );
    }

}
