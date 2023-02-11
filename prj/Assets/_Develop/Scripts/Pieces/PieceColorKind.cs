using System;

namespace ProgramDesignMock230211.Pieces
{
    public enum PieceColorKind
    {
        None = 0,
        Black = 1,
        White = 2,
    }

    public static class PieceColorKindEx
    {
        public static PieceColorKind OppositeColorKind(this PieceColorKind pieceColorKind)
        {
            switch (pieceColorKind)
            {
                case PieceColorKind.None:
                    return PieceColorKind.None;
                case PieceColorKind.Black:
                    return PieceColorKind.White;
                case PieceColorKind.White:
                    return PieceColorKind.Black;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pieceColorKind), pieceColorKind, null);
            }
        }
    }
}
