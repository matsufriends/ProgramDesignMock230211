using ProgramDesignMock230211.Grids;
using ProgramDesignMock230211.Grids.Views;
using ProgramDesignMock230211.Pieces;
using UnityEngine;

namespace ProgramDesignMock230211
{
    /// <summary>
    /// GameManager
    /// </summary>
    public sealed class GameManagerMono : MonoBehaviour
    {
        /// <summary>
        /// Gridの3dView
        /// </summary>
        [SerializeField] private Grid3dViewMono _grid3dView;

        /// <summary>
        /// Gridのサイズ
        /// </summary>
        [SerializeField] private Vector2Int _gridSize;

        /// <summary>
        /// 先行プレイヤーのコマ色
        /// </summary>
        [SerializeField] private PieceColorKind _firstPlayerPieceColorKind;

        /// <summary>
        /// Awake関数
        /// </summary>
        private void Awake()
        {
            var model = new GridModel(_gridSize, _firstPlayerPieceColorKind);
            var presenter = new GridPresenter(model, _grid3dView);
        }
    }
}
