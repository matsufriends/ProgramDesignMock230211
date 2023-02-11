using System;
using UniRx;
using UnityEngine;

namespace ProgramDesignMock230211.Boards
{
    /// <summary>
    ///     Board上の座標変換などを担当するクラス
    /// </summary>
    public sealed class BoardMono : MonoBehaviour
    {
        /// <summary>
        ///     BoardのColliderのLayer
        /// </summary>
        [SerializeField] private LayerMask _boardColliderLayer;

        /// <summary>
        ///     Board上の線のRenderer
        /// </summary>
        [SerializeField] private MeshRenderer _gridRenderer;

        /// <summary>
        ///     Board上の丸のRenderer
        /// </summary>
        [SerializeField] private MeshRenderer _gridCircleRenderer;

        /// <summary>
        ///     Grid座標をワールド座標に変換時のオフセット座標
        /// </summary>
        private Vector3 _gridPosToWorldPosOffset;

        /// <summary>
        ///     Grid座標をワールド座標に変換時の拡大率
        /// </summary>
        private Vector3 _gridPosToWorldPosScaler;

        /// <summary>
        ///     Raycastに使用するCamera
        /// </summary>
        private Camera _rayCamera;

        /// <summary>
        ///     Gridの選択を通知するSubject
        /// </summary>
        private readonly Subject<Vector2Int> _gridSelectSubject = new();

        /// <inheritdoc />
        public IObservable<Vector2Int> OnGridSelected => _gridSelectSubject;

        public float PieceScale { get; private set; }

        /// <summary>
        ///     GridSize変更のShaderIDのキャッシュ
        /// </summary>
        private static readonly int s_GridSize = Shader.PropertyToID("_GridSize");

        /// <summary>
        ///     基準のGridSize
        /// </summary>
        private const int BaseGridSize = 8;

        /// <summary>
        ///     BoardのCollider検知するRayの最大長
        /// </summary>
        private const int DetectBoardRayMaxDistance = 100;

        /// <summary>
        ///     Awake関数
        /// </summary>
        private void Awake()
        {
            _rayCamera = Camera.main;
        }

        /// <summary>
        ///     見た目の初期化処理
        /// </summary>
        /// <param name="gridSize">Gridの盤面サイズ</param>
        public void Initialize(Vector2Int gridSize)
        {
            PieceScale = 8f / Mathf.Max(gridSize.x, gridSize.y);
            _gridPosToWorldPosScaler = new Vector3((float)BaseGridSize / gridSize.x, 1, (float)BaseGridSize / gridSize.y);
            _gridPosToWorldPosOffset = new Vector3(-BaseGridSize / 2f + PieceScale / 2f, 0, -BaseGridSize / 2f + PieceScale / 2f);
            var matPropertyBlock = new MaterialPropertyBlock();
            matPropertyBlock.SetVector(s_GridSize, new Vector4(gridSize.x, gridSize.y, 0, 0));
            _gridRenderer.SetPropertyBlock(matPropertyBlock);
            _gridCircleRenderer.SetPropertyBlock(matPropertyBlock);
        }

        /// <summary>
        ///     Update関数
        /// </summary>
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) == false)
            {
                return;
            }

            var mouseRay = _rayCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out var raycastHitInfo, DetectBoardRayMaxDistance, _boardColliderLayer.value))
            {
                var gridPos = CovertWorldPosToGridPos(raycastHitInfo.point);
                _gridSelectSubject.OnNext(gridPos);
            }
        }

        /// <summary>
        ///     Grid上での座標をワールド座標に変換する
        /// </summary>
        /// <param name="gridPos">Grid上での座標</param>
        /// <returns>ワールド座標</returns>
        public Vector3 ConvertGridPosToWorldPos(Vector2Int gridPos)
        {
            var gridScaledPos = new Vector3(gridPos.x * _gridPosToWorldPosScaler.x, 0, gridPos.y * _gridPosToWorldPosScaler.z);
            var worldPos = transform.position + _gridPosToWorldPosOffset + gridScaledPos;
            return worldPos;
        }

        /// <summary>
        ///     ワールド座標をGrid上での座標に変換する
        /// </summary>
        /// <param name="worldPos">ワールド座標</param>
        /// <returns>Grid上での座標</returns>
        private Vector2Int CovertWorldPosToGridPos(Vector3 worldPos)
        {
            var gridPos = worldPos - transform.position - _gridPosToWorldPosOffset;
            gridPos.x /= _gridPosToWorldPosScaler.x;
            gridPos.z /= _gridPosToWorldPosScaler.z;
            return new Vector2Int(Mathf.RoundToInt(gridPos.x), Mathf.RoundToInt(gridPos.z));
        }
    }
}
