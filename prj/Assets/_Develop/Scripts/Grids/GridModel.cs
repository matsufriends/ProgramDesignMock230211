using ProgramDesignMock230211.Pieces;
using UnityEngine;

namespace ProgramDesignMock230211.Grids
{
    /// <summary>
    /// オセロのロジック担当
    /// </summary>
    public sealed class GridModel
    {
        /// <summary>
        /// 盤面のサイズ
        /// </summary>
        private readonly Vector2Int _gridSize;

        /// <summary>
        /// 現在のプレイヤーのコマ色
        /// </summary>
        private readonly PieceColorKind _currentPlayerPieceColorKind;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="gridSize">盤面のサイズ</param>
        /// <param name="firstPlayerPieceColorKind">初期プレイヤーのコマ色</param>
        public GridModel(Vector2Int gridSize, PieceColorKind firstPlayerPieceColorKind)
        {
            _gridSize = gridSize;
            _currentPlayerPieceColorKind = firstPlayerPieceColorKind;
        }
    }
}
