namespace ProgramDesignMock230211.Pieces
{
    /// <summary>
    ///     コマ色の列挙型
    /// </summary>
    public enum PieceColorKind
    {
        None = 0,
        Black = 1,
        White = 2,
    }

    /// <summary>
    ///     <see cref="PieceColorKind" />の拡張クラス
    /// </summary>
    public static class PieceColorKindEx
    {
        /// <summary>
        ///     反対のコマ色を返す拡張メソッド
        ///     NoneのときはNoneを返す
        /// </summary>
        /// <param name="pieceColorKind">基準のコマ色</param>
        /// <returns>基準のコマ色と反対のコマ色</returns>
        public static PieceColorKind OppositeColorKind(this PieceColorKind pieceColorKind)
        {
            switch (pieceColorKind)
            {
                case PieceColorKind.Black:
                    return PieceColorKind.White;
                case PieceColorKind.White:
                    return PieceColorKind.Black;
            }

            return PieceColorKind.None;
        }
    }
}
