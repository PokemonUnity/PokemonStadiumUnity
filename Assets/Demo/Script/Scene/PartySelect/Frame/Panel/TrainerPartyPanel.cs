using PokemonEssentials.Interface.PokeBattle;
using PokemonUnity.Monster;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace PokemonUnity.Stadium
{
	/// <summary>
	/// This is a container to hold objects, it doesnt really need any logic
	/// </summary>
	[RequireComponent(typeof(ToggleGroup))]
	public class TrainerPartyPanel : MonoBehaviour //, IEventSystemHandler, ISelectHandler, IDeselectHandler, ISubmitHandler//, IUpdateSelectedHandler
	{
		#region Variables
		public TrainerPokemonButton[] party;
		public TrainerPokemonButton pokemonButtonPrefab;
		/// <summary>
		/// Nested <see cref="UnityEngine.UI.LayoutGroup"/> to hold the party pokemon
		/// </summary>
		public GameObject partyContentFrame; //ToDo: What is this?

		public Text trainerName;
		public Text trainerId;
		#endregion

		#region Unity
		private void Awake()
		{
			//Clear child objects
			foreach (Transform child in partyContentFrame.transform)
				Destroy(child.gameObject);

			//toggleGroup = GetComponent<ToggleGroup>();
			party = new TrainerPokemonButton[Core.MAXPARTYSIZE]; //Should be 6, or Game.GameData.Global.Features.LimitPokemonPartySize
			//foreach(TrainerPokemonButton pokemon in party)
			//for(int i = 0; i < party.Length; i++)
			//{
			//	//Instantiate new Prefab to Scene
			//	TrainerPokemonButton pokemon = Instantiate(pokemonButtonPrefab, partyContentFrame.transform);
			//
			//	// Set the width/height (Fix this code => There's nothing to fix as Unity components are set to automatically adjust size, and position)
			//	//var layoutElement = pokemon.GetComponent<LayoutElement>();
			//	//layoutElement.preferredWidth = 125;
			//	//layoutElement.preferredHeight = 41;
			//
			//	pokemon.partyIndex = i;
			//	pokemon.toggle.group = GetComponent<ToggleGroup>(); //toggleGroup;
			//	pokemon.toggle.interactable = false;
			//	pokemon.name = "Slot" + i;
			//	//pokemon.PokemonSelect = PokemonSelect;
			//	party[i] = pokemon;
			//}
			SetTrainerID(0);
			//RefreshPartyDisplay();
		}
		#endregion

		#region Methods
		public void AddPokemonToParty(IPokemon pokemon, int index)
		{
            //Instantiate new Prefab to Scene
            TrainerPokemonButton tainerPokemonButton = Instantiate(pokemonButtonPrefab, partyContentFrame.transform);
            tainerPokemonButton.partyIndex = index;
            tainerPokemonButton.toggle.group = GetComponent<ToggleGroup>();
            tainerPokemonButton.toggle.interactable = false;
            tainerPokemonButton.name = "Slot" + index;
            tainerPokemonButton.SetDisplay(pokemon);

            party[index] = tainerPokemonButton;
        }
		public void SetTrainerID(int ID, string name = null)
		{
			trainerName.text = string.IsNullOrWhiteSpace(name) ? "Trainer" : name.TrimEnd();
			trainerId.text = $"ID {ID:00000}".TrimEnd();
		}
		#endregion
	}
}