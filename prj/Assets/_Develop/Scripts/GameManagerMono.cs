using ProgramDesignMock230211.Logics;
using ProgramDesignMock230211.Logics.Views;
using UnityEngine;

namespace ProgramDesignMock230211
{
    /// <summary>
    ///     GameManager
    /// </summary>
    public sealed class GameManagerMono : MonoBehaviour
    {
        /// <summary>
        ///     Othelloの3dView
        /// </summary>
        [SerializeField] private Othello3dViewMono _othello3dView;

        /// <summary>
        ///     盤面サイズ
        /// </summary>
        [SerializeField] private Vector2Int _gridSize;

        /// <summary>
        ///     Awake関数
        /// </summary>
        private void Awake()
        {
            var presenter = new OthelloPresenter(_gridSize, _othello3dView);
        }
    }
}
