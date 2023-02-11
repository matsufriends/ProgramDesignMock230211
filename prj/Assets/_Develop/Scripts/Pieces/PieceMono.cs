using UnityEngine;
using UnityEngine.Assertions;

namespace ProgramDesignMock230211.Pieces
{
    /// <summary>
    ///     PieceのMonoBehaviour
    /// </summary>
    public sealed class PieceMono : MonoBehaviour
    {
        /// <summary>
        ///     キャッシュしたコマ色
        /// </summary>
        private PieceColorKind _cachedPieceColorKind;

        /// <summary>
        ///     コマを更新する
        /// </summary>
        /// <param name="pieceScale">サイズ</param>
        /// <param name="worldPos">ワールド座標</param>
        /// <param name="pieceColorKind">コマ色</param>
        public void UpdatePiece(float pieceScale, Vector3 worldPos, PieceColorKind pieceColorKind)
        {
            Assert.IsTrue(pieceColorKind is PieceColorKind.Black or PieceColorKind.White);
            //TODO Animationをつける
            switch (pieceColorKind)
            {
                case PieceColorKind.Black:
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    break;
                case PieceColorKind.White:
                    transform.eulerAngles = new Vector3(180, 0, 0);
                    break;
            }

            transform.localScale = Vector3.one * pieceScale;
            transform.position = worldPos;
            _cachedPieceColorKind = pieceColorKind;
        }
    }
}
