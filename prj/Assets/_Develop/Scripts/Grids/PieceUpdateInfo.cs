using ProgramDesignMock230211.Pieces;
using UnityEngine;

namespace ProgramDesignMock230211.Grids
{
    /// <summary>
    ///     コマの更新情報群
    /// </summary>
    public readonly struct PieceUpdateInfo
    {
        /// <summary>
        ///     コマの配置座標
        /// </summary>
        public readonly Vector2Int PlacePos;

        /// <summary>
        ///     コマ色
        /// </summary>
        public readonly PieceColorKind PieceColorKind;

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="placePos">コマの配置座標</param>
        /// <param name="pieceColorKind">コマ色</param>
        public PieceUpdateInfo(Vector2Int placePos, PieceColorKind pieceColorKind)
        {
            PlacePos = placePos;
            PieceColorKind = pieceColorKind;
        }
    }
}
