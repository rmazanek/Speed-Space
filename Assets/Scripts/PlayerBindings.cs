using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerBindings : MonoBehaviour
{
    [SerializeField] float xMovementSensitivity = 1f;
    [SerializeField] float yMovementSensitivity = 1f;
    [SerializeField] float invincibilityTimeAfterShipDeath = 0.5f;
    [SerializeField] AudioClip shiftClockwiseSound;
    [SerializeField] float shiftClockwiseSoundVolume = 0.05f;
    [SerializeField] AudioClip shiftCounterClockwiseSound;
    [SerializeField] float shiftCounterClockwiseSoundVolume = 0.05f;
    [SerializeField] AudioClip buttonPressSound;
    [SerializeField] float buttonPressSoundVolume = 0.1f;
    PlayerControls controls;
    GameSession gameSession;
    Vector2 move;
    Player player;
    public ParentPositionMover ParentPositionMover;
    public List<Player> PlayerShips;
    public List<CrewMember> CrewMembers;
    public GameObject Players {get; set;}
    public HealthDisplay HealthDisplayPublic;
    List<Transform> healthTransforms;
    EngineSoundEffects engineSoundEffects;
    SceneLoader sceneLoader;
    MapBindings mapBindings;
    float cumulativeMovementModifier = 1f;
    bool shipControlsEnabled = false;
    bool initializeShipObjectsNeeded = false;
    bool rotateEnabled = true;
    int mainShipIndex = 0;
    public Player MainPlayer;
    private void Awake()
    {
        controls = new PlayerControls();

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
        sceneLoader.SetPlayerControls(controls);

        mapBindings = FindObjectOfType<MapBindings>();
        mapBindings.SetPlayerControls(controls);

        gameSession = FindObjectOfType<GameSession>();
    }

    // Update is called once per frame
    void Update()
    {
        if(initializeShipObjectsNeeded)
        {
            ParentPositionMover = GameObject.FindObjectOfType<ParentPositionMover>();
            engineSoundEffects = GameObject.FindObjectOfType<EngineSoundEffects>();
            initializeShipObjectsNeeded = false;
        }
        if(shipControlsEnabled)
        {
            ParentPositionMover.Move(move.x * Time.deltaTime * xMovementSensitivity * cumulativeMovementModifier, move.y * Time.deltaTime * yMovementSensitivity * cumulativeMovementModifier);
            if(engineSoundEffects.gameObject.activeInHierarchy)
            {
                engineSoundEffects.ThrusterSound(move.x + move.y);
            }
        }
        if(gameSession != null)
        {
            if(gameSession.GameStarted)
            {
                PlayingControls();
            }
            if(gameSession.HidePlayers)
            {
                //Debug.Log(players.name + " with " + players.transform.childCount + " children set inactive by gameSession update.");
                Players.SetActive(false);
                gameSession.HidePlayers = false;
            }
        }
    }
    public void SetGameSession(GameSession gameSessionFound)
    {
        gameSession = gameSessionFound;
    }
    public PlayerControls GetPlayerControls()
    {
        return controls;
    }
    public void InitializeShipControls()
    {
        initializeShipObjectsNeeded = true;
        EnableShipControls();      
    }
    public void EnableShipControls()
    {
        shipControlsEnabled = true;

        controls.Gameplay.Enable();

        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;      
    }
    public void DisableShipControls()
    {
        shipControlsEnabled = false;
        controls.Gameplay.Disable();
    }
    public void ExtractFromLevel(InputAction.CallbackContext context)
    {
        if(gameSession.GameStarted & gameSession.ExtractAvailable & context.performed)
        {

            Players = GameObject.Find("Players");
            PlayerShips = GetPlayerShips();
            CrewMembers = GetCrewMembersOrderedByPosition();
            StorePlayerItemLists();
            //SaveCrewMemberList();
            //Debug.Log(players.name + " with " + players.transform.childCount + " children set to the players variable.");
            gameSession.GameStarted = false;
            gameSession.HidePlayers = true;
            sceneLoader = FindObjectOfType<SceneLoader>();
            sceneLoader.LoadLevelEndScene();
        }
    }
    private void PlayingControls()
    {
        Debug.Log("PlayingControls run.");
        if(gameSession.CycleMainShip)
        {
            StartCoroutine(CycleMainShipStatusCoroutine());
        }
        if(gameSession.HealthUpdateNeeded)
        {
            SetNewMainShip();
            HealthDisplayPublic = FindObjectOfType<HealthDisplay>();
            HealthDisplayPublic.UpdateHealth();
            gameSession.HealthUpdateNeeded = false;
        }
    }
    public List<Player> GetPlayerShips()
    {
        var playerShipList = new List<Player>();
        var orderedPlayerManagerList = GetPlayerShipMangersOrderedByPosition();

        foreach(PlayerManager ship in orderedPlayerManagerList)
        {
            playerShipList.Add(ship.GetComponent<Player>());
        }

        return playerShipList;
    }
    private void StorePlayerItemLists()
    {
        foreach (Player player in GetPlayerShips())
        {
            player.StoreShipModifiers();
        }
    }
    //public List<CrewMember> GetCrewMembers()
    //{
    //    var crewMemberList = new List<CrewMember>();
//
    //    foreach(CrewMember crewMember in FindObjectsOfType<CrewMember>(true))
    //    {
    //        crewMemberList.Add(crewMember);
    //    }
//
    //    return crewMemberList;
    //}
    public List<CrewMember> GetCrewMembersOrderedByPosition()
    {
        var orderedPlayerManagerList = GetPlayerShipMangersOrderedByPosition();
        var crewMemberList = new List<CrewMember>();

        foreach(PlayerManager playerManager in orderedPlayerManagerList)
        {
            crewMemberList.AddRange(playerManager.GetComponentsInChildren<CrewMember>(true));
        }

        return crewMemberList;
    }
    public List<PlayerManager> GetPlayerShipManagers()
    {
        var playerShipManagerList = new List<PlayerManager>();

        foreach(PlayerManager playerManager in FindObjectsOfType<PlayerManager>())
        {
            playerShipManagerList.Add(playerManager);
        }

        return playerShipManagerList;
    }
    public List<PlayerManager> GetPlayerShipMangersOrderedByPosition()
    {
        var playerManagerList = GetPlayerShipManagers();
        return playerManagerList.OrderBy(o=>o.PositionIndex).ToList();
    }
    public void DecrementMainShipIndex()
    {
        PlayerShips = GetPlayerShips();

        if(mainShipIndex == 0 | mainShipIndex >= PlayerShips.Count)
        {
            mainShipIndex = PlayerShips.Count - 1;
        }
        else
        {
            mainShipIndex--;
        }
    }
    public void IncrementMainShipIndex()
    {
        PlayerShips = GetPlayerShips();

        if(mainShipIndex == PlayerShips.Count - 1)
        {
            mainShipIndex = 0;
        }
        else
        {
            mainShipIndex++;
        }
    }
    public void SetMainShipIndex(int index)
    {
        mainShipIndex = index;
    }
    public void SetNewMainShip()
    {
        var playerShipManagers = GetPlayerShipManagers();//.OrderBy(o=>o.PositionIndex).ToList();
        //int positionModifier = playerShipManagers.Count;

        foreach(PlayerManager p in playerShipManagers)
        {
            if(playerShipManagers.IndexOf(p) == mainShipIndex)
            {
                p.SetMainShipStatus(true);
                p.PositionIndex = 0;
                MainPlayer = p.GetComponent<Player>();
            }
            else
            {
                p.SetMainShipStatus(false);
                //Debug.Log(p.name + " has a playerShipManager index of " + playerShipManagers.IndexOf(p) + " while mainShipIndex is " + mainShipIndex);
                p.PositionIndex = playerShipManagers.IndexOf(p) - mainShipIndex;
                if(p.PositionIndex + playerShipManagers.Count < playerShipManagers.Count)
                {
                    p.PositionIndex = p.PositionIndex + playerShipManagers.Count;
                }
                //Debug.Log(p.name + " position index was set to: "+ p.PositionIndex + " = " + positionModifier + " - " + playerShipManagers.IndexOf(p));
                //positionModifier--;
            }
        }
    }
    public IEnumerator CycleMainShipStatusCoroutine()
    {
        DecrementMainShipIndex();
        gameSession.CycleMainShip = false;
        rotateEnabled = false;
        SetNewMainShip();
        HealthDisplayPublic = FindObjectOfType<HealthDisplay>();
        HealthDisplayPublic.UpdateHealth();
        var playerManagerMainPlayer = GetMainPlayer().GetComponent<PlayerManager>();
        playerManagerMainPlayer.SetMainShipStatus(false);
        yield return new WaitForSeconds(invincibilityTimeAfterShipDeath);
        rotateEnabled = true;
        playerManagerMainPlayer.SetMainShipStatus(true);
    }
    public void PlayerShipIncrementPositionIndex()
    {
        PlayerShips = GetPlayerShips();
        
        for(int index = 0; index < PlayerShips.Count; index++)
        {
            PlayerManager playerManager = PlayerShips[index].gameObject.GetComponent<PlayerManager>();
            if(playerManager.PositionIndex == PlayerShips.Count - 1)
            {
                playerManager.PositionIndex = 0;
            }
            else
            {
                playerManager.PositionIndex++;
            }
        }
    }
    private void PlayerShipPositionIndexDebugLog(string message)
    {
        PlayerShips = GetPlayerShips();
        
        for(int index = 0; index < PlayerShips.Count; index++)
        {
            PlayerManager playerManager = PlayerShips[index].gameObject.GetComponent<PlayerManager>();
            Debug.Log(playerManager.gameObject.name + " position index after " + message + " : " + playerManager.PositionIndex);
        }
    }
    public void PlayerShipDecrementPositionIndex()
    {
        PlayerShips = GetPlayerShips();
        
        for(int index = 0; index < PlayerShips.Count; index++)
        {
            PlayerManager playerManager = PlayerShips[index].gameObject.GetComponent<PlayerManager>();
            if(playerManager.PositionIndex == 0 | playerManager.PositionIndex >= PlayerShips.Count - 1)
            {
                playerManager.PositionIndex = PlayerShips.Count - 1;
            }
            else
            {
                playerManager.PositionIndex--;
            }
        }
    }
    public void MainShipRotate(InputAction.CallbackContext context)
    {
        //& Input.GetKeyDown(rotateShipsKeyCode)
        PlayerShips = GetPlayerShips();
        if (rotateEnabled && context.performed && (PlayerShips.Count > 1))
        {
            PlayerShipIncrementPositionIndex();
            //PlayerShipPositionIndexDebugLog("increment");
            DecrementMainShipIndex();
            //PlayerShipPositionIndexDebugLog("main ship index");
            SetNewMainShip();
            //PlayerShipPositionIndexDebugLog("set new main ship");
            ParentPositionMover.ShipPositionsNeedUpdate = true;
            AudioSource.PlayClipAtPoint(shiftClockwiseSound, Camera.main.transform.position, shiftClockwiseSoundVolume);
            HealthDisplayPublic = FindObjectOfType<HealthDisplay>();
            HealthDisplayPublic.UpdateHealth();
            //PlayerShipPositionIndexDebugLog("function end");
        }
    }
    public void MainShipRotateCounterClockwise(InputAction.CallbackContext context)
    {
        //& Input.GetKeyDown(rotateShipsKeyCode)
        PlayerShips = GetPlayerShips();
        if (rotateEnabled && context.performed && (PlayerShips.Count > 1))
        {
            PlayerShipDecrementPositionIndex();
            //PlayerShipPositionIndexDebugLog("decrement");
            IncrementMainShipIndex();
            //PlayerShipPositionIndexDebugLog("main ship index");
            SetNewMainShip();
            //PlayerShipPositionIndexDebugLog("set new main ship");
            ParentPositionMover.ShipPositionsNeedUpdate = true;
            AudioSource.PlayClipAtPoint(shiftCounterClockwiseSound, Camera.main.transform.position, shiftCounterClockwiseSoundVolume);
            HealthDisplayPublic = FindObjectOfType<HealthDisplay>();
            HealthDisplayPublic.UpdateHealth();
            //PlayerShipPositionIndexDebugLog("function end");
        }
    }
    public Player GetMainPlayer()
    {
        List<PlayerManager> playerManagers = new List<PlayerManager>();

        foreach (PlayerManager p in FindObjectsOfType<PlayerManager>())
        {
            playerManagers.Add(p);
        }
        //Set up a default player to choose if there end up being none.
        PlayerManager returnPlayerManager = playerManagers.Where(o => o.PositionIndex == 0).FirstOrDefault();
        Player returnPlayer = returnPlayerManager.GetComponent<Player>();

        return returnPlayer;    
    }
    public List<Transform> GetHealthBarTransforms()
    {
        List<Transform> healthBarTransforms = new List<Transform>();

        foreach(Player p in GetPlayerShips())
        {
            Transform healthBarTransform = p.transform.Find("HealthBarPosition").transform;
            healthBarTransforms.Add(healthBarTransform);
        }
        
        return healthBarTransforms;
    }
    void CreateHealthBars()
    {
        PlayerShips = GetPlayerShips();
        healthTransforms = GetHealthBarTransforms();

        for(int index = 0; index < PlayerShips.Count; index++)
        {
            GameObject healthBar = new GameObject();
            healthBar.transform.position = healthTransforms[index].position;
        }
    }
    void SaveCrewMemberList()
    {
        for(int index = 0; index < PlayerShips.Count; index++)
        {
            PlayerShips[index].SaveCrewMemberList();
        }
    }
    public void Menu()
    {
        Debug.Log("Menu PlayerBindings.");
        PauseMenu pauseMenu = GameObject.FindObjectOfType<PauseMenu>();
        if(pauseMenu != null)
        {
            if(PauseMenu.GamePaused)
            {
                pauseMenu.ResumeGame();
            }
            else
            {
                pauseMenu.PauseGame();
            }
            AudioSource.PlayClipAtPoint(buttonPressSound, Camera.main.transform.position, buttonPressSoundVolume);
        }
    }
    public void SwitchInputActions(string actionMapName)
    {
        PlayerInput playerInput = gameObject.GetComponent<PlayerInput>();
        playerInput.actions.FindActionMap(actionMapName).Enable();
        //playerInput.SwitchCurrentActionMap(actionMapName);

        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach(PlayerInput p in playerInputs)
        {
            Debug.Log(p.name);
        }
    }
}
