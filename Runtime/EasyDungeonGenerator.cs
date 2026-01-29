using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BreadThief.EasyDungeonGenerator
{
    /// <summary>
    /// Main dungeon generator class that creates procedural dungeon layouts using a connector-based room placement algorithm.
    /// The generator works by starting with a single room and iteratively attaching new rooms to available connectors.
    /// It supports performance optimization through frame-based generation and multiple regeneration attempts on failure.
    /// </summary>
    public class EasyDungeonGenerator : MonoBehaviour
    {
        [Tooltip("Total number of rooms to generate including the start room")]
        [SerializeField, Range(1, 5000), Space(5)] private int _numberOfRooms = 20;
        [Tooltip("Maximum number of regeneration attempts if generation fails to reach target room count")]
        [SerializeField, Range(1, 10)] private int _maxGenerationAttempts = 3;
        [Tooltip("Prefab for the starting room of the dungeon")]
        [SerializeField, Space] private GameObject _startRoomPrefab;
        [Tooltip("Array of room prefabs that can be randomly placed during generation")]
        [SerializeField] private GameObject[] _roomPrefabs;
        [Tooltip("Number of rooms to generate per frame for performance optimization")]
        [SerializeField, Space, Min(1)] private int _roomsPerFrame = 5;
        [Tooltip("Minimum distance between room centers when using distance-based collision check")]
        [SerializeField] private float _minRoomDistance = 15f;

        private List<GameObject> _spawnedRooms = new List<GameObject>();
        private List<RoomConnector> _availableConnectors = new List<RoomConnector>();
        private Dictionary<GameObject, Bounds> _roomBoundsCache = new Dictionary<GameObject, Bounds>();
        private bool _isGenerating = false;
        private int _currentGenerationAttempt = 0;
        private bool _is2DProject = false;

        private void Start()
        {
            DetectProjectType();
        }

        private void DetectProjectType()
        {
            bool has2D = false;
            bool has3D = false;

            if (_startRoomPrefab != null)
            {
                var spriteRenderer = _startRoomPrefab.GetComponentInChildren<SpriteRenderer>();
                var meshRenderer = _startRoomPrefab.GetComponentInChildren<MeshRenderer>();

                has2D = spriteRenderer != null;
                has3D = meshRenderer != null;
            }

            if (!has2D && !has3D && _roomPrefabs != null && _roomPrefabs.Length > 0)
            {
                foreach (var room in _roomPrefabs)
                {
                    if (room != null)
                    {
                        if (room.GetComponentInChildren<SpriteRenderer>() != null)
                            has2D = true;
                        if (room.GetComponentInChildren<MeshRenderer>() != null)
                            has3D = true;
                    }
                }
            }

            _is2DProject = has2D && !has3D;
        }

        /// <summary>
        /// Initiates the dungeon generation process.
        /// This method performs validation checks and starts the generation coroutine.
        /// Use the ContextMenu attribute to call this method from the Unity Editor.
        /// </summary>
        [ContextMenu("Generate Dungeon")]
        public void GenerateDungeon()
        {
            if (_isGenerating)
            {
                LogUtility.Log("Dungeon generation already in progress", MessageType.WARNING);
                return;
            }

            if (_startRoomPrefab == null)
            {
                LogUtility.Log("Start room prefab is not set. Generation aborted.", MessageType.ERROR);
                return;
            }

            if (_roomPrefabs == null || _roomPrefabs.Length == 0)
            {
                LogUtility.Log("Room prefabs array is empty. Generation aborted.", MessageType.ERROR);
                return;
            }

            ClearDungeon();
            StartCoroutine(GenerationEnumerator());
        }

        /// <summary>
        /// Clears all generated dungeon rooms and resets the generator state.
        /// This method destroys all spawned rooms and clears internal data structures.
        /// Use the ContextMenu attribute to call this method from the Unity Editor.
        /// </summary>
        [ContextMenu("Clear Dungeon")]
        public void ClearDungeon()
        {
            StopAllCoroutines();
            _isGenerating = false;
            _currentGenerationAttempt = 0;

            foreach (var room in _spawnedRooms)
            {
                if (room != null)
                {
                    Destroy(room);
                }
            }

            _spawnedRooms.Clear();
            _availableConnectors.Clear();
            _roomBoundsCache.Clear();
        }

        private IEnumerator GenerationEnumerator()
        {
            _isGenerating = true;
            _currentGenerationAttempt++;

            if (_currentGenerationAttempt > 2)
            {
                LogUtility.Log($"Generation attempt {_currentGenerationAttempt}/{_maxGenerationAttempts} started", MessageType.WARNING);
            }

            if (!SpawnStartRoom())
            {
                LogUtility.Log("Failed to create start room. Generation aborted.", MessageType.ERROR);
                _isGenerating = false;
                yield break;
            }

            int roomsPlacedInCurrentFrame = 0;
            int failedPlacementAttempts = 0;
            int maxFailedAttempts = 200;

            while (_spawnedRooms.Count < _numberOfRooms && _availableConnectors.Count > 0)
            {
                if (TryPlaceNextRoom())
                {
                    roomsPlacedInCurrentFrame++;
                    failedPlacementAttempts = 0;

                    if (roomsPlacedInCurrentFrame >= _roomsPerFrame)
                    {
                        roomsPlacedInCurrentFrame = 0;
                        yield return null;
                    }
                }
                else
                {
                    failedPlacementAttempts++;

                    if (failedPlacementAttempts >= maxFailedAttempts)
                    {
                        LogUtility.Log($"Maximum failed placement attempts reached ({maxFailedAttempts}) on attempt {_currentGenerationAttempt}.", MessageType.WARNING);
                        break;
                    }
                }
            }

            int generatedRooms = _spawnedRooms.Count;
            bool isSuccessful = generatedRooms >= _numberOfRooms;

            if (isSuccessful)
            {
                LogUtility.Log($"Generation complete. Target rooms reached: {generatedRooms}/{_numberOfRooms}", MessageType.SUCCESSFUL);
                _isGenerating = false;
                yield break;
            }
            else
            {
                LogUtility.Log($"Generation attempt {_currentGenerationAttempt} failed: {generatedRooms}/{_numberOfRooms} rooms", MessageType.WARNING);

                if (_currentGenerationAttempt < _maxGenerationAttempts)
                {
                    LogUtility.Log($"Starting regeneration attempt {_currentGenerationAttempt + 1}/{_maxGenerationAttempts}...", MessageType.WARNING);
                    yield return new WaitForEndOfFrame();

                    ClearDungeon();
                    StartCoroutine(GenerationEnumerator());
                }
                else
                {
                    LogUtility.Log($"Maximum generation attempts ({_maxGenerationAttempts}) reached. Best result: {generatedRooms}/{_numberOfRooms} rooms", MessageType.ERROR);
                    _isGenerating = false;
                }
            }
        }

        private bool SpawnStartRoom()
        {
            try
            {
                Vector3 position = Vector3.zero;
                if (_is2DProject)
                {
                    position.z = 0f;
                }

                GameObject startRoom = Instantiate(_startRoomPrefab, position, Quaternion.identity, transform);

                if (_is2DProject)
                {
                    Vector3 rotation = startRoom.transform.eulerAngles;
                    rotation.x = 0f;
                    rotation.z = 0f;
                    startRoom.transform.rotation = Quaternion.Euler(rotation);
                }

                RegisterRoom(startRoom);
                CacheRoomBounds(startRoom);
                return true;
            }
            catch (System.Exception ex)
            {
                LogUtility.Log($"Error creating start room: {ex.Message}", MessageType.ERROR);
                return false;
            }
        }

        private bool TryPlaceNextRoom()
        {
            if (_availableConnectors.Count == 0)
            {
                return false;
            }

            int connectorIndex = Random.Range(0, _availableConnectors.Count);
            RoomConnector targetConnector = _availableConnectors[connectorIndex];

            List<GameObject> allPrefabs = new List<GameObject>(_roomPrefabs);
            ArrayUtility.ShuffleList(allPrefabs);

            foreach (GameObject roomPrefab in allPrefabs)
            {
                if (roomPrefab == null)
                {
                    continue;
                }

                GameObject candidateRoom = Instantiate(roomPrefab);
                candidateRoom.SetActive(false);

                RoomConnector[] candidateConnectors = candidateRoom.GetComponentsInChildren<RoomConnector>();

                if (candidateConnectors.Length == 0)
                {
                    LogUtility.Log($"Room {roomPrefab.name} has no connectors!", MessageType.ERROR);
                    Destroy(candidateRoom);
                    continue;
                }

                List<RoomConnector> shuffledConnectors = new List<RoomConnector>(candidateConnectors);
                ArrayUtility.ShuffleList(shuffledConnectors);

                foreach (RoomConnector entryConnector in shuffledConnectors)
                {
                    if (entryConnector == null || entryConnector.IsConnected)
                    {
                        continue;
                    }

                    if (_is2DProject)
                    {
                        Vector3 targetForward = targetConnector.transform.forward;
                        Vector3 entryForward = entryConnector.transform.forward;

                        targetForward.z = 0f;
                        entryForward.z = 0f;

                        if (targetForward.magnitude < 0.1f || entryForward.magnitude < 0.1f)
                        {
                            continue;
                        }

                        targetForward.Normalize();
                        entryForward.Normalize();

                        float angle = Vector3.SignedAngle(entryForward, -targetForward, Vector3.forward);
                        candidateRoom.transform.Rotate(0, 0, angle);

                        Vector3 rotation = candidateRoom.transform.eulerAngles;
                        rotation.x = 0f;
                        rotation.y = 0f;
                        candidateRoom.transform.rotation = Quaternion.Euler(rotation);
                    }
                    else
                    {
                        float angle = Vector3.SignedAngle(entryConnector.transform.forward, -targetConnector.transform.forward, Vector3.up);
                        candidateRoom.transform.Rotate(0, angle, 0);
                    }

                    Vector3 offset = targetConnector.transform.position - entryConnector.transform.position;

                    if (_is2DProject)
                    {
                        offset.z = 0f;
                    }

                    candidateRoom.transform.position += offset;

                    if (_is2DProject)
                    {
                        Vector3 position = candidateRoom.transform.position;
                        position.z = 0f;
                        candidateRoom.transform.position = position;
                    }

                    Vector3 direction1, direction2;

                    if (_is2DProject)
                    {
                        direction1 = entryConnector.transform.forward;
                        direction2 = targetConnector.transform.forward;

                        direction1.z = 0f;
                        direction2.z = 0f;

                        if (direction1.magnitude < 0.1f || direction2.magnitude < 0.1f)
                        {
                            continue;
                        }

                        direction1.Normalize();
                        direction2.Normalize();
                    }
                    else
                    {
                        direction1 = entryConnector.transform.forward;
                        direction2 = targetConnector.transform.forward;
                    }

                    float alignment = Vector3.Dot(direction1, direction2);
                    if (alignment > -0.95f)
                    {
                        continue;
                    }

                    Vector3 forwardDir = _is2DProject ?
                        (targetConnector.transform.forward.normalized) :
                        targetConnector.transform.forward;

                    candidateRoom.transform.position += forwardDir * 0.01f;

                    if (_is2DProject)
                    {
                        Vector3 position = candidateRoom.transform.position;
                        position.z = 0f;
                        candidateRoom.transform.position = position;
                    }

                    if (!HasCollision(candidateRoom))
                    {
                        candidateRoom.SetActive(true);

                        if (_is2DProject)
                        {
                            Vector3 position = candidateRoom.transform.position;
                            position.z = 0f;
                            candidateRoom.transform.position = position;

                            Vector3 rotation = candidateRoom.transform.eulerAngles;
                            rotation.x = 0f;
                            rotation.y = 0f;
                            candidateRoom.transform.rotation = Quaternion.Euler(rotation);
                        }

                        CacheRoomBounds(candidateRoom);
                        FinalizeRoomPlacement(candidateRoom, targetConnector, entryConnector);
                        return true;
                    }
                }

                Destroy(candidateRoom);
            }

            _availableConnectors.RemoveAt(connectorIndex);
            return false;
        }

        private void CacheRoomBounds(GameObject room)
        {
            Renderer[] renderers = room.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                return;
            }

            Bounds combinedBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            if (_is2DProject)
            {
                Vector3 center = combinedBounds.center;
                center.z = 0f;
                combinedBounds.center = center;

                Vector3 size = combinedBounds.size;
                size.z = 0.1f;
                combinedBounds.size = size;
            }

            _roomBoundsCache[room] = combinedBounds;
        }

        private bool HasCollision(GameObject candidateRoom)
        {
            if (_spawnedRooms.Count == 0)
            {
                return false;
            }

            Renderer[] candidateRenderers = candidateRoom.GetComponentsInChildren<Renderer>();
            if (candidateRenderers.Length == 0)
            {
                return false;
            }

            Bounds candidateBounds = candidateRenderers[0].bounds;
            for (int i = 1; i < candidateRenderers.Length; i++)
            {
                candidateBounds.Encapsulate(candidateRenderers[i].bounds);
            }

            if (_is2DProject)
            {
                Vector3 candidateCenter = candidateBounds.center;
                candidateCenter.z = 0f;
                candidateBounds.center = candidateCenter;

                Vector3 candidateSize = candidateBounds.size;
                candidateSize.z = 0.1f;
                candidateBounds.size = candidateSize;
            }

            foreach (var existingRoom in _spawnedRooms)
            {
                if (existingRoom == null)
                {
                    continue;
                }

                if (_roomBoundsCache.ContainsKey(existingRoom))
                {
                    Bounds existingBounds = _roomBoundsCache[existingRoom];

                    if (_is2DProject)
                    {
                        Vector3 existingCenter = existingBounds.center;
                        existingCenter.z = 0f;
                        existingBounds.center = existingCenter;

                        Vector3 existingSize = existingBounds.size;
                        existingSize.z = 0.1f;
                        existingBounds.size = existingSize;
                    }

                    if (candidateBounds.Intersects(existingBounds))
                    {
                        return true;
                    }
                }
                else
                {
                    float distance;

                    if (_is2DProject)
                    {
                        Vector2 position1 = new Vector2(candidateRoom.transform.position.x, candidateRoom.transform.position.y);
                        Vector2 position2 = new Vector2(existingRoom.transform.position.x, existingRoom.transform.position.y);
                        distance = Vector2.Distance(position1, position2);
                    }
                    else
                    {
                        distance = Vector3.Distance(candidateRoom.transform.position, existingRoom.transform.position);
                    }

                    if (distance < _minRoomDistance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void FinalizeRoomPlacement(GameObject room, RoomConnector target, RoomConnector entry)
        {
            if (room == null)
            {
                return;
            }

            if (_is2DProject)
            {
                Vector3 position = room.transform.position;
                position.z = 0f;
                room.transform.position = position;

                Vector3 rotation = room.transform.eulerAngles;
                rotation.x = 0f;
                rotation.y = 0f;
                room.transform.rotation = Quaternion.Euler(rotation);
            }

            RegisterRoom(room);
            target.SetIsConnected(true);
            entry.SetIsConnected(true);
            _availableConnectors.Remove(target);
        }

        private void RegisterRoom(GameObject room)
        {
            room.transform.SetParent(this.transform);
            _spawnedRooms.Add(room);

            RoomConnector[] connectors = room.GetComponentsInChildren<RoomConnector>();
            foreach (RoomConnector connector in connectors)
            {
                if (connector != null && !connector.IsConnected)
                {
                    _availableConnectors.Add(connector);
                }
            }
        }

        /// <summary>
        /// Gets the list of spawned rooms
        /// </summary>
        public List<GameObject> SpawnedRooms => _spawnedRooms;

        /// <summary>
        /// Gets whether generation is currently in progress
        /// </summary>
        public bool IsGenerating => _isGenerating;
    }
}