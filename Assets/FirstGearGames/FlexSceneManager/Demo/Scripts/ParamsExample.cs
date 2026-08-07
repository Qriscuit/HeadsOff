using FirstGearGames.FlexSceneManager.LoadUnloadDatas;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGearGames.FlexSceneManager.Demos
{

    /// <summary>
    /// Loads a single scene, additive scenes, or both when a client
    /// enters or exits this trigger.
    /// </summary>
    public class ParamsExample : MonoBehaviour
    {
        /// <summary>
        /// Additive scene to load.
        /// </summary>
        [Tooltip("Additive scene to load.")]
        [SerializeField]
        private string _additiveScene = string.Empty;
        /// <summary>
        /// Next matchId to load players into.
        /// </summary>
        private int _matchId = 1;

        /// <summary>
        /// Scenes belonging to matchIds.
        /// </summary>
        private Dictionary<int, Scene[]> _matchScenes = new Dictionary<int, Scene[]>();

        private void OnEnable()
        {
            FlexSceneManager.OnLoadSceneEnd += FlexSceneManager_OnLoadSceneEnd;
            FlexSceneManager.OnClientPresenceChangeEnd += FlexSceneManager_OnClientPresenceChangeEnd;
        }
        private void OnDisable()
        {
            FlexSceneManager.OnLoadSceneEnd -= FlexSceneManager_OnLoadSceneEnd;
            FlexSceneManager.OnClientPresenceChangeEnd -= FlexSceneManager_OnClientPresenceChangeEnd;
        }

        /// <summary>
        /// Called after a scene load finishes.
        /// </summary>
        /// <param name="args"></param>
        private void FlexSceneManager_OnLoadSceneEnd(Events.LoadSceneEndEventArgs args)
        {
            //If not loaded as server.
            if (!args.RawData.AsServer)
                return;
            //No loaded scenes.
            if (args.LoadedScenes.Length == 0)
                return;

            //Get matchId used.
            int matchId = (int)args.RawData.LoadParams.ServerParams[0];
            //Assign loaded scenes to match scenes.
            _matchScenes[matchId] = args.LoadedScenes;
        }

        /// <summary>
        /// Called when client is first being added to a scene, or fully removed from a scene.
        /// </summary>
        /// <param name="args"></param>
        private void FlexSceneManager_OnClientPresenceChangeEnd(Events.ClientPresenceChangeEventArgs args)
        {
            string changedText = (args.Added) ? " added to " : " removed from ";
            //Go through match scenes and find out what match this player belongs to.
            foreach (KeyValuePair<int, Scene[]> item in _matchScenes)
            {
                //Go through each scene.
                for (int i = 0; i < item.Value.Length; i++)
                {
                    /* If the scene client was added to is found then the client
                    * belongs to this matchId. */
                    if (item.Value[i] == args.Scene)
                    {
                        Debug.Log("Client was" + changedText + "match id " + item.Key + ".");
                        return;
                    }
                }
            }
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            LoadScene(other.GetComponent<NetworkIdentity>());
        }

        private void LoadScene(NetworkIdentity triggeringIdentity)
        {
            //NetworkIdentity isn't necessarily needed but to ensure its the player only run if found.
            if (triggeringIdentity == null)
                return;

            SingleSceneData ssd = null;
            //Additive.
            AdditiveScenesData asd = new AdditiveScenesData(new string[] { _additiveScene });
            //Add matchId to parameters and increase so next client is loaded into a new match.
            object[] parameters = new object[] { _matchId };
            _matchId++;
            LoadOptions lo = new LoadOptions
            {
                LoadOnlyUnloaded = false,
            };
            LoadParams lp = new LoadParams
            {
                ServerParams = parameters
            };

            /* You can also use LoadParams within Networked scenes now.
             * Networked scenes will use the latest supplied load params
             * for late joiners. */
            FlexSceneManager.LoadConnectionScenes(triggeringIdentity.connectionToClient, ssd, asd, lo, lp);
        }


    }




}