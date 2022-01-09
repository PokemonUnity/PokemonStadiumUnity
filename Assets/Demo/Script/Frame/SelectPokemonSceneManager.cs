﻿using PokemonUnity;
using PokemonUnity.Character;
using PokemonUnity.Monster;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class SelectPokemonSceneManager : MonoBehaviour
{
	#region Variables
	public PokemonSelect SelectionState;

	[SerializeField] private PartyPanelUI partySelectionUI;
	[SerializeField] private GameObject rosterEntryPrefab;
	[SerializeField] private GameObject partyEntryPrefab;
	[SerializeField] private Transform partyGridContent;
	[SerializeField] private Transform rosterGridContent;
	[SerializeField] private ToggleGroup toggleGroup;
	
	//List
	private Dictionary<int, PartyEntryButton> PartyViewer;
	private Dictionary<int, RosterEntryButton> StoreButtonData;
	//Sprite
	public static Sprite[] PkmnType { get; private set; }
	public static Sprite[] IconSprites { get; private set; }

	//public Toggle Toggle { get { return toggleGroup.ActiveToggles().FirstOrDefault(); } }
	//public int CurrentOnParty 
	//{ 
	//	get 
	//	{
	//		//return Game.GameData.Player.Party.
	//		//PartyEntryButton party = PartyViewer
	//		//	.Values
	//		//	.SingleOrDefault(x => x.IsSelected);
	//		//	//.Select(x => x.PartySlot);
	//		//return party != null ? party.PartySlot : 0;
	//		return PokemonSelect.CurrentSelectedPartySlot;
	//	} 
	//}
	#endregion
	#region Unity Monobehavior
	void Awake()
	{
		Debug.Log("Is Scriptable Object Null? " + (SelectionState == null).ToString());
		toggleGroup = GetComponent<ToggleGroup>();
		Debug.Log("Create Dictionary for Player Party UI Mono");
		PartyViewer = new Dictionary<int, PartyEntryButton>();
		Debug.Log("Create Dictionary for Roster Entry UI Mono");
		StoreButtonData = new Dictionary<int, RosterEntryButton>();
		//Debug.Log("Create Dictionary for Temp Instantiated Pokemon Objects");
		//StorePokemon = new Dictionary<Pokemons, Pokemon>();
		//Debug.Log("Create Dictionary for Temp Viewed Pokemons");
		//ViewedRentalPokemon = new Queue<Pokemons>();
		////Debug.Log("Create Dictionary for Pokemons Selected by Player");
		////SelectedPokemons = new Dictionary<KeyValuePair<bool, int?>, int>();
		//Debug.Log("Create LookUp Table for Pokemons Selected by Player");
		//SelectedPokemons = new HashSet<KeyValuePair<KeyValuePair<bool, int?>, int>>();
		//
		Debug.Log("Load Assets for UI into Array");
		IconSprites = Resources.LoadAll<Sprite>("PokemonIcon");
		PkmnType = Resources.LoadAll<Sprite>("PokemonType");

		if (Game.DatabasePath == @"Data Source=..\..\..\\veekun-pokedex.sqlite")
		{
			try
			{
				Game.DatabasePath = @"Data Source=..\..\\veekun-pokedex.sqlite";
				Game g = new Game();
				//Game.ResetSqlConnection();
				//Debug.Log("Path to DB: " + Game.con.FileName);
			}
			catch (InvalidOperationException) { Debug.LogError("problem connecting with database"); } //ignore...
			finally
			{
				//Game.con.Open();

				Debug.Log("Is Pokemon DB Null? " + (Game.PokemonData == null).ToString());
				if (Game.PokemonData == null)
				{
					//Game.InitPokemons();
					try
					{
						Game.InitTypes();
						Game.InitNatures();
						Game.InitPokemons();
						Game.InitPokemonForms();
						Game.InitPokemonMoves();
						//Game.InitPokemonEvolutions();
						Game.InitPokemonItems();
						Game.InitMoves();
						Game.InitItems();
						Game.InitBerries();
						Game.InitTrainers();
						//Game.InitRegions();
						//Game.InitLocations();
					}
					catch (Exception) { Debug.LogError("there were some problems running sql..."); } //ignore...
				}
				Debug.Log(string.Format("Is Pokemon DB Greater than 0? {0} : {1}", 
					(Game.PokemonData.Count > 0).ToString(), Game.PokemonData.Count));
				if (Game.PokemonData.Count == 0) 
					Debug.Log("Was Pokemon DB Successfully Created? " + Game.InitPokemons());
				Debug.Log(string.Format("Is Pokemon DB Greater than 0? {0} : {1}", 
					(Game.PokemonData.Count > 0).ToString(), Game.PokemonData.Count));
			}
		}

		Debug.Log("Is Game Null? " + (Game.GameData == null).ToString());
		Debug.Log("Is Player Null? " + (Game.GameData.Player == null).ToString());
		if(Game.GameData.Player == null)
		{
			Debug.Log("Create Player Object");
			Player p = new Player();
			Debug.Log("Saving Player Object to Global Singleton");
			Game.GameData.Player = p;
		}
		Debug.Log("Is Trainer Null? " + (Game.GameData.Player.Trainer == null).ToString());
	}

	void Start()
	{
		Debug.Log("Is Game Events Null? " + (GameEvents.current == null).ToString());
		GameEvents.current.onChangePartyLineup += Scene_onChangePartyLineup;

		//RentalControlUI.DisplayPokemonSelectUI();
		SetPokemonRental();
		//party.DisplayPartyUI();
		//party.GetPartyButton();
		SetPartyButton();
		Debug.Log("Trainer Id: " + Game.GameData.Player.Trainer.TrainerID.ToString());
		//Use ID but I will leave 00000 as Example
		partySelectionUI.SetTrainerID(Game.GameData.Player.Trainer.TrainerID);
	}
	void OnDestroy()
	{
		try
		{
			GameEvents.current.onChangePartyLineup -= Scene_onChangePartyLineup;
		}
		catch (NullReferenceException) { Debug.LogError("Calling destroy before event wakes up"); } //Ignore...
	}
	#endregion
	#region Methods
	//public void SelectToggle(int id)
	//{
	//	Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
	//	toggles[id].isOn = true;
	//}
	#endregion
	#region Party Roster UI
	public void SetPartyButton()
	{
		for (int Id = 0; Id < Game.GameData.Features.LimitPokemonPartySize && Id < Core.MAXPARTYSIZE; Id++)
		{
			//if (Id == Core.MAXPARTYSIZE) break;
			GameObject Button = Instantiate(partyEntryPrefab);
			PartyEntryButton slot = Button.GetComponent<PartyEntryButton>();
			slot.PokemonSelect = SelectionState; //Should be duplicated for each player controller on screen
			//PartyData(Id, slot);
			PartyViewer.Add(Id, slot);
			//Button.GetComponent<PartyEntryButton>().ActivePartyUIButton(true);
			//Button.GetComponent<PartyEntryButton>().ActivePokemonDisplay(false);
			slot.ActivePartyUIButton(true);
			slot.ActivePokemonDisplay(false);
			//Button.transform.SetParent(partyEntryPrefab.transform.parent, false);
			Button.transform.SetParent(partyGridContent, false);
		}
	}
	private void SetPokemonRental()
	{
		int i = 0;
		//foreach (int id in ID)
		//for (int id = 1; id <= 151; id++)
		foreach (int id in 
			Game.PokemonData
			.Values
			.Where(x => x.ID != Pokemons.NONE && 
				(int)x.ID <= 1000 &&
				//x.GenerationId <= (int)Generation.RedBlueYellow)
				SelectionState.PokemonGens.Contains((Generation)x.GenerationId))
			.Select(y => (int)y.ID))
		{
			int? page = SelectionState.CurrentSelectedRosterPage;
			bool isSelected = SelectionState.SelectedPokemons.Contains(
				new KeyValuePair<KeyValuePair<bool, int?>, int>(
					//new KeyValuePair<bool, int?>(true, null), i));
					new KeyValuePair<bool, int?>(true, page), i));

			GameObject Button = Instantiate(rosterEntryPrefab);
			RosterEntryButton roster = Button.GetComponent<RosterEntryButton>();
			roster.PokemonSelect = SelectionState; //Should be duplicated for each player controller on screen
			//RentalData(id, roster);
			StoreButtonData.Add(id, roster);
			//Button.GetComponent<RosterEntryButton>().SetID(id);
			roster.SetID(i,(Pokemons)id,page:page,selected:isSelected); i++;
			Button.SetActive(true);
			Button.transform.SetParent(rosterGridContent, false);
		} Debug.Log("Highest Species Counted: " + i);
		rosterGridContent.gameObject.SetActive(true);
	}

	private void Scene_onChangePartyLineup()
	{
		Game.GameData.Player.Party.PackParty();
		foreach (PartyEntryButton item in PartyViewer.Values)
		{
			if (Game.GameData.Player.Party[item.PartySlot].IsNotNullOrNone())
			{
				//Game.GameData.Player.Party[item.PartySlot] = new Pokemon((Pokemons)PkmnSelected, LevelFixed, false);
				PartyViewer[item.PartySlot].SetDisplay(); //pkmn.Name, pkmn.Species, pkmn.Level);
				//StoreButtonData[item.PartySlot].DisableOnClick(true);
				PartyViewer[item.PartySlot].ActivePokemonDisplay(true);
			}
			else 
			{
				//Game.GameData.Player.Party[item.PartySlot] = new Pokemon((Pokemons)PkmnSelected, LevelFixed, false);
				PartyViewer[item.PartySlot].ActivePokemonDisplay(false);
				PartyViewer[item.PartySlot].SetDisplay(); //pkmn.Name, pkmn.Species, pkmn.Level);
				//StoreButtonData[item.PartySlot].DisableOnClick(true);
			}
		}
	}
	#endregion
}