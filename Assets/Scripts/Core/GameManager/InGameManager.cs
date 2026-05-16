using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private UIController _uiController;
    private GameStateController<InGameState> _gameStateController;
    private InputProvider _inputProvider;
    private GoldSystem _goldSystem;

    [Space]
    
    [Header("0 - Hero")]
    [SerializeField] private HeroDatabase _heroDatabase;
    [SerializeField] private HeroView _heroViewPrefab;
    [SerializeField] private HeroSellZone _heroSellZone;

    [Space]

    [Header("1- Prepare Phase")]
    [SerializeField] private SummonTable _summonTable;
    [SerializeField] private SummonConfig _summonConfig;
    [SerializeField] private SquadConfig _squadConfig;
    [SerializeField] private HeroSlotController _summonSlotController;
    [SerializeField] private HeroSlotController _squadSlotController;
    [SerializeField] private SummonTrail _summonTrailPrefab;
    private SummonManager _summonManager;
    private SquadManager _squadManager;

    [Space]
    
    [Header("2 - Battle Phase")]
    [SerializeField] private RoundTable _roundTable;
    private RoundManager _roundManager;
    private BattleManager _battleManager;

    private void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        _gameStateController.OnStateChanged -= _cameraController.OnStateChanged;
        _gameStateController.OnStateChanged -= _uiController.OnStateChanged;
        if (SoundManager.Instance != null) _gameStateController.OnStateChanged -= SoundManager.Instance.OnStateChanged;
    }
    
    // TODO: 호버 규칙 다시 확인해보기 (이후에)
    private void Update()
    {
        // var screenPos = _inputProvider.GetScreenPos;
        // _summonSlotController.TryHoverAt(screenPos);
        // _squadSlotController.TryHoverAt(screenPos);
    }

    private void Initialize()
    {
        _gameStateController = new GameStateController<InGameState>();
        _cameraController.Initialize(null);
        _inputProvider = new InputProvider();
        _goldSystem = new GoldSystem(999);
        _summonManager = new SummonManager(_heroDatabase, _summonConfig, _summonTable);
        _squadManager = new SquadManager(_squadConfig);
        _summonManager.Initialize(_goldSystem.TrySpend);
        _squadManager.Initialize();
        _summonSlotController?.Initialize(
            _summonManager,
            _heroViewPrefab,
            OnHeroDrag,
            OnSummonHeroDropped,
            _summonTrailPrefab,
            _uiController.GetSummonButtonWorldPos);
        _squadSlotController?.Initialize(
            _squadManager,
            _heroViewPrefab,
            OnHeroDrag,
            OnSquadHeroDropped,
            null,
            null);
        _roundManager = new RoundManager(_roundTable);
        _battleManager = new BattleManager();
        _roundManager.Initialize(_ => _summonManager.ResetCost());
        _battleManager.Initialize();
        _uiController.Initialize(() => _summonManager.TrySummon(0), null);
        _goldSystem.Initialize(_uiController.SetGoldText);
        if (SoundManager.Instance != null) _gameStateController.OnStateChanged += SoundManager.Instance.OnStateChanged;
        _gameStateController.OnStateChanged += _uiController.OnStateChanged;
        _gameStateController.OnStateChanged += _cameraController.OnStateChanged;
        _gameStateController.ChangeState(InGameState.Prepare);
    }

    private void OnHeroDrag(Vector2 screenPos)
    {
        _summonSlotController.TryHoverAt(screenPos, true);
        _squadSlotController.TryHoverAt(screenPos);
    }

    private void OnHeroDropped()
    {
        _summonSlotController?.ClearHover();
        _squadSlotController?.ClearHover();
    }

    /// <summary>
    /// 소환 슬롯의 영웅을 드랍했을때
    /// </summary>
    private bool OnSummonHeroDropped(int srcIdx, Vector3 worldPos)
    {
        OnHeroDropped();

        if (_squadSlotController == null) return false;

        // 스쿼드 슬롯 내 드랍 위치 (슬롯 인덱스) 반환
        var dstIdx = _squadSlotController.GetSlotIndexAtWorldPos(worldPos);
        if (dstIdx == -1) return false;

        // 해당 슬롯이 비어있는지 체크
        var isSlotEmpty = _squadSlotController.IsSlotEmpty(dstIdx);
        if (isSlotEmpty == false) return false;

        // 소환 슬롯에서 영웅을 가져감
        var heroInstance = _summonManager.TakeHeroFromBench(srcIdx);
        if (heroInstance == null) return false;

        // 스쿼드 슬롯으로 영웅을 가져옴
        return _squadManager.AddHeroToBench(heroInstance, dstIdx);
    }

    /// <summary>
    /// 스쿼드 슬롯의 영웅을 드랍했을때
    /// </summary>
    private bool OnSquadHeroDropped(int srcIdx, Vector3 worldPos)
    {
        OnHeroDropped();

        // 1. 영웅을 판매하는 경우
        if (_heroSellZone != null && _heroSellZone.ContainsWorldPos(worldPos))
        {
            var heroInstance = _squadManager.TakeHeroFromBench(srcIdx);
            if (heroInstance == null) return false;

            _goldSystem.AddGold(_squadConfig.sellPrice);

            return true;
        }

        // 2. 스쿼드 내 영웅을 이동시키는 경우 (빈 슬롯이면 이동, 차 있으면 스왑)
        if (_squadSlotController != null)
        {
            var dstIdx = _squadSlotController.GetSlotIndexAtWorldPos(worldPos);
            if (dstIdx == -1) return false;

            return _squadManager.MoveHero(srcIdx, dstIdx);
        }

        return false;
    }
}
