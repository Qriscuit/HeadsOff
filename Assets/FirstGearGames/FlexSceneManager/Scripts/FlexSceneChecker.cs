using FirstGearGames.Utilities.Maths;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGearGames.FlexSceneManager
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkIdentity))]
#pragma warning disable CS0618 // Type or member is obsolete
    public class FlexSceneChecker : NetworkVisibility
    {
        #region Public.
        /// <summary>
        /// True if this scene checker also uses proximity.
        /// </summary>
        public bool UsesContinuousProximity { get { return (_proximityDistance > 0f && _continuous); } }
        #endregion

        #region Serialized.
        /// <summary>
        /// Enable to force this object to be hidden from all observers.
        /// <para>If this object is a player object, it will not be hidden for that client.</para>
        /// </summary>
        [Tooltip("Enable to force this object to be hidden from all observers. If this object is a player object, it will not be hidden for that client.")]
        [SerializeField]
        private bool _forceHidden = false;
        /// <summary>
        /// True to synchronize which scene the object was spawned in to clients. When true this object will be moved to the clients equivelant of the scene it was spawned in on the server.
        /// </summary>
        [Tooltip("True to synchronize which scene the object was spawned in to clients. When true this object will be moved to the clients equivelant of the scene it was spawned in on the server.")]
        [SerializeField]
        private bool _synchronizeScene = false;
        /// <summary>
        /// If not 0 only show other objects within this proximity that are in the same scene.
        /// </summary>
        [Tooltip("If not 0 only show other objects within this proximity that are in the same scene.")]
        [Range(0f, 10000)]
        [SerializeField]
        private float _proximityDistance = 0f;
        /// <summary>
        /// True to continuously update network visibility. False to only update on creation or when PerformCheck is called.
        /// </summary>
        [Tooltip("True to continuously update network visibility. False to only update on creation or when PerformCheck is called.")]
        [SerializeField]
        private bool _continuous = true;
        /// <summary>
        /// True to only check distance from the localPlayer object. False to compare distance from any player object. False is useful if the player can have authority over multiple objects which need to be affected by proximity checkers.
        /// </summary>
        [Tooltip("True to only check distance from the localPlayer object. False to compare distance from any player object. False is useful if the player can have authority over multiple objects which need to be affected by proximity checkers.")]
        [SerializeField]
        private bool _localPlayerOnly = true;
        #endregion

        #region Private.
        /// <summary>
        /// Squared value of proximity distance.
        /// </summary>
        private float _squaredProximityDistance;
        #endregion

        private void Awake()
        {
            SquareRange();
        }

        private void OnEnable()
        {
            if (NetworkServer.active)
                FlexSceneManager.AddSceneChecker(this);
        }
        private void OnDisable()
        {
            /* Server may not be active OnDisable if object is disabled
             * after server shutsdown. To prevent checkers being added
             * but not removed RemoveSceneChecker will be called
             * OnDisable regardless if server or not. If client the
             * scene checkers list will be empty since they're only
             * added on server, and this will incur no penalty. */
            FlexSceneManager.RemoveSceneChecker(this);
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (_synchronizeScene && initialState)
            {
                writer.WriteString(gameObject.scene.name);
            }
            return base.OnSerialize(writer, initialState);
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (_synchronizeScene && initialState)
            {
                string sceneName = reader.ReadString();
                Scene s = SceneManager.GetSceneByName(sceneName);
                if (!string.IsNullOrEmpty(s.name))
                    SceneManager.MoveGameObjectToScene(gameObject, s);
                else
                    Debug.LogWarning($"Scene could not be found for {sceneName}.");
            }
            base.OnDeserialize(reader, initialState);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();

        }

        /// <summary>
        /// Manually rebuilds observers.
        /// </summary>
        [Server]
        public void RebuildObservers()
        {
            base.netIdentity.RebuildObservers(false);
        }

        /// <summary>
        /// Forces observers to update.
        /// </summary>
        [Server]
        public void PerformCheck()
        {
            base.netIdentity.RebuildObservers(false);
        }

        /// <summary>
        /// Callback used by the visibility system to determine if an observer (player) can see this object.
        /// <para>If this function returns true, the network connection will be added as an observer.</para>
        /// </summary>
        /// <param name="newObserver">Network connection of a player.</param>
        /// <returns>True if the player can see this object.</returns>
        public override bool OnCheckObserver(NetworkConnection newObserver)
        {
            if (_forceHidden)
                return false;

            if (FlexSceneManager.SceneConnections.TryGetValue(gameObject.scene, out HashSet<NetworkConnection> sceneConnections))
            {
                bool inScene = sceneConnections.Contains(newObserver);
                //Is in same scene.
                if (inScene)
                    return SceneConnectionInRange(newObserver, transform.position);
                //Not in scene.
                else
                    return false;
            }

            //Fall through. Scene doesn't exist in collection therefor no identities are added to it.
            return false;
        }

        /// <summary>
        /// Callback used by the visibility system to (re)construct the set of observers that can see this object.
        /// <para>Implementations of this callback should add network connections of players that can see this object to the observers set.</para>
        /// </summary>
        /// <param name="observers">The new set of observers for this object.</param>
        /// <param name="initialize">True if the set of observers is being built for the first time.</param>
        public override void OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
        {
            //Hidden to clients, don't add any observers.
            if (_forceHidden)
                return;

            Vector3 position = transform.position;
            if (FlexSceneManager.SceneConnections.TryGetValue(gameObject.scene, out HashSet<NetworkConnection> sceneConnections))
            {               
                //For every object in the same scene.
                foreach (NetworkConnection conn in sceneConnections)
                {
                    if (SceneConnectionInRange(conn, position))
                        observers.Add(conn);
                }
            }
        }

        /// <summary>
        /// Returns if a scene connection is within the range visibility setting.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="thisPosition">Position of this transform.</param>
        /// <returns></returns>
        private bool SceneConnectionInRange(NetworkConnection conn, Vector3 thisPosition)
        {
            //Connection or identity went null.
            if (conn == null || conn.identity == null)
                return false;
            //No distance check required.
            if (_squaredProximityDistance == 0f)
                return true;

            //Check only against local player object.
            if (_localPlayerOnly)
            {
                return Vectors.FastSqrMagnitude(thisPosition - conn.identity.transform.position) < _squaredProximityDistance;
            }
            //Include all player objects.
            else
            {
                //Add becomes true if any object for the connection is within range.
                bool inRange = false;
                foreach (NetworkIdentity netId in conn.clientOwnedObjects)
                {
                    if (Vectors.FastSqrMagnitude(thisPosition - netId.transform.position) < _squaredProximityDistance)
                    {
                        inRange = true;
                        break;
                    }
                }

                return inRange;
            }
        }

        /// <summary>
        /// Squares current visibility range for testing.
        /// </summary>
        private void SquareRange()
        {
            _squaredProximityDistance = (_proximityDistance * _proximityDistance);
        }

        #region Editor.
#if UNITY_EDITOR
        private void OnValidate()
        {
            SquareRange();
        }
#endif
        #endregion
    }
}
